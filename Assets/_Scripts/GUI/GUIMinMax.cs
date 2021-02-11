using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GUIMinMax : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text current;
    public TMP_Text slash;
    public TMP_Text max;

    public string template = "";

    public float displayMod = 1;

    void Start()
    {
        List<TMP_Text> texts = GetComponentsInChildren<TMP_Text>().ToList();
        
        texts.ForEach(t =>
        {


            switch (t.name.ToLower())
            {
                case "title": title = t; break;
                case "current": current = t; break;
                case "slash": slash = t; break;
                case "max": max = t; break;
            }

        });
    }
}
