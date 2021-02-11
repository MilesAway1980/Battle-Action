using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArenaEditGUI : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera shipCamera;
    public Camera shipCamera3D;
    public Camera arenaEditCamera;
    public Button arenaEditButton;

    ArenaSaveControlsGUI saveControls;

    void Start()
    {
        saveControls = FindObjectOfType<ArenaSaveControlsGUI>();
        GetComponent<LoadArenaEditGUI>().Load();
        arenaEditButton.onClick.AddListener(ShowArenaEdit);
        arenaEditCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowArenaEdit()
    {
        shipCamera.enabled = false;
        shipCamera3D.enabled = false;
        arenaEditCamera.enabled = true;

        saveControls.Show();        
    }

    public void HideArenaEdit()
    {
        shipCamera.enabled = true;
        shipCamera3D.enabled = true;
        arenaEditCamera.enabled = false;

        saveControls.Hide();
    }
}
