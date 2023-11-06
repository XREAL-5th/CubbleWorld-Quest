using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubeSelectButtonBehavior : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image cubeImage;

    private GameObject cube;

    public void SetCube(GameObject cube, Sprite sprite)
    {
        nameText.text = cube.name;
        Debug.Log(sprite);
        cubeImage.sprite = sprite;
        this.cube = cube;
    }
    
    public void SelectCube()
    {
        CubeManager.Instance.SelectedCube = cube;
    }
}
