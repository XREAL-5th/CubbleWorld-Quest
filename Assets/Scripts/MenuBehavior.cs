using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehavior : MonoBehaviour
{
    [SerializeField] private CubeList cubeList;
    [SerializeField] private CubeSelectButtonBehavior cubeSelectButton;

    void Awake()
    {
        foreach (var cube in cubeList.cubes)
        {
            Instantiate(cubeSelectButton, transform).SetCube(cube);
        }
    }
}