using UnityEngine;
using System.Linq;

public class ShipSelect : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject[] shipPrefabs;
    GameObject testShip;
    static GameObject shipPreview;
    GameObject shipContainer;

    //public Camera guiCamera;
    public static int shipSelect;
    int selectionChange = 0;

    string cheatString;
    static string charset = "abcdefghijklmnopqrstuvwxyz";
    bool test = false;

    void Start()
    {
        SpawnShips();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //shipSelect--;
            //shipSelect = shipSelect < 1 ? shipPrefabs.Length - 1 : shipSelect;            
            selectionChange = -1;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //shipSelect++;
            //shipSelect = shipSelect > shipPrefabs.Length ? 0 : shipSelect;
            selectionChange = 1;
        }

        if (test)
        {
            shipSelect = 1000;
            selectionChange = 1;
        }

        if (selectionChange != 0)
        {
            ChangeShip();
        }

        CheckCheat();
    }

    void SpawnShips()
    {
        Object[] shipObjects = Resources.LoadAll("Prefabs/Ships");
        shipPrefabs = new GameObject[shipObjects.Length];

        bool hasShip = false;
        for (int i = 0; i < shipObjects.Length; i++)
        {
            GameObject shipObject = (GameObject)shipObjects[i];
            //print(shipObject.activeSelf);

            if (shipObject.activeSelf)
            {
                if (shipObject.name != "Test_Ship")
                {
                    int.TryParse(shipObject.name.Split('_')[1], out int shipNum);
                    shipPrefabs[shipNum] = shipObject;
                    hasShip = true;
                }
                else
                {
                    testShip = shipObject;
                }
            }
        }

        if (!hasShip)
        {
            Debug.LogError("NO SHIPS ARE ACTIVE");
            Application.Quit();
        }

        shipSelect = Random.Range(0, shipPrefabs.Length - 1);

        shipContainer = new GameObject()
        {
            name = "Ship Container"
        };

        ChangeShip();
    }

    void ChangeShip()
    {
        if (shipPreview != null)
        {
            Destroy(shipPreview);
        }

        GameObject newShip = null;
        if (shipSelect != 1000)
        {
            while (newShip == null)
            {
                shipSelect += selectionChange;

                if (shipSelect < 0)
                {
                    shipSelect = shipPrefabs.Length - 1;
                }

                if (shipSelect > shipPrefabs.Length - 1)
                {
                    shipSelect = 0;
                }

                newShip = shipPrefabs[shipSelect];
            }
        }
        else
        {
            newShip = testShip;
        }

        shipPreview = Instantiate(
            newShip,
            Vector2.zero,
            Quaternion.Euler(90, 0, 0),
            shipContainer.transform
        );

        selectionChange = 0;
    }

    public static GameObject GetSelectedShip()
    {
        return shipPreview;
    }

    void CheckCheat()
    {
        if (Input.inputString != "")
        {
            char x = Input.inputString[0];
            if (charset.Contains(x))
            {
                cheatString += x.ToString().ToLower();
            }
            
            if (cheatString.Contains("test"))
            {
                test = true;
            }            
        }
    }
}
