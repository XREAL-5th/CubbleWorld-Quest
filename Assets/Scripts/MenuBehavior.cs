using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehavior : MonoBehaviour
{
    [SerializeField] private CubeList cubeList;
    [SerializeField] private CubeSelectButtonBehavior cubeSelectButton;

    void Start()
    {
        for (var i = 0; i < cubeList.cubes.Length; i++)
        {
            var cube = cubeList.cubes[i];
            var sprite = cubeList.Sprites[i];
            Instantiate(cubeSelectButton, transform).SetCube(cube, sprite);
        }
    }
}