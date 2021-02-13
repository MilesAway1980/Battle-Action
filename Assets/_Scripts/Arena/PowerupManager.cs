using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class PowerupManager : NetworkBehaviour
{
    // Start is called before the first frame update

    public GameObject powerupTemplate;

    public Sprite machineGun;
    public Sprite rockets;
    public Sprite missile;
    public Sprite blaster;
    public Sprite crush;
    public Sprite nuke;
    public Sprite warp;
    public Sprite plasma;
    public Sprite mines;
    public Sprite decoy;
    public Sprite turret;
    public Sprite deactivator;

    public Sprite smallHealth;
    public Sprite mediumHealth;
    public Sprite largeHealth;

    float lastCheck;

    public static float[,] rarity;

    GameObject allPowerups;

    void Start()
    {
        allPowerups = new GameObject();
        allPowerups.name = "Powerups";

        for (int i = 0; i < ArenaInfo.GetStartingPowerups(); i++)
        {
            SpawnPowerup();
        }

        WeaponData[] weaponData = ArenaInfo.GetWeaponData().ToArray();
        float pointer = 0;

        rarity = new float[weaponData.Length, 2];

        for (int i = 0; i < weaponData.Length; i++)
        {

            WeaponData powerup = ArenaInfo.GetWeaponData(i);

            rarity[i, 0] = pointer;
            rarity[i, 1] = pointer + powerup.rarity;

            pointer += powerup.rarity;
        }

        for (int i = 0; i < rarity.GetLength(0); i++)
        {
            WeaponData powerup = ArenaInfo.GetWeaponData(i);
        }

        lastCheck = ArenaInfo.GetPowerupRespawnRate();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.realtimeSinceStartup - lastCheck) > ArenaInfo.GetPowerupRespawnRate())
        {
            lastCheck = Time.realtimeSinceStartup;

            if (Powerup.GetPowerupCount() < ArenaInfo.GetMaxPowerups())
            {
                SpawnPowerup();
            }
        }
    }

    void SpawnPowerup()
    {
        if (!isServer)
        {
            return;
        }

        GameObject newPowerup = Instantiate(
            powerupTemplate,
            ArenaInfo.GetRandomArenaLocation(),
            Quaternion.identity
        );

        newPowerup.transform.parent = allPowerups.transform;

        NetworkServer.Spawn(newPowerup);
    }

    public static string GetPowerupName(int whichPowerup)
    {
        /*return whichPowerup switch
        {
            0 => "machinegun",
            1 => "rocket",
            2 => "missile",
            3 => "blaster",
            4 => "crush",
            5 => "nuke",
            6 => "warp",
            7 => "plasma",
            8 => "mines",
            9 => "decoy",
            10 => "turret",
            11 => "deactivator",
            12 => "smallhealth",
            13 => "mediumhealth",
            14 => "largehealth",
            _ => "",
        };*/

        switch (whichPowerup)
        {
            case 0: return "machinegun";
            case 1: return "rocket";
            case 2: return "missile";
            case 3: return "blaster";
            case 4: return "crush";
            case 5: return "nuke";
            case 6: return "warp";
            case 7: return "plasma";
            case 8: return "mines";
            case 9: return "decoy";
            case 10: return "turret";
            case 11: return "deactivator";
            case 12: return "smallhealth";
            case 13: return "mediumhealth";
            case 14: return "largehealth";
            default: return "";
        }
    }

    public static int GetPowerupNumber(string whichPowerup)
    {
        /*return whichPowerup switch
        {
            "machinegun" => 0,
            "rocket" => 1,
            "missile" => 2,
            "blaster" => 3,
            "crush" => 4,
            "nuke" => 5,
            "warp" => 6,
            "plasma" => 7,
            "mines" => 8,
            "decoy" => 9,
            "turret" => 10,
            "deactivator" => 11,
            "smallhealth" => 12,
            "mediumhealth" => 13,
            "largehealth" => 14,
            _ => -1,
        };*/

        switch (whichPowerup)
        {
            case "machinegun": return 0;
            case "rocket": return 1;
            case "missile": return 2;
            case "blaster": return 3;
            case "crush": return 4;
            case "nuke": return 5;
            case "warp": return 6;
            case "plasma": return 7;
            case "mines": return 8;
            case "decoy": return 9;
            case "turret": return 10;
            case "deactivator": return 11;
            case "smallhealth": return 12;
            case "mediumhealth": return 13;
            case "largehealth": return 14;
            default: return -1;
        }
    }
}
