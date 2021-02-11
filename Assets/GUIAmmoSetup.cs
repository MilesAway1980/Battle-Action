using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class GUIAmmoSetup : MonoBehaviour
{
    public SpriteRenderer icon;
    public TMP_Text weaponName;
    public TMP_Dropdown ammoCollectType;
    public TMP_InputField ammoCollected;
    public TMP_Dropdown startingAmountType;
    public TMP_InputField startingAmountValue;
    public TMP_InputField baseMaxAmmo;
    public TMP_Dropdown rarity;

    public string loadFileName;

    static List<GUIAmmoSetup> allAmmoSetups = new List<GUIAmmoSetup>();

    public static float rarityPower = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        allAmmoSetups.Add(this);
    }

    void OnDestroy()
    {
        allAmmoSetups.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        int maxAmmo;
        if (!int.TryParse(baseMaxAmmo.text, out maxAmmo) || maxAmmo < 0)
        {
            baseMaxAmmo.GetComponent<Image>().color = new Color32(200, 100, 100, 255);
        }
        else
        {
            baseMaxAmmo.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        /*
         * Starting amount
         * 
         * 0 - Empty
         * 1 - Full
         * 2 - Amount
         * 3 - Percent
         * 
         */

        if (startingAmountType.value == 0 || startingAmountType.value == 1)
        {
            startingAmountValue.enabled = false;
            startingAmountValue.GetComponent<Image>().color = new Color32(128, 128, 128, 255);
        }
        else
        {
            startingAmountValue.enabled = true;

            float startingAmount;
            if (
                    (!float.TryParse(startingAmountValue.text, out startingAmount)) ||                  //Invalid
                    (startingAmountType.value == 2 && startingAmount < 0) ||                            //Value less than 0
                    (startingAmountType.value == 3 && (startingAmount < 0 || startingAmount > 100))     //Percent not between 0 and 100
            )
            {
                startingAmountValue.GetComponent<Image>().color = new Color32(200, 100, 100, 255);
            }
            else
            {
                if (startingAmountType.value == 2)
                {
                    if (startingAmount > maxAmmo)
                    {
                        startingAmountValue.text = maxAmmo.ToString();
                    }
                }
                startingAmountValue.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }

        if (ammoCollectType.value == 2)
        {
            ammoCollected.enabled = false;
            ammoCollected.GetComponent<Image>().color = new Color32(128, 128, 128, 255);
        }
        else
        {
            ammoCollected.enabled = true;

            float collectAmount;
            if (
                (!float.TryParse(ammoCollected.text, out collectAmount)) ||                     //Invalid
                (ammoCollectType.value == 0 && collectAmount < 0) ||                            //Value less than 0
                (ammoCollectType.value == 1 && (collectAmount < 0 || collectAmount > 100))      //Percent not between 0 and 100

            )
            {
                ammoCollected.GetComponent<Image>().color = new Color32(200, 100, 100, 255);
            }
            else
            {
                ammoCollected.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    public void SetIcon(Sprite newIcon)
    {
        icon.sprite = newIcon;
    }

    public int GetMaximumAmmo()
    {
        int maxAmmo;
        if (!int.TryParse(baseMaxAmmo.text, out maxAmmo))
        {
            return 0;
        }
        else
        {
            return maxAmmo;
        }
    }

    public float GetStartingAmmo()
    {
        float startingAmmo;
        if (!float.TryParse(startingAmountValue.text, out startingAmmo))
        {
            return 0;
        }
        else
        {
            return startingAmmo;
        }
    }

    public int GetStartingAmmoType()
    {
        return startingAmountType.value;
    }

    public float GetAmountCollected()
    {
        float collected;
        if (!float.TryParse(ammoCollected.text, out collected))
        {
            return 0;
        }
        else
        {
            return collected;
        }
    }

    public int GetAmountCollectedType()
    {
        return ammoCollectType.value;
    }

    public int GetRarityValue()
    {
        return rarity.value;
    }

    public float GetRarity()
    {

        /*                      points
        * 0 = abundant          5
        * 1 = plentiful         4
        * 2 = common            3
        * 3 = scarce            2
        * 4 = rare              1
        * 5 = disabled          0    
        */


        float rarityTotal = 0;
        int val = 0;

        allAmmoSetups.ForEach(
            a =>
            {
                val = rarity.options.Count - a.GetRarityValue() - 1;
                rarityTotal += Mathf.Pow(val, rarityPower);
            }
        );

        val = rarity.options.Count - GetRarityValue() - 1;
        return Mathf.Pow(val, rarityPower) / rarityTotal;
    }

    public void SetStartingAmmo(float value)
    {
        switch (startingAmountType.value)
        {
            case 3:
                if (value >= 0 && value <= 100)
                {
                    startingAmountValue.text = value.ToString();
                }
                break;

            default:
                if (value >= 0)
                {
                    startingAmountValue.text = value.ToString();
                }
                break;
        }
    }

    public void SetStartingAmmoType(int value)
    {
        if (value >= 0 && value < startingAmountType.options.Count)
        {
            startingAmountType.value = value;
        }
    }

    public void SetMaximumAmmo(int value)
    {
        if (value >= 0)
        {
            baseMaxAmmo.text = value.ToString();
        }
    }

    public void SetAmountCollected(float value)
    {
        switch (ammoCollectType.value)
        {
            case 1:
                if (value >= 0 && value <= 100)
                {
                    ammoCollected.text = value.ToString();
                }
                break;

            default:
                if (value >= 0)
                {
                    ammoCollected.text = value.ToString();
                }
                break;
        }
    }

    public void SetAmountCollectedType(int value)
    {
        if (value >= 0 && value < ammoCollectType.options.Count)
        {
            ammoCollectType.value = value;
        }
    }

    public void SetRarity(int value)
    {
        if (value >= 0 && value < rarity.options.Count)
        {
            rarity.value = value;
        }
    }
}
