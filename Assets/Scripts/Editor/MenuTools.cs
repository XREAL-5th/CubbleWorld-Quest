using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;

public static class MenuTools {
    
    private const string MainScene = "Assets/Scenes/CubbleEditScene.unity";
    private const string PreviewScene = "Assets/Scenes/CubePreviewScene.unity";
    
    [MenuItem("Tools/Switch to Main Scene", priority = 0)]
    public static void SwitchMainScene() {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(MainScene);
        }
    }

    private const string CUBELIST_PATH = "Assets/Scripts/Cubes/CubeList.asset";

    [MenuItem("Tools/Generate Cube Previews", priority = 1)]
    public static void SwitchCubePreviewScene()
    {
        var scenePath = EditorSceneManager.GetActiveScene().path;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        EditorSceneManager.OpenScene(PreviewScene);
        var cubeAsset = AssetDatabase.LoadAssetAtPath<CubeList>(CUBELIST_PATH);
        cubeAsset.Sprites = new();
        foreach (var cube in cubeAsset.cubes)
        {
            var sprite = CaptureCube(cube);
            cubeAsset.Sprites.Add(sprite);
            EditorUtility.SetDirty(cubeAsset);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(scenePath);
    }

    private static Sprite CaptureCube(GameObject cube)
    {
        var instance = PrefabUtility.InstantiatePrefab(cube);
        var savePath = CubeList.PreviewPath(cube);
        try
        {
            return Capture(savePath);
        }
        finally
        {
            Object.DestroyImmediate(instance);
        }
    }

    /// <summary>
    /// Captures the specific cube prefab of the cube list using the main camera.
    /// This will capture what is rendered at the main camera and save it as a file, as well as return it as a sprite.
    /// Note how import options are handled, as well as how the RenderTexture is disposed (to prevent a memory leak)
    /// This code snippen is given on purpose; Read it thoroughly.
    /// </summary>
    private static Sprite Capture(string path) {
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
