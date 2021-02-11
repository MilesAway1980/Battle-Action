using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SaveListText : MonoBehaviour, IPointerClickHandler
{
    TMP_Text text;
    static ArenaSaveControlsGUI arenaSave;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        if (!arenaSave)
        {
            arenaSave = FindObjectOfType<ArenaSaveControlsGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData ped)
    {
        arenaSave.UpdateFileName(text.text);
    }
}
