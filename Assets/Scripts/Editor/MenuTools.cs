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

        ///////////////////////////////////////////////////////
        //////////////// TODO: WRITE CODE HERE ////////////////
        ///////////////////////////////////////////////////////
    }

    private const string PREVIEWS_PATH = "Assets/Generated/CubePreviews";
    private const string CUBELIST_PATH = "Assets/Scripts/Cubes/CubeList.asset";

    [MenuItem("Tools/Generate Cube Previews", priority = 1)]
    public static void SwitchCubePreviewScene() {
        //todo: Generate cube preview assets
        //you can use the above path consts, as well as the Capture() method provided below

        ///////////////////////////////////////////////////////
        //////////////// TODO: WRITE CODE HERE ////////////////
        ///////////////////////////////////////////////////////
    }

    /// <summary>
    /// Captures the specific cube prefab of the cube list using the main camera.
    /// This will capture what is rendered at the main camera and save it as a file, as well as return it as a sprite.
    /// Note how import options are handled, as well as how the RenderTexture is disposed (to prevent a memory leak)
    /// This code snippen is given on purpose; Read it thoroughly.
    /// </summary>
    private static Sprite Capture(string spriteName) {
        //Capture preview and save it to asset folder
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
