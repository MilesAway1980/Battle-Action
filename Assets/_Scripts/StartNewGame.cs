using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StartNewGame : MonoBehaviour
{
    // Start is called before the first frame update
    NetworkManager manager;
    bool initialized = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            return;
        }

        manager = FindObjectOfType<NetworkManager>();
        if (manager == null)
        {
            Debug.LogWarning("No Network Manager Found");
            return;
        }

        initialized = true;

        if (!NetworkClient.isConnected && !NetworkClient.active)
        {
            string ipAddress = ScenePersistence.GetIPAddress();
            if (ipAddress == "")
            {
                ipAddress = "LocalHost";
            }
            manager.networkAddress = ipAddress;

            switch (ScenePersistence.GetNetworkType())
            {
                case 0: manager.StartHost(); break;      //Host
                case 1: manager.StartClient(); break;      //Client
                case 2:
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        // cant be a server in webgl build
                        Debug.LogError("(  WebGL cannot be server  )");
                        return;
                    }
                    else
                    {
                        manager.StartServer();
                    }

                    break;      //Server
            }
        }
    }
}
