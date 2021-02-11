using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GUIInputField : MonoBehaviour
{
    public TMP_Text title;
    public TMP_InputField input;
    public string loadFileName;
    public bool isFloat;


    // Start is called before the first frame update
    void Start()
    {
        input.placeholder.GetComponent<TextMeshProUGUI>().text = title.text;
    }

    // Update is called once per frame
    void Update()
    {
        int valueInt;
        float valueFloat;
        bool valid = false;

        if (isFloat)
        {
            valid = (float.TryParse(input.text, out valueFloat) && valueFloat >= 0);
        }
        else
        {
            valid = (int.TryParse(input.text, out valueInt) && valueInt >= 0);
        }

        
        if (!valid)
        {
            input.GetComponent<Image>().color = new Color32(200, 100, 100, 255);
        }
        else
        {
            input.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    public string GetValue()
    {
        return input.text;
    }

    public void SetValue(string value)
    {
        input.text = value;
    }
}
