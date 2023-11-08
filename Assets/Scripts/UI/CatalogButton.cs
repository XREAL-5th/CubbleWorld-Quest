using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class CatalogButton : MonoBehaviour
{
    private Button button;
    private int cubeID;
    
    void Start()
    {
        button = GetComponent<Button>();
    }

    public void InitSetting(int currentIndex)
    {
        cubeID = currentIndex;
        if(button == null)
            button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        SelectedCube.Cube = cubeID;
    }
}
