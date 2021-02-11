using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class ShipGUINetwork : MonoBehaviour
{

    public TMP_InputField userName;
    public TMP_InputField IPAddress;
    public TMP_InputField gameRoomName;
    public TMP_InputField gameRoomPassword;
    public TMP_Dropdown networkType;
    public Button startGame;

    //int persistenceUserId;

    // Start is called before the first frame update
    void Start()
    {
        //persistenceUserId = ScenePersistence.AssignUserID();
        startGame.onClick.AddListener(StartNewGame);
    }

    public void SetUserName(string name)
    {
        userName.text = name;
    }

    public string GetUserName()
    {
        return userName.text;
    }

    public void SetIPAddress(string address)
    {
        IPAddress.text = address;
    }

    public string GetIPAddress()
    {
        return IPAddress.text;
    }

    public void SetRoomName(string room)
    {
        gameRoomName.text = room;
    }

    public string GetRoomName()
    {
        return gameRoomName.text;
    }

    public void SetRoomPassword(string password)
    {
        gameRoomPassword.text = password;
    }

    public string GetRoomPassword()
    {
        return gameRoomPassword.text;
    }

    public void SetNetworkType(int type)
    {
        //networkType.value = networkType;        
        networkType.SetValueWithoutNotify(type);
    }

    public int GetNetworkType()
    {
        return networkType.value;
    }

    void StartNewGame()
    {

        //ScenePersistence scene = ScenePersistence.instance;
        FindObjectOfType<LoadArenaEditGUI>().SaveUser("justin");

        ScenePersistence.SetUserName(userName.text);
        ScenePersistence.SetIPAddress(IPAddress.text);
        ScenePersistence.SetGameRoomName(gameRoomName.text);
        ScenePersistence.SetGameRoomPassword(gameRoomPassword.text);
        ScenePersistence.SetNetworkType(networkType.value);
        ScenePersistence.SetShipSelection(ShipSelect.shipSelect);

        GUIInputField[] fields = FindObjectsOfType<GUIInputField>();
        for (int i = 0; i < fields.Length; i++)
        {
            GUIInputField field = fields[i];

            if (float.TryParse(field.GetValue(), out float value))
            {
                switch (field.loadFileName.Trim().ToLower())
                {
                    case "arenasize": ScenePersistence.SetArenaSize((int)value); break;
                    case "numbeacons": ScenePersistence.SetNumberOfBeacons((int)value); break;
                    case "maxpowerups": ScenePersistence.SetMaxPowerups((int)value); break;
                    case "startingpowerups": ScenePersistence.SetStartingPowerups((int)value); break;
                    case "powerupspawnrate": ScenePersistence.SetPowerupSpawnRate(value); break;
                    case "beaconradarrange": ScenePersistence.SetBeaconRadarRange((int)value); break;
                    case "shipradarrange": ScenePersistence.SetShipRadarRange((int)value); break;
                    case "maxplayers": ScenePersistence.SetMaxPlayers((int)value); break;
                }
            }
        }

        GUIAmmoSetup[] setups = FindObjectsOfType<GUIAmmoSetup>();
        List<WeaponData> weaponData = new List<WeaponData>();
        for (int i = 0; i < setups.Length; i++)
        {
            GUIAmmoSetup setup = setups[i];

            WeaponData weapon = new WeaponData();
            weapon.name = setup.loadFileName;

            weapon.amountCollected = setup.GetAmountCollected();
            weapon.amountCollectedType = setup.ammoCollectType.value;

            weapon.startingAmmo = setup.GetStartingAmmo();
            weapon.startingAmmoType = setup.startingAmountType.value;

            weapon.maxAmmo = setup.GetMaximumAmmo();

            weapon.rarity = setup.GetRarity();

            weaponData.Add(weapon);
            
        }

        ScenePersistence.SetWeaponData(weaponData);

        /*
        print(ScenePersistence.GetArenaSize());
        print(ScenePersistence.GetNumberOfBeacons());
        print(ScenePersistence.GetMaxPlayers());
        print(ScenePersistence.GetMaxPowerups());
        print(ScenePersistence.GetStartingPowerups());
        print(ScenePersistence.GetPowerupSpawnRate());
        print(ScenePersistence.GetBeaconRadarRange());
        print(ScenePersistence.GetShipRadarRange());
        print(ScenePersistence.GetWeaponData());
        */

        SceneManager.LoadScene("Game");
        
    }
}
