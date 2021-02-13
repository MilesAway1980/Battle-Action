using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class InitGameWorld : NetworkBehaviour
{

    public bool test = true;
    public int testShips = 1;
    GameObject[] testShip;
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
        if (test)
        {
            testShip = new GameObject[testShips];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && test)
        {
            for (int i = 0; i < testShips; i++)
            {
                if (testShip[i] == null)
                {
                    testShip[i] = PlayerShipSpawner.SpawnShip(Guid.NewGuid(), UnityEngine.Random.Range(0, 25)).gameObject;
                    testShip[i].transform.position = ArenaInfo.GetRandomArenaLocation();
                }
            }            
        }
    }
}
