using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipGUIStats : MonoBehaviour
{

    public TMP_Text shipName;
    public TMP_Text armorValue;
    public TMP_Text mass;
    public TMP_Text ammoCapacity;

    Dictionary<TMP_Text, string> initialString = new Dictionary<TMP_Text, string>();

    // Start is called before the first frame update
    void Start()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        
        for (int i = 0; i < texts.Length; i++)
        {
            string initial = texts[i].text.Split(':')[0];
            initialString.Add(texts[i], initial);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject currentShip = ShipSelect.GetSelectedShip();
        if (currentShip != null)
        {
            Ship ship = currentShip.GetComponent<Ship>();
            Damageable dm = ship.GetComponent<Damageable>();
            Rigidbody2D rb = currentShip.GetComponent<Rigidbody2D>();
            //armorValue.text = dm.maxArmor.ToString();

            if (dm)
            {
                armorValue.text = initialString[armorValue] + ": " + dm.maxArmor.ToString();
                ammoCapacity.text = initialString[ammoCapacity] + ": " + ship.GetAmmoBoost().ToString("000%");
            }

            shipName.text = GetShipName();

            if (rb)
            {
                mass.text = initialString[mass] + ": " + rb.mass.ToString();
            }
        }
        else
        {
            shipName.text = "NO SHIP";
        }
    }

    string GetShipName()
    {
        switch (ShipSelect.shipSelect)
        {
            case 1: return "Astra Shuttle";
            case 2: return "Omega Fighter";
            case 3: return "Green Death";
            case 4: return "Agent Orange";
            case 5: return "Nave";
            case 6: return "Star Blaster";
            case 7: return "UAV Trident";
            case 8: return "Beetle";
            case 9: return "Renegade";
            case 10: return "Andromedus Spore";
            case 11: return "Nebulous Stingray";
            case 12: return "NS-2";
            case 13: return "Outer Rim Contender";
            case 14: return "Voyager";
            case 15: return "Rapscillistar";
            case 16: return "Radogost";
            case 17: return "Stribog";
            case 18: return "Velez";
            case 19: return "Quadrant Master";
            case 20: return "Avalon";
            case 21: return "Bulgyar Ferroso";
            case 22: return "Renold";
            case 23: return "Soul of Phillon";
            case 24: return "Mirana Cargo Buddy";
            case 25: return "Battle Bug";
            case 26: return "Varl Wing";
        }

        return "";
    }
}
