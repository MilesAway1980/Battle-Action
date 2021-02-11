using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Mirror;

public class PlayerGUI : NetworkBehaviour
{
    // Start is called before the first frame update
    public TMP_Text currentWeaponText;
    public TMP_Text junk;

    List<GUIMinMax> minMax;

    Canvas mainCanvas;
    Player player;
    Ship ship;

    public GUIMinMax speed;
    public GUIMinMax thrust;
    public GUIMinMax armor;
    public GUIMinMax shield;
    public GUIMinMax ammo;

    bool initialized = false;

    void Start()
    {


        List<Canvas> canvases = FindObjectsOfType<Canvas>().ToList();
        canvases.ForEach(c =>
        {
            if (c.name == "Player GUI Canvas")
            {
                mainCanvas = c;
            }
        });

        mainCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            player = Player.GetLocalPlayer();
        }

        if (player)
        {
            Init();

            if (ship)
            {

                Damageable damage = ship.GetComponent<Damageable>();
                BulletShooter shooter = ship.GetComponent<BulletShooter>();
                Shield shieldComp = ship.GetComponent<Shield>();

                speed.current.text = (ship.GetSpeed() * speed.displayMod).ToString(speed.template);
                speed.max.text = (ship.GetMaxSpeed() * speed.displayMod).ToString();

                thrust.current.text = (ship.GetThrust() * thrust.displayMod).ToString(thrust.template);
                thrust.max.text = (ship.GetMaxThrust() * thrust.displayMod).ToString();

                if (damage)
                {
                    armor.current.text = (damage.GetArmor() * armor.displayMod).ToString(armor.template);
                    armor.max.text = (damage.GetMaxArmor() * armor.displayMod).ToString();
                }
                else
                {
                    armor.current.text = " - ";
                    armor.max.text = " - ";
                }

                int currentWeapon;
                if (shooter)
                {
                    currentWeapon = shooter.GetCurrentWeapon();
                    Ammo currentAmmo = shooter.GetWeaponAmmo();

                    if (currentAmmo != null)
                    {
                        ammo.current.text = currentAmmo.GetCurrentAmmo().ToString(ammo.template);
                        ammo.max.text = currentAmmo.GetMaxAmmo().ToString(ammo.template);
                    }
                    
                    currentWeaponText.text = WeaponInfo.GetWeaponName(currentWeapon);
                }
                else
                {
                    currentWeaponText.text = " - ";
                    ammo.current.text = " - ";
                    ammo.max.text = " - ";
                }

                if (shieldComp)
                {
                    shield.current.text = (shieldComp.GetCharge() * shield.displayMod).ToString(shield.template);
                    shield.max.text = (shieldComp.GetMaxCharge() * shield.displayMod).ToString();
                }

                //junk.text = ship.GetTurnRate().ToString();

                junk.text = ScenePersistence.GetPlayerGuid().ToString() + "  " + ScenePersistence.GetShipSelection();
            }
        }
        else
        {
            mainCanvas.enabled = false;
            initialized = false;
        }

    }

    private void Init()
    {
        if (initialized)
        {
            return;
        }

        ship = player.GetShip();

        if (!ship)
        {
            return;
        }

        mainCanvas.enabled = true;
        initialized = true;
    }
}
