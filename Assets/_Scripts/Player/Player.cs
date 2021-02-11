using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Player : NetworkBehaviour
{

    static Player thisPlayer;

    static int playerCount = 0;
    [SyncVar] Guid playerGuid;
    [SyncVar] int shipSelection;

    [SyncVar] int kills;
    [SyncVar] float lastTimeAlive;
    [SyncVar] public float spawnDelay;

    bool pointersActive;

    Chat chatMessages;
    bool newMessage = false;
    bool initialShip = true;

    string chatText;

    public SyncList<string> texts = new SyncList<string>();

    static Dictionary<Guid, int> playerShipSelectionRegistry = new Dictionary<Guid, int>();
    static Dictionary<Guid, Ship> playerShipRegistry = new Dictionary<Guid, Ship>();
    static Dictionary<Guid, Player> playerRegistery = new Dictionary<Guid, Player>();

    PlayerControls controls;
    ShipCamera shipCamera;

    Ship ship;

    void Awake()
    {
        playerCount++;
    }

    void Start()
    {

        chatMessages = new Chat();
        chatMessages.SetMessageHistorySize(5);
        chatMessages.SetMessageLifeSpan(10000);

        if (isLocalPlayer)
        {
            thisPlayer = this;

            kills = 0;

            shipCamera = gameObject.AddComponent<ShipCamera>();
            shipCamera.SetHeight(10);

            controls = gameObject.GetComponent<PlayerControls>();
            
            playerGuid = ScenePersistence.GetPlayerGuid();
            shipSelection = ScenePersistence.GetShipSelection();
            CmdRegisterGuid(playerGuid);

            CmdRegisterPlayerShip(playerGuid, shipSelection);
        }

        pointersActive = false;
    }

    void OnDestroy()
    {
        if (isLocalPlayer)
        {
            //TODO  Doesn't work - player doesn't "have authority"??
            CmdRemovePlayer(playerGuid);
        }
    }

    [Command]
    void CmdRegisterGuid(Guid guid)
    {
        playerGuid = guid;
        if (playerRegistery.ContainsKey(guid))
        {
            playerRegistery[guid] = this;
        }
        else
        {
            playerRegistery.Add(guid, this);
        }        
    }

    void Update()
    {

        if (isLocalPlayer)
        {
            controls.CheckControls();

            if (ship == null)
            {
                ship = Ship.GetShipByGuid(playerGuid);
                shipCamera.SetTargetShip(ship);
            }
            else
            {
                shipCamera.UpdatePosition(ship.transform.position);                
            }
        }

        if (isServer)
        {
            CheckPlayerShips();            
        }

        /*

        if (newMessage == true)
        {
            print("Passing: " + chatMessages);
            if (chatMessages != null)
            {
                string[] messages = chatMessages.GetMessages();
                if (messages != null)
                {
                    print(messages.Length);
                }
            }
            CmdSendMessage(chatMessages);
            newMessage = false;
        }*/
    }

    [Command]
    void CmdRegisterPlayerShip(Guid playerGuid, int shipSelection)
    {
        if (playerShipSelectionRegistry.ContainsKey(playerGuid))
        {
            playerShipSelectionRegistry[playerGuid] = shipSelection;
        }
        else
        {
            playerShipSelectionRegistry.Add(playerGuid, shipSelection);
        }        
    }

    [Command]
    void CmdRemovePlayer(Guid playerGuid)
    {
        print($"Removing player {playerGuid}");
        if (playerShipSelectionRegistry.ContainsKey(playerGuid))
        {
            playerShipSelectionRegistry.Remove(playerGuid);
            print($"Removed player {playerGuid}");
        }
    }    

    [Server]
    void CheckPlayerShips()
    {
        Guid[] guids = playerShipSelectionRegistry.Keys.ToArray();
        
        for (int i = 0; i < guids.Length; i++)
        {
            Guid playerGuid = guids[i];

            if (!playerShipRegistry.ContainsKey(playerGuid)) 
            {
                playerShipRegistry.Add(playerGuid, null);
            }
            
            Ship ship = playerShipRegistry[playerGuid];
            Player player = playerRegistery[playerGuid];
            if (ship == null)
            {
                //Player doesn't have a ship
                int selection = playerShipSelectionRegistry[playerGuid];

                if ((Time.realtimeSinceStartup - player.GetLastTimeAlive()) >= spawnDelay || player.initialShip)
                {
                    ship = PlayerShipSpawner.SpawnShip(playerGuid, selection);
                    player.initialShip = false;
                }

                playerShipRegistry[playerGuid] = ship;
            }
            else
            {
                player.UpdateLastTimeAlive();
            }
        }
    }

    [Server]
    static public int? GetPlayerShipSelection(Guid playerGuid)
    {
        if (playerShipSelectionRegistry.ContainsKey(playerGuid))
        {
            return playerShipSelectionRegistry[playerGuid];
        }
        else
        {
            return null;
        }
    }

    [Server]
    static public Ship GetPlayerShip(Guid playerGuid)
    {
        if (playerShipRegistry.ContainsKey(playerGuid))
        {
            return playerShipRegistry[playerGuid];
        }
        else
        {
            return null;
        }
    }

    void ActivatePointers()
    {
        if (pointersActive == false)
        {
            Pointer[] pointers = ship.GetComponents<Pointer>();
            for (int i = 0; i < pointers.Length; i++)
            {
                pointers[i].SetActive(true);
            }

            pointersActive = true;
        }
    }

    [Command]
    void CmdDestroyShip(Ship ship)
    {
        if (ship != null)
        {
            Damageable d = ship.GetComponent<Damageable>();
            if (d)
            {
                d.Destruct();
            }
        }
    }

    [Command]
    void CmdSendMessage(Chat sendMessages)
    {
        print("Sending Message!");
        print(sendMessages);
        if (sendMessages != null)
        {
            string[] all = sendMessages.GetMessages();
            if (all != null)
            {
                print(all.Length);
            }
            else
            {
                print("messages are null");
            }
        }
        RpcReceiveMessage(sendMessages);
    }

    [ClientRpc]
    void RpcReceiveMessage(Chat newChatMessage)
    {
        print("Receiving message!");
        if (newChatMessage != null)
        {
            string[] messages = newChatMessage.GetMessages();
            if (messages != null)
            {
                print(messages.Length);
            }
        }
        else
        {
            print(newChatMessage);
        }
        chatMessages = newChatMessage;
    }

    public static int GetPlayerCount()
    {
        return playerCount;
    }

    public Ship GetShip()
    {
        return ship;
    }

    public static Player GetLocalPlayer()
    {
        return thisPlayer;
    }

    [Server]
    public static Player GetPlayerByGuid(Guid guid)
    {
        Player[] players = FindObjectsOfType<Player>();
        print(players.Length);
        for (int p = 0; p < players.Length; p++)
        {
            print (players[p].GetPlayerGuid());
            if (players[p].GetPlayerGuid() == guid)
            {
                return players[p];
            }
        }

        return null;
    }

    public Guid GetPlayerGuid()
    {
        return playerGuid;
    }

    [Server]
    public float GetLastTimeAlive()
    {
        return lastTimeAlive;
    }

    [Server]
    public void UpdateLastTimeAlive()
    {        
        lastTimeAlive = Time.realtimeSinceStartup;
    }

    public void AddKill()
    {
        kills++;
    }

    public void DumpAmmo()
    {

    }
}