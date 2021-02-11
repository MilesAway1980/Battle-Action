using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class Powerup : NetworkBehaviour
{

    int whichPowerup;

    static PowerupManager manager;
    //static List<Powerup> powerups = new List<Powerup>();
    public static ObjectList powerupList = new ObjectList();

    // Start is called before the first frame update
    void Start()
    {
        powerupList.AddObject(gameObject);

        if (manager == null)
        {
            manager = FindObjectOfType<PowerupManager>();
        }

        float random = Random.Range(0.0f, 1.0f);

        for (int i = 0; i < PowerupManager.rarity.GetLength(0); i++)
        {
            if (random >= PowerupManager.rarity[i, 0] && random <= PowerupManager.rarity[i, 1])
            {
                whichPowerup = i;
                break;
            }
        }

        name = ArenaInfo.GetWeaponData(whichPowerup).name;

        GetSprite();
    }

    private void OnDestroy()
    {
        powerupList.RemoveObject(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        /*
          
             whichPowerup
          
                0 - Machine Gun
                1 - Rocket
                2 - Missile
                3 - Blaster
                4 - Crush
                5 - Nuke
                6 - Warp
                7 - Plasma
                8 - Mines
                9 - Decoy
                10 - Turret
                11 - Deactivator

                12 - Small Health
                13 - Medium Health
                14 - Large Health

         */

        Ship ship = collision.gameObject.GetComponent<Ship>();

        if (ship)
        {
            WeaponData powerup = ArenaInfo.GetWeaponData(PowerupManager.GetPowerupName(whichPowerup));

            if (whichPowerup >= 0 && whichPowerup <= 11)
            {
                BulletShooter shooter = ship.GetComponent<BulletShooter>();
                Ammo ammo = shooter.GetWeaponAmmo(whichPowerup);
                if (shooter != null && ammo != null)
                {
                    int amount;
                    switch (powerup.amountCollectedType)
                    {
                        case 0: //Amount
                            amount = (int)powerup.amountCollected;
                            ammo.IncreaseAmmo(amount);

                            break;
                        case 1: //Percent

                            amount = (int)(ammo.GetMaxAmmo() * powerup.amountCollected / 100.0f);
                            ammo.IncreaseAmmo(amount);

                            break;
                        case 2: //Full
                            ammo.FillAmmo();
                            break;
                    }
                }
            }
            else
            {
                Damageable dm = ship.GetComponent<Damageable>();
                if (dm)
                {
                    int health;
                    switch (powerup.amountCollectedType)
                    {
                        case 0: //Amount

                            health = (int)powerup.amountCollected;
                            dm.Heal(health);

                            break;
                        case 1: //Percent

                            health = (int)(powerup.amountCollected * dm.GetArmor() / 100.0f);
                            dm.Heal(health);

                            break;
                        case 2: //Full
                            dm.HealFull();
                            break;
                    }


                    //dm.Heal()
                }
            }

            Destroy(gameObject);
        }
    }

    void GetSprite()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        switch (whichPowerup)
        {
            case 0: renderer.sprite = manager.machineGun; break;
            case 1: renderer.sprite = manager.rockets; break;
            case 2: renderer.sprite = manager.missile; break;
            case 3: renderer.sprite = manager.blaster; break;
            case 4: renderer.sprite = manager.crush; break;
            case 5: renderer.sprite = manager.nuke; break;
            case 6: renderer.sprite = manager.warp; break;
            case 7: renderer.sprite = manager.plasma; break;
            case 8: renderer.sprite = manager.mines; break;
            case 9: renderer.sprite = manager.decoy; break;
            case 10: renderer.sprite = manager.turret; break;
            case 11: renderer.sprite = manager.deactivator; break;

            case 12: renderer.sprite = manager.smallHealth; break;
            case 13: renderer.sprite = manager.mediumHealth; break;
            case 14: renderer.sprite = manager.largeHealth; break;
        }
    }

    static public int GetPowerupCount()
    {
        return powerupList.Count;
    }
}
