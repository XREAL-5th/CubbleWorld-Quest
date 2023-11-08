using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class MenuTools {
    [MenuItem("Tools/Switch to Main Scene", priority = 0)]
    public static void SwitchMainScene() {
        //todo: Ask for saving and open CubbleEditScene
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            EditorSceneManager.OpenScene("Assets/Scenes/CubbleEditScene.unity");
        }
    }

    private static Sprite Capture(int i) {
        //todo: Capture preview and save it to asset folder
        string spriteName = cubeList.cubes[i].name + "-preview";
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

    #region SKELETON
    //////////////// SKELETON CODE - do not touch ////////////////
    private const string PREVIEWS_PATH = "Assets/Generated/CubePreviews";
    private const string CUBELIST_PATH = "Assets/Scripts/Cubes/CubeList.asset";

    [MenuItem("Tools/Generate Cube Previews", priority = 1)]
    public static void SwitchCubePreviewScene() {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            EditorSceneManager.OpenScene("Assets/Scenes/CubeScreenshotScene.unity");
            EditorCoroutineUtility.StartCoroutineOwnerless(IGenerateCubePreviews());
        }
    }

    static IEnumerator IGenerateCubePreviews() {
        GenerateCubes();

        yield return null;

        CaptureCubes();
    }

    private const float GENERATE_DIST = 15;
    private static CubeList cubeList;

    private static void GenerateCubes() {
        cubeList = (CubeList)AssetDatabase.LoadAssetAtPath(CUBELIST_PATH, typeof(CubeList));
        Transform root = GameObject.Find("Root").transform;
        ClearChildren(root);

        float x = 0;
        float z = 0;
        for (int i = 0; i < cubeList.cubes.Length; i++) {
            GameObject.Instantiate(cubeList.cubes[i], new Vector3(x, 0, z), Quaternion.identity, root);
            x += GENERATE_DIST;
            if(x > GENERATE_DIST * 4) {
                x = 0;
                z += GENERATE_DIST;
            }
        }

        EditorApplication.QueuePlayerLoopUpdate();
        SceneView.RepaintAll();
    }

    private static void ClearChildren(Transform o) {
        int n = o.childCount;
        if (n <= 0) return;
        for (int i = n - 1; i >= 0; i--) {
            GameObject.DestroyImmediate(o.GetChild(i).gameObject);
        }
    }

    private static void CaptureCubes() {
        Debug.Log("Capturing Cubes...");
        Transform camCenter = GameObject.Find("Center").transform;
        cubeList.cubePreviews = new Sprite[cubeList.cubes.Length];

        float x = 0;
        float z = 0;
        for (int i = 0; i < cubeList.cubes.Length; i++) {
            camCenter.transform.position = new Vector3(x, 0, z);

            Sprite s = Capture(i);
            cubeList.cubePreviews[i] = s;
            x += GENERATE_DIST;
            if (x > GENERATE_DIST * 4) {
                x = 0;
                z += GENERATE_DIST;
            }
        }

        EditorUtility.SetDirty(cubeList);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    //////////////// SKELETON CODE - do not touch ////////////////
    #endregion
}
