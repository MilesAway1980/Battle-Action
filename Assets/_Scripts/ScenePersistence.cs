using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ScenePersistence : MonoBehaviour
{

    static ScenePersistence self;

    /*
    public Dictionary<int, string> userName;
    public Dictionary<int, string> IPAddress;
    public Dictionary<int, string> gameRoomName;
    public Dictionary<int, string> gameRoomPassword;

    public Dictionary<int, int> networkType;
    public Dictionary<int, int> shipSelection;
    */

    public string userName;
    public string IPAddress;
    public string gameRoomName;
    public string gameRoomPassword;

    int networkType;
    public int shipSelection;

    int arenaSize;
    int numberOfBeacons;
    int maxPowerups;
    int startingPowerups;
    float powerupSpawnRate;
    int beaconRadarRange;
    int shipRadarRange;
    int maxPlayers;

    List<WeaponData> weaponData;

    public string playerGuidString;
    Guid playerGuid;

    //static int userID;
    //int thisUserID;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (self == null)
        {
            DontDestroyOnLoad(gameObject);
            self = this;
        }
        else if (self != this)
        {
            Destroy(gameObject);
        }

        playerGuid = Guid.NewGuid();
        playerGuidString = playerGuid.ToString();
    }

    public static string GetUserName() {
        return self.userName;
    }

    public static void SetUserName(string userName)
    {
        self.userName = userName;
    }

    public static string GetIPAddress()
    {
        return self.IPAddress;
    }

    public static void SetIPAddress(string ipAddress)
    {
        self.IPAddress = ipAddress;
    }

    public static string GetGameRoomName()
    {
        return self.gameRoomName;
    }

    public static void SetGameRoomName(string gameRoomName)
    {
        self.gameRoomName = gameRoomName;
    }

    public static string GetGameRoomPassword()
    {
        return self.gameRoomPassword;
    }

    public static void SetGameRoomPassword(string gameRoomPassword)
    {
        self.gameRoomPassword = gameRoomPassword;
    }

    public static int GetNetworkType()
    {
        return self.networkType;
    }

    public static void SetNetworkType(int networkType)
    {
        self.networkType = networkType;
    }

    public static int GetShipSelection()
    {
        return self.shipSelection;
    }

    public static void SetShipSelection(int shipSelection)
    {
        self.shipSelection = shipSelection;
    }

    public static int GetArenaSize()
    {
        return self.arenaSize;
    }
    public static void SetArenaSize(int arenaSize)
    {
        self.arenaSize = arenaSize;
    }

    public static int GetNumberOfBeacons()
    {
        return self.numberOfBeacons;
    }

    public static void SetNumberOfBeacons(int numberOfBeacons)
    {
        self.numberOfBeacons = numberOfBeacons;
    }

    public static int GetMaxPowerups()
    {
        return self.maxPowerups;
    }

    public static void SetMaxPowerups(int maxPowerups)
    {
        self.maxPowerups = maxPowerups;
    }

    public static int GetStartingPowerups()
    {
        return self.startingPowerups;
    }

    public static void SetStartingPowerups(int startingPowerups)
    {
        self.startingPowerups = startingPowerups;
    }

    public static float GetPowerupSpawnRate()
    {
        return self.powerupSpawnRate;
    }

    public static void SetPowerupSpawnRate(float powerupSpawnRate)
    {
        self.powerupSpawnRate = powerupSpawnRate;
    }

    public static int GetBeaconRadarRange()
    {
        return self.beaconRadarRange;
    }

    public static void SetBeaconRadarRange(int beaconRadarRange)
    {
        self.beaconRadarRange = beaconRadarRange;
    }

    public static int GetShipRadarRange()
    {
        return self.shipRadarRange;
    }

    public static void SetShipRadarRange(int shipRadarRange)
    {
        self.shipRadarRange = shipRadarRange;
    }

    public static int GetMaxPlayers()
    {
        return self.maxPlayers;
    }

    public static void SetMaxPlayers(int maxPlayers)
    {
        self.maxPlayers = maxPlayers;
    }

    public static List<WeaponData> GetWeaponData()
    {
        return self.weaponData;
    }

    public static void SetWeaponData(List<WeaponData> weaponData)
    {
        self.weaponData = weaponData;
    }

    public static Guid GetPlayerGuid()
    {
        return self.playerGuid;
    }
}
