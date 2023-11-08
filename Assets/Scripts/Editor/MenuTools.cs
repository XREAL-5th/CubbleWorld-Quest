using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System;

public static class MenuTools {
    private const string PREVIEWS_PATH = "Assets/Generated/CubePreviews";
    private const string CUBELIST_PATH = "Assets/Scripts/Cubes/CubeList.asset";

    private const string PREVIEWS_SCENE = "Assets/Scenes/CubePreviewScene.unity";
    private const string MAIN_SCENE = "Assets/Scenes/CubbleEditScene.unity";


    public static CubeList cubeList;

    [MenuItem("Tools/Switch to Main Scene", priority = 0)]
    public static void SwitchMainScene() {
        //todo: Ask for saving and open CubbleEditScene
        Debug.Log("SwitchMainScene");
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(MAIN_SCENE);
        }
    }

    [MenuItem("Tools/Generate Cube Previews", priority = 1)]
    public static void SwitchCubePreviewScene()
    {
        Debug.Log("SwitchCubePreviewScene");

        if (!AssetDatabase.IsValidFolder(PREVIEWS_PATH)) // check PREVIEWS_PATH 
        {
            AssetDatabase.CreateFolder("Assets/Generated", "CubePreviews");
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(PREVIEWS_SCENE);
            EditorCoroutineUtility.StartCoroutineOwnerless(IGenerateCubePreviews());
        }
    }
    static IEnumerator IGenerateCubePreviews()
    {
        
        GenerateCubes();

        yield return null;

        CaptureCubes();
    }

    private const float GENERATE_DIST = 15;

    private static void GenerateCubes()
    {
        cubeList = (CubeList)AssetDatabase.LoadAssetAtPath(CUBELIST_PATH, typeof(CubeList));
        Transform root = GameObject.Find("Root").transform;
        ClearChildren(root);

        float x = 0;
        float z = 0;
        for (int i = 0; i < cubeList.cubes.Length; i++)
        {
            GameObject.Instantiate(cubeList.cubes[i], new Vector3(x, 0, z), Quaternion.identity, root);
            x += GENERATE_DIST;
            if (x > GENERATE_DIST * 4)
            {
                x = 0;
                z += GENERATE_DIST;
            }
        }

        EditorApplication.QueuePlayerLoopUpdate();
        SceneView.RepaintAll();
    }

    private static void CaptureCubes()
    {
        Debug.Log("Capturing Cubes...");
        Transform camCenter = GameObject.Find("Center").transform;
        cubeList.cubePreviews = new Sprite[cubeList.cubes.Length];

        float x = 0;
        float z = 0;
        for (int i = 0; i < cubeList.cubes.Length; i++)
        {
            camCenter.transform.position = new Vector3(x, 0, z);

            Sprite s = Capture(i);
            cubeList.cubePreviews[i] = s;
            x += GENERATE_DIST;
            if (x > GENERATE_DIST * 4)
            {
                x = 0;
                z += GENERATE_DIST;
            }
        }

        EditorUtility.SetDirty(cubeList);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /*
     * 아직 수정중!
    private static void GenerateANDCaptureCubes()
    {
        cubeList = (CubeList)AssetDatabase.LoadAssetAtPath(CUBELIST_PATH, typeof(CubeList));
        Debug.Log("cube list: " + cubeList);

        for (int i = 0; i < cubeList.cubes.Length; i++)
        {
            Debug.Log("i: " + i);

            GameObject.Instantiate(cubeList.cubes[i], new Vector3(0, 0, 0), Quaternion.identity);

            Transform camCenter = GameObject.Find("Center").transform;

            cubeList.cubePreviews = new Sprite[cubeList.cubes.Length];
            
            camCenter.transform.position = new Vector3(0, 0, 0);

            cubeList.cubePreviews[i] = Capture(i);

            EditorUtility.SetDirty(cubeList);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Destroy(cubeList.cubes[i]); // 오브젝트 삭제
        }

        
    }

    static IEnumerator IGenerateCubePreviews()
    {
        if (!AssetDatabase.IsValidFolder(PREVIEWS_PATH)) // check PREVIEWS_PATH
        {
            AssetDatabase.CreateFolder("Assets/Generated", "CubePreviews");
        }
        GenerateANDCaptureCubes();
    }

  
    */

    private static void ClearChildren(Transform o)
    {
        int n = o.childCount;
        if (n <= 0) return;
        for (int i = n - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(o.GetChild(i).gameObject);
        }
    }



    /// <summary>
    /// Captures the specific cube prefab of the cube list using the main camera.
    /// This will capture what is rendered at the main camera and save it as a file, as well as return it as a sprite.
    /// Note how import options are handled, as well as how the RenderTexture is disposed (to prevent a memory leak)
    /// This code snippen is given on purpose; Read it thoroughly.

    private static Sprite Capture(int i) { // change spriteName to i
        //Capture preview and save it to asset folder
        Debug.Log("Capture()");

        string spriteName = cubeList.cubes[i].name;
        string path = PREVIEWS_PATH + "/" + spriteName + ".png";

        int w = 512;
        int h = 512;
        RenderTexture rt = new RenderTexture(w, h, 16);

        Camera cam = Camera.main;

        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(w, h, TextureFormat.RGBA32, false);
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;
        GameObject.DestroyImmediate(rt);

        //save screenshot
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.filterMode = FilterMode.Bilinear;

        var textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);
        if (textureSettings.spriteMeshType != SpriteMeshType.FullRect || textureSettings.spriteGenerateFallbackPhysicsShape) {
            textureSettings.spriteMeshType = SpriteMeshType.FullRect;
            textureSettings.spriteGenerateFallbackPhysicsShape = false;

            importer.SetTextureSettings(textureSettings);
        }

        AssetDatabase.WriteImportSettingsIfDirty(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
    }
}
