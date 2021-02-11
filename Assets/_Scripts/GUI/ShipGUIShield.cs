using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipGUIShield : MonoBehaviour
{
    public TMP_Text absorption;
    public TMP_Text battery;
    public TMP_Text rechargeRate;
    public TMP_Text rammingDamage;
    

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
            

            if (ship)
            {
                Shield shield = ship.GetComponent<Shield>();

                if (shield)
                {
                    absorption.text = initialString[absorption] + ": " + shield.damageReduction;
                    battery.text = initialString[battery] + ": " + shield.maxCharge;
                    rechargeRate.text = initialString[rechargeRate] + ": " + shield.rechargeRate;
                    rammingDamage.text = initialString[rammingDamage] + ": " + shield.shieldContactDamage;
                }
            }
        }
    }
}
