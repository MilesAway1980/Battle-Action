using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Mirror;

public class PlayerPointers : NetworkBehaviour
{
    List<Pointer> pointers = new List<Pointer>();

    public float playerPointerSize;
    public float playerPointerDistance;
        
    public float beaconPointerSize;
    public float beaconPointerDistance;
    
    
    public float powerupPointerSize;    
    public float powerupPointerDistance;

    int playerPointerCount = 0;

    Ship ship;
    Player player;

    Vector2 closestBeacon;
    bool closeToBeacon = false;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            pointers.Add(Pointer.CreateNewPointer(PointerType.Beacon, null, beaconPointerSize, beaconPointerDistance));
            pointers.Add(Pointer.CreateNewPointer(PointerType.PowerUp, null, powerupPointerSize, powerupPointerDistance));
            player = GetComponent<Player>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (ship == null)
        {
            ship = Ship.GetShipByGuid(player.GetPlayerGuid());            
        }

        if (ship == null)
        {
            pointers.ForEach(p => p.SetActive(false));
            return;
        }
        else
        {
            pointers.ForEach(p => p.SetOwner(ship.gameObject));
        }

        Ship[] allPlayerShips = Ship.GetAllShips();
        Ship[] opponentShips = new Ship[allPlayerShips.Length - 1];

        int count = 0;
        for (int i = 0; i < allPlayerShips.Length; i++)
        {
            if (allPlayerShips[i].GetOwnerGuid() != ship.GetOwnerGuid())
            {
                opponentShips[count++] = allPlayerShips[i];
            }
        }

        playerPointerCount = pointers.Where(p => p.GetPointerType() == PointerType.Player).Count();

        int pointerNeed = allPlayerShips.Length - playerPointerCount - 1;
        
        if (pointerNeed > 0)
        {
            //ADD MORE PLAYER POINTERS!
            
            for (int i = 0; i < pointerNeed; i++)
            {
                pointers.Add(Pointer.CreateNewPointer(PointerType.Player, ship.gameObject, playerPointerSize, playerPointerDistance));
            }
        }

        count = 0;
        pointers.ForEach(
            p =>
            {
                switch (p.GetPointerType())
                {
                    case PointerType.Beacon:
                        p.SetActive(true);
                        closestBeacon = p.UpdatePointer();
                        closeToBeacon = Vector2.Distance(ship.transform.position, closestBeacon) <= ArenaInfo.GetBeaconRange();

                        break;

                    case PointerType.PowerUp:

                        if (Powerup.GetPowerupCount() == 0)
                        {
                            p.SetActive(false);
                        }
                        else
                        {
                            Vector2 closestPowerup = p.UpdatePointer();

                            if (closeToBeacon || Vector2.Distance(ship.transform.position, closestPowerup) <= ArenaInfo.GetShipRadarRange())
                            {
                                p.SetActive(true);
                            }
                            else
                            {
                                p.SetActive(false);
                            }
                        }

                        break;

                    case PointerType.Player:
                        if (count < opponentShips.Length)
                        {
                            Ship otherShip = opponentShips[count++];

                            Vector2 otherShipPos;
                            if (otherShip.HasDecoy())
                            {
                                Decoy[] decoys = Decoy.GetPlayerDecoysByGuid(otherShip.GetOwnerGuid());
                                if (decoys.Length > 0)
                                {
                                    otherShipPos = decoys[0].gameObject.transform.position;
                                }
                                else
                                {
                                    Debug.LogError("Other Ship has Decoy but no Decoys were returned");
                                    otherShipPos = otherShip.transform.position;
                                }
                            }
                            else
                            {
                                otherShipPos = otherShip.transform.position;
                            }

                            bool closeEnough = false;
                            if (closeToBeacon)
                            {
                                closeEnough = true;
                            }
                            else
                            {
                                float distance = Vector2.Distance(ship.transform.position, otherShipPos);
                                if (distance <= ArenaInfo.GetShipRadarRange())
                                {
                                    closeEnough = true;
                                }
                            }

                            if (closeEnough)
                            {
                                p.SetActive(true);
                                p.UpdatePointer(otherShipPos);
                            }
                            else
                            {
                                p.SetActive(false);
                            }
                        }
                        else
                        {
                            p.SetActive(false);
                        }
                        
                        break;
                }
            }
        );
    }    
}
