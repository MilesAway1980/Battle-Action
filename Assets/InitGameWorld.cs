using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class InitGameWorld : NetworkBehaviour
{

    public bool test = true;
    GameObject testShip;
    // Start is called before the first frame update
    void Awake()
    {
        UnityEngine.Object[] shipObjects = Resources.LoadAll("Prefabs/Ships");
        for (int i = 0; i < shipObjects.Length; i++)
        {
            ClientScene.RegisterPrefab((GameObject)shipObjects[i]);            
        }
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && test)
        {
            if (testShip == null)
            {
                testShip = PlayerShipSpawner.SpawnShip(Guid.NewGuid(), 20).gameObject;
            }
        }
    }
}
