using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Catalog : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject windowPrefab;
    [SerializeField] private CubeList cubeList;

    void Start()
    {
        Set();
    }

    private void Set()
    {
        int cubeListLength = cubeList.cubes.Length;
        for (int count = 0; count < cubeListLength; ++count)
        {
            GameObject window = Instantiate(windowPrefab, transform);
            window.AddComponent<CatalogButton>().InitSetting(count);
            TextMeshProUGUI catalogName = window.GetComponentInChildren<TextMeshProUGUI>();
            catalogName.text = cubeList.cubes[count].name;
        }
    }
}
