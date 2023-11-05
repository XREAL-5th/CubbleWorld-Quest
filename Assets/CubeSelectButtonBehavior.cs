using TMPro;
using UnityEngine;

public class CubeSelectButtonBehavior : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;

    private GameObject cube;

    public void SetCube(GameObject cube)
    {
        Debug.Log("set");
        nameText.text = cube.name;
        this.cube = cube;
    }

    public void SelectCube()
    {
        Debug.Log(cube);
        CubeManager.Instance.SelectedCube = cube;
    }
}
