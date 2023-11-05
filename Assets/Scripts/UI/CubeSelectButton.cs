using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO> THIS IS AN ANSWER CLASS
public class CubeSelectButton : MonoBehaviour {
    [SerializeField] private Color defColor = Color.white, activeColor = Color.yellow;

    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Graphic bg;
    [SerializeField] private Button button;

    private int cubeID;

    private void Start() {
        button.onClick.AddListener(Clicked);
    }

    private void Update() {
        bg.color = CubePalette.Main.currentCubeID == cubeID ? activeColor : defColor;
    }

    public void Set(int cubeID) {
        nameLabel.text = CubePalette.Main.cubeList.cubes[cubeID].name;
        this.cubeID = cubeID;
    }

    private void Clicked() {
        CubePalette.Main.currentCubeID = cubeID;
    }
}
