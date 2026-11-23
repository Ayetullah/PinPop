using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using PinPop;

public static class ProjectSetup
{
    [MenuItem("PinPop/Create or reset playable scene")]
    public static void CreateScene()
    {
        // The scene is generated at runtime, so retain shaders explicitly in player builds.
        var graphics = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
        var shaders = graphics.FindProperty("m_AlwaysIncludedShaders");
        foreach (var name in new[] { "Standard", "Sprites/Default", "UI/Default", "PinPop/HTML Surface", "PinPop/HTML Transparent", "PinPop/HTML Unlit", "Hidden/PinPop/HtmlLook" })
        {
            var shader = Shader.Find(name); bool exists = false;
            for (int i = 0; i < shaders.arraySize; i++) exists |= shaders.GetArrayElementAtIndex(i).objectReferenceValue == shader;
            if (!exists) { int i = shaders.arraySize; shaders.InsertArrayElementAtIndex(i); shaders.GetArrayElementAtIndex(i).objectReferenceValue = shader; }
        }
        graphics.ApplyModifiedPropertiesWithoutUndo();
        foreach(var path in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/Resources/HtmlReference"}))
        {
            var file=AssetDatabase.GUIDToAssetPath(path);var importer=AssetImporter.GetAtPath(file) as TextureImporter;if(importer==null)continue;
            importer.isReadable=Path.GetFileName(file)=="texture-4.png"||Path.GetFileName(file).StartsWith("plaque-digit-");importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.filterMode=FilterMode.Bilinear;importer.mipmapEnabled=!Path.GetFileName(file).StartsWith("p");importer.alphaIsTransparency=true;importer.sRGBTexture=true;importer.SaveAndReimport();
        }
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        new GameObject("PinPop · Game").AddComponent<PinPopGame>();
        SplashScenePreviewSetup.Create();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/PinPop.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/PinPop.unity", true) };
        PlayerSettings.companyName = "PinPop Studio"; PlayerSettings.productName = "PinPop";
        PlayerSettings.defaultScreenWidth = 600; PlayerSettings.defaultScreenHeight = 1000;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed; PlayerSettings.resizableWindow = true;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false; PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.colorSpace = ColorSpace.Linear;
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Standalone, "com.pinpop.colorbowling");
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.pinpop.colorbowling");
        AssetDatabase.SaveAssets(); Debug.Log("PINPOP: Playable scene generated.");
    }
    public static void BuildMac()
    {
        CreateScene(); SplashScenePreviewSetup.ValidateAndCapture(); PinIconBaker.Bake(); Directory.CreateDirectory("Builds");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { "Assets/Scenes/PinPop.unity" }, locationPathName = "Builds/PinPop.app", target = BuildTarget.StandaloneOSX, options = BuildOptions.Development });
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) throw new System.Exception("Build failed: " + report.summary.result);
        Debug.Log("PINPOP: macOS build succeeded.");
    }
}
