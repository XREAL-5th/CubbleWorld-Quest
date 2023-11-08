using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CubeList", menuName = "Cube List", order = 0)]
public class CubeList : ScriptableObject {
    public GameObject[] cubes = { };
    public Sprite[] cubePreviews = { };
}
