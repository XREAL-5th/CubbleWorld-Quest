using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class SelectedCube
{
    private static int selecetedCubeIndex;

    public static int Cube {get { return selecetedCubeIndex; } set { selecetedCubeIndex = value; } } 
}
