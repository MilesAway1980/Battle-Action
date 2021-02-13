using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadArenaEditGUI : MonoBehaviour
{
    string currentDirectory;
    public string arenaDataFolder = "SaveData/Arenas";
    public string userDataFolder = "SaveData/Users";
    public string userFileExtension = "usr";
    public string arenaFileExtension = "arn";

    void Awake()
    {
        currentDirectory = Directory.GetCurrentDirectory();

        arenaDataFolder = currentDirectory + "/" + arenaDataFolder;
        userDataFolder = currentDirectory + "/" + userDataFolder;

        if (!Directory.Exists(arenaDataFolder))
        {
            Directory.CreateDirectory(arenaDataFolder);
        }

        if (!Directory.Exists(userDataFolder))
        {
            Directory.CreateDirectory(userDataFolder);
        }
    }

    public void Load()
    {
        LoadUser();
        LoadArena();
    }

    void LoadUser()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(userDataFolder);
        FileInfo[] files = directoryInfo.GetFiles($"*.{userFileExtension}", SearchOption.AllDirectories);

        if (files.Length > 0)
        {
            FileInfo first = files[0];

            string[] lines = File.ReadAllLines(first.FullName);
            if (lines.Length == 1)
            {
                string[] data = lines[0].Split(',');
                if (data.Length == 5)
                {

                    int netType;
                    if (int.TryParse(data[2], out netType))
                    {

                        ShipGUINetwork gui = FindObjectOfType<ShipGUINetwork>();
                        gui.SetUserName(data[0]);
                        gui.SetIPAddress(data[1]);
                        gui.SetNetworkType(netType);
                        gui.SetRoomName(data[3]);
                        gui.SetRoomPassword(data[4]);
                    }
                }
            }
        }
    }

    public void SaveUser(string fileName)
    {
        fileName = "justin";
        string filePath = $"{userDataFolder}/{fileName}.{userFileExtension}";

        ShipGUINetwork gui = FindObjectOfType<ShipGUINetwork>();

        string data = "";
        data += $"{gui.GetUserName()},";
        data += $"{gui.GetIPAddress()},";
        data += $"{gui.GetNetworkType()},";
        data += $"{gui.GetRoomName()},";
        data += $"{gui.GetRoomPassword()}";

        using (StreamWriter file = new StreamWriter(filePath))
        {
            file.WriteLine(data);
        }
    }

    public void LoadArena(string fileName = null)
    {
        if (fileName == null)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(arenaDataFolder);
            FileInfo[] files = directoryInfo.GetFiles($"*.{arenaFileExtension}", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                fileName = files[0].FullName;
            }
        }
        else
        {
            if (fileName == "")
            {
                return;
            }

            fileName = $"{arenaDataFolder}/{fileName}.{arenaFileExtension}";
        }

        GUIInputField[] fields = FindObjectsOfType<GUIInputField>();
        GUIAmmoSetup[] setups = FindObjectsOfType<GUIAmmoSetup>();

        if (fileName != null)
        {
            string[] lines = File.ReadAllLines(fileName);

            for (int l = 0; l < lines.Length; l++)
            {
                if (!lines[l].Trim().StartsWith("#") && lines[l].Trim().Length > 0)     //Skip lines that start with a # or are empty
                {
                    string[] data = lines[l].Split('-');
                    if (data.Length == 2)
                    {
                        string name = data[0].Trim().ToLower();
                        string[] ammoData = data[1].Split(',');

                        if (ammoData.Length == 1)   //ArenaSize, NumBeacons, etc. don't have a comma
                        {
                            for (int f = 0; f < fields.Length; f++)
                            {
                                if (fields[f].loadFileName.Trim().ToLower() == name)
                                {
                                    fields[f].SetValue(data[1].Trim());
                                }
                            }
                        }
                        else  //It's a powerup  ie.  MachineGun - 100,1,100,2,100,0
                        {
                            for (int s = 0; s < setups.Length; s++)
                            {
                                if (setups[s].loadFileName.Trim().ToLower() == name)
                                {
                                    int valueInt;
                                    float valueFloat;

                                    if (int.TryParse(ammoData[0], out valueInt))
                                    {
                                        setups[s].SetMaximumAmmo(valueInt);
                                    }

                                    if (int.TryParse(ammoData[1], out valueInt))
                                    {
                                        setups[s].SetStartingAmmoType(valueInt);
                                    }

                                    if (float.TryParse(ammoData[2], out valueFloat))
                                    {
                                        setups[s].SetStartingAmmo(valueFloat);
                                    }

                                    if (int.TryParse(ammoData[3], out valueInt))
                                    {
                                        setups[s].SetAmountCollectedType(valueInt);
                                    }

                                    if (float.TryParse(ammoData[4], out valueFloat))
                                    {
                                        setups[s].SetAmountCollected(valueFloat);
                                    }

                                    if (int.TryParse(ammoData[5], out valueInt))
                                    {
                                        setups[s].SetRarity(valueInt);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void SaveArena(string fileName)
    {
        if (fileName == "")
        {
            return;
        }

        List<string> allLines = new List<string>();

        GUIInputField[] fields = FindObjectsOfType<GUIInputField>();
        GUIAmmoSetup[] setups = FindObjectsOfType<GUIAmmoSetup>();

        for (int i = 0; i < fields.Length; i++)
        {
            GUIInputField field = fields[i];
            //print();
            allLines.Add(field.loadFileName + " - " + field.GetValue().Trim());
        }

        for (int i = 0; i < setups.Length; i++)
        {
            GUIAmmoSetup setup = setups[i];
            allLines.Add(setup.loadFileName + " - " + setup.GetMaximumAmmo() + "," + setup.GetStartingAmmoType() + "," + setup.GetStartingAmmo() + "," + setup.GetAmountCollectedType() + "," + setup.GetAmountCollected() + "," + setup.GetRarityValue());
        }

        string filePath = $"{arenaDataFolder}/{fileName}.{arenaFileExtension}";

        using (StreamWriter file = new StreamWriter(filePath))
        {
            allLines.ForEach(l => file.WriteLine(l));
        }

        ArenaSaveControlsGUI arenaControl = FindObjectOfType<ArenaSaveControlsGUI>();
        arenaControl.RefreshFileList();
    }
}
