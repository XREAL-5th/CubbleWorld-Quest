using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubeSelectButton : MonoBehaviour
{
    [SerializeField] private Color defColor = Color.white, activeColor = Color.yellow;

    [SerializeField] private TextMeshProUGUI nameLabel; // TextMeshProUGUI component to display the cube's name
    //[SerializeField] private Sprite SpriteIcon; // cube preview
    [SerializeField] public RawImage icon; // cube preview // why RawImage?  
    [SerializeField] private Graphic bg; // background
    [SerializeField] private Button button; // cube selected button


    private int cubeID;

    private void Start()
    {
        button.onClick.AddListener(Clicked); // Add event listener to the button to call Clicked()
    }

    private void Update()
    {
        bg.color = CubePalette.Main.currentCubeID == cubeID ? activeColor : defColor;
    }

    public void Set(int cubeID)
    {
        nameLabel.text = CubePalette.Main.cubeList.cubes[cubeID].name;
        icon.texture = CubePalette.Main.cubeList.cubePreviews[cubeID].texture; // add cube Icon  
        this.cubeID = cubeID;
    }

    private void Clicked() // After click
    {
        // CubePalette.Main.currentCubeID 값 = 버튼이 나타내는 큐브의 cubeID
        CubePalette.Main.currentCubeID = cubeID; 
    }
}