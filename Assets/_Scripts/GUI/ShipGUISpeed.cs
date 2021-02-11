using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipGUISpeed : MonoBehaviour
{

    public TMP_Text maxSpeed;
    public TMP_Text maxThrust;
    public TMP_Text thrustChange;
    public TMP_Text turnRate;
    public TMP_Text turnChange;
    public TMP_Text drag;

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
            Rigidbody2D rb = currentShip.GetComponent<Rigidbody2D>();

            if (ship)
            {
                maxSpeed.text = initialString[maxSpeed] + ": " + ship.maxSpeed;
                maxThrust.text = initialString[maxThrust] + ": " + ship.maxThrust;
                turnRate.text = initialString[turnRate] + ": " + ship.maxTurnRate;

                turnChange.text = initialString[turnChange] + ": " + ship.turnRateAcceleration.ToString("0.00") + " / " + ship.turnRateDeceleration.ToString("0.00");
                thrustChange.text = initialString[thrustChange] + ": " + ship.acceleration.ToString("0.00") + " / " + ship.deceleration.ToString("0.00");
            }

            if (rb)
            {
                drag.text = initialString[drag] + ": " + rb.drag.ToString();
            }
        }
    }
}