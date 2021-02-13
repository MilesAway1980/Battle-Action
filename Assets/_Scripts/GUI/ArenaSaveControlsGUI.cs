using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class ArenaSaveControlsGUI : MonoBehaviour
{
    public Button arenaEditButton;
    public Button shipEditButton;
    public ScrollRect fileNameArea;

    public List<CanvasRenderer> canvasesToHide;

    public Button saveButton;
    public Button loadButton;
    public TMP_InputField newFileName;

    ArenaEditGUI arenaEdit;
    Canvas saveControlCanvas;

    bool initialized = false;
    LoadArenaEditGUI loadArena;

    // Start is called before the first frame update
    void Start()
    {
        loadArena = FindObjectOfType<LoadArenaEditGUI>();
        saveButton.onClick.AddListener(SaveArena);
        loadButton.onClick.AddListener(LoadArena);
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform contentRect = fileNameArea.content.gameObject.GetComponent<RectTransform>();
        if (!initialized)
        {
            if (contentRect.rect.size.x == 0)
            {
                return;
            }
        }
        else
        {
            return;
        }

        initialized = true;

        arenaEdit = FindObjectOfType<ArenaEditGUI>();
        saveControlCanvas = GetComponent<Canvas>();
        saveControlCanvas.enabled = false;
        shipEditButton.onClick.AddListener(arenaEdit.HideArenaEdit);

        RefreshFileList();
    }

    public void RefreshFileList()
    {
        RectTransform contentRect = fileNameArea.content.gameObject.GetComponent<RectTransform>();

        DirectoryInfo directoryInfo = new DirectoryInfo(loadArena.arenaDataFolder);
        List<FileInfo> files = directoryInfo.GetFiles($"*.{loadArena.arenaFileExtension}", SearchOption.AllDirectories).ToList();

        List<string> fileNames = files.Select(f => f.Name).ToList();

        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, fileNames.Count * 15 + 30);

        TextMeshProUGUI[] currentTexts = fileNameArea.content.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < currentTexts.Length; i++)
        {
            Destroy(currentTexts[i].gameObject);
        }

        List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

        int fontSize = 12;
        int areaSize = 15;

        int count = 0;
        fileNames.ForEach(f =>
        {
            GameObject newText = new GameObject();

            newText.transform.parent = fileNameArea.content.transform;

            newText.AddComponent<SaveListText>();

            TextMeshProUGUI tmp_text = newText.AddComponent<TextMeshProUGUI>();

            Vector3 textPos = new Vector3(10, count++ * -areaSize - 10, 0);

            tmp_text.rectTransform.anchorMin = new Vector2(0, 1);
            tmp_text.rectTransform.anchorMax = new Vector2(0, 1);
            tmp_text.rectTransform.pivot = new Vector2(0, 1);

            tmp_text.gameObject.transform.localPosition = textPos;
            tmp_text.gameObject.transform.localScale = Vector2.one;

            tmp_text.fontSize = fontSize;
            tmp_text.text = Path.GetFileNameWithoutExtension(f);

            texts.Add(tmp_text);
        });
    }

    void SaveArena()
    {
        if (newFileName.text != "")
        {
            loadArena.SaveArena(newFileName.text);
        }
    }

    void LoadArena()
    {
        if (newFileName.text != "")
        {
            loadArena.LoadArena(newFileName.text);
        }
    }

    public void UpdateFileName(string fileName)
    {
        newFileName.text = fileName;
    }

    public void Show()
    {
        arenaEditButton.gameObject.SetActive(false);
        canvasesToHide.ForEach(c => c.gameObject.SetActive(false));
        saveControlCanvas.enabled = true;
    }

    public void Hide()
    {
        arenaEditButton.gameObject.SetActive(true);
        canvasesToHide.ForEach(c => c.gameObject.SetActive(true));
        saveControlCanvas.enabled = false;
    }
}
