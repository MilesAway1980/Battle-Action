using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManagerGUI : MonoBehaviour
{
    Player player;
    NetworkManagerHUD managerHUD;

    public bool hideNetworkManagerGUIOnPlayerStart = false;


    // Start is called before the first frame update
    void Start()
    {
        NetworkManager manager = FindObjectOfType<NetworkManager>();
        managerHUD = manager.gameObject.GetComponent<NetworkManagerHUD>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.GetLocalPlayer() == null)
        {
            managerHUD.showGUI = true;
        }
        else
        {
            managerHUD.showGUI = !hideNetworkManagerGUIOnPlayerStart;
        }
    }
}
