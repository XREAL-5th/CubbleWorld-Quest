using UnityEngine;

public class ToggleGrid : MonoBehaviour {
    [SerializeField] private MeshRenderer mrenderer;
    [SerializeField] private Material[] mats = { };
    private int id = 0;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) Toggle();
    }

    private void Toggle() {
        id++;
        id %= mats.Length;
        mrenderer.sharedMaterial = mats[id];
    }
}
