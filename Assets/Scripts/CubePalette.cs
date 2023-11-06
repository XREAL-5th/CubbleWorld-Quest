using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePalette : MonoBehaviour
{
    // singleton
    public static CubePalette Main 
    {
        get
        {
            if (!_loaded)
            {
                _loaded = true;
                _main = FindObjectOfType<CubePalette>();
            }
            return _main;
        }
    }

    private static CubePalette _main;
    private static bool _loaded;

    public CubeList cubeList;
    public CubeSelectButton cubeSelectButton;
    public int currentCubeID = 0; // currently selected cube

    [SerializeField] private Transform content;

    private void Start()
    {
        ClearChildren(content);

        // Instantiate CubeSelectButton objects for each cube in 'cubeList'
        for (int i = 0; i < cubeList.cubes.Length; i++) 
        {
            Instantiate(cubeSelectButton, content).Set(i); 
        }
    }

    private static void ClearChildren(Transform o) // clear all child objects (given transform o)
    {
        int n = o.childCount;
        if (n <= 0) return;
        for (int i = n - 1; i >= 0; i--)
        {
            GameObject.Destroy(o.GetChild(i).gameObject);
        }
    }
}
