using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO> THIS IS AN ANSWER CLASS
public class CubePalette : MonoBehaviour {
    //singleton
    public static CubePalette Main {
        get {
            if (!_loaded) {
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
    public int currentCubeID = 0;

    [SerializeField] private Transform content;

    private void Start() {
        ClearChildren(content);

        for (int i = 0; i < cubeList.cubes.Length; i++) {
            Instantiate(cubeSelectButton, content).Set(i);
        }
    }

    private static void ClearChildren(Transform o) {
        int n = o.childCount;
        if (n <= 0) return;
        for (int i = n - 1; i >= 0; i--) {
            GameObject.Destroy(o.GetChild(i).gameObject);
        }
    }
}
