using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "CubeList", menuName = "Cube List", order = 0)]
public class CubeList : ScriptableObject {
    private const string PREVIEWS_PATH = "Assets/Generated/CubePreviews";
    
    public GameObject[] cubes = { };
    public List<Sprite> Sprites;

    public static string PreviewPath(GameObject cube)
    {
        return Path.Combine(PREVIEWS_PATH, cube.name + ".png");
    }
}
