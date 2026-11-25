using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

// Uses the same meshes and printed coats as the game; no runtime icon generator is shipped.
public static class IOSAppIconSetup
{
    public const string Folder = "Assets/AppIcons/iOS";
    public const string MasterPath = Folder + "/PinPop-iOS-1024.png";
    static readonly NamedBuildTarget Target = NamedBuildTarget.iOS;

    [MenuItem("PinPop/iOS/Prepare app icons")]
    public static void Prepare()
    {
        Directory.CreateDirectory(Folder);
        RenderArtwork();
        ApplyIfPresent();
        AssetDatabase.SaveAssets();
        Validate();
    }

    // Called by scene generation as well, so a future project rebuild retains the artwork.
    public static void ApplyIfPresent()
    {
        if (!File.Exists(MasterPath)) return;
        AssetDatabase.ImportAsset(MasterPath);
        var textures = new Dictionary<string, Texture2D>();
        foreach (var kind in PlayerSettings.GetSupportedIconKinds(Target))
        {
            var slots = PlayerSettings.GetPlatformIcons(Target, kind);
            bool changed = false;
            foreach (var slot in slots)
            {
                string path = PathFor(slot.width, slot.height);
                if (!textures.TryGetValue(path, out var texture))
                {
                    if (!File.Exists(path)) ResizeMaster(path, slot.width, slot.height);
                    Import(path);
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    textures.Add(path, texture);
                }
                if (slot.GetTexture() != texture) { slot.SetTexture(texture); changed = true; }
            }
            if (changed) PlayerSettings.SetPlatformIcons(Target, kind, slots);
        }
    }

    static string PathFor(int width, int height) => width == 1024 && height == 1024
        ? MasterPath : Folder + "/PinPop-iOS-" + width + "x" + height + ".png";

    static void Import(string path)
    {
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer.textureType == TextureImporterType.Default && !importer.mipmapEnabled &&
            importer.alphaSource == TextureImporterAlphaSource.None && importer.maxTextureSize == 1024 &&
            importer.textureCompression == TextureImporterCompression.Uncompressed && importer.npotScale == TextureImporterNPOTScale.None) return;
        importer.textureType = TextureImporterType.Default;
        importer.alphaSource = TextureImporterAlphaSource.None;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = 1024;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.sRGBTexture = true;
        importer.SaveAndReimport();
    }

    static void RenderArtwork()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
        var stage = new GameObject("iOS icon studio") { hideFlags = HideFlags.HideAndDontSave };
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(stage, scene);
        var previousAmbient = RenderSettings.ambientLight;
        var previousMode = RenderSettings.ambientMode;
        var previousActive = RenderTexture.active;
        var owned = new List<UnityEngine.Object>();
        var rt = new RenderTexture(2048, 2048, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB) { antiAliasing = 4 };
        try
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.65f, .65f, .68f);
            var camObject = new GameObject("Icon camera"); camObject.transform.SetParent(stage.transform, false);
            var camera = camObject.AddComponent<Camera>(); camera.enabled = false; camera.orthographic = true;
            camera.orthographicSize = 1.48f; camera.aspect = 1; camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = PinPop.Art.Hex("32A6BA"); camera.cullingMask = 1 << 31;
            camera.scene = scene;
            camera.nearClipPlane = .1f; camera.farClipPlane = 20; camera.allowHDR = false;
            camera.transform.position = new Vector3(0, 1.85f, -6);
            camera.transform.LookAt(new Vector3(0, 1.08f, 0)); camera.targetTexture = rt;
            LightRig(stage.transform, "Soft key", new Vector3(28, -32, 0), 1.2f);
            LightRig(stage.transform, "Rim light", new Vector3(40, 155, 0), .65f);
            var background = GameObject.CreatePrimitive(PrimitiveType.Quad);
            background.name = "Full bleed turquoise backdrop"; background.transform.SetParent(stage.transform, false);
            background.transform.position = new Vector3(0, 1, 2); background.transform.localScale = Vector3.one * 10;
            var texture = new Texture2D(128, 128, TextureFormat.RGB24, false);
            for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
            {
                float radius = Vector2.Distance(new Vector2(x / 127f, y / 127f), new Vector2(.4f, .65f));
                texture.SetPixel(x, y, Color.Lerp(PinPop.Art.Hex("60D4D8"), PinPop.Art.Hex("13849F"), Mathf.Clamp01(radius)));
            }
            texture.Apply(); owned.Add(texture);
            var backMat = new Material(Shader.Find("Unlit/Texture")) { mainTexture = texture }; owned.Add(backMat);
            background.GetComponent<Renderer>().sharedMaterial = backMat;
            int[] colors = { 0, 1, 2 };
            for (int i = 0; i < 3; i++)
            {
                var pin = new GameObject("Icon pin " + i); pin.transform.SetParent(stage.transform, false);
                pin.transform.localPosition = new Vector3((i - 1) * .75f, i == 1 ? .35f : .21f, .32f);
                pin.transform.localScale = Vector3.one * 1.15f;
                pin.transform.localRotation = Quaternion.Euler(0, 0, i == 0 ? 9 : i == 2 ? -9 : 0);
                pin.AddComponent<MeshFilter>().sharedMesh = PinPop.Art.PinMesh();
                var renderer = pin.AddComponent<MeshRenderer>(); renderer.sharedMaterial = PinPop.Art.PinCoat(colors[i]);
                renderer.shadowCastingMode = ShadowCastingMode.Off;
            }
            var ball = Sphere(stage.transform, "Polished bowling ball", Vector3.zero, Vector3.one * 1.38f, PinPop.BallLooks.Material(0, 0));
            foreach (var hole in new[] { new Vector3(-.20f, .70f, -.66f), new Vector3(.20f, .70f, -.66f), new Vector3(0, .42f, -.90f) })
                Sphere(ball.transform, "Finger hole", hole.normalized * .48f, Vector3.one * .105f, PinPop.Art.Mat(PinPop.Art.Hex("263051")));
            ball.transform.localPosition = new Vector3(-.47f, .58f, -.85f);
            ball.transform.localRotation = Quaternion.Euler(10, -8, -10);
            foreach (var child in stage.GetComponentsInChildren<Transform>(true)) child.gameObject.layer = 31;
            camera.Render();
            Save(rt, MasterPath, 1024, 1024);
            // Small, independent preview sizes are useful even if not requested by this Unity version.
            foreach (int size in new[] { 180, 120, 60 }) Save(rt, PathFor(size, size), size, size);
        }
        finally
        {
            RenderTexture.active = previousActive;
            rt.Release(); UnityEngine.Object.DestroyImmediate(rt);
            UnityEngine.Object.DestroyImmediate(stage);
            foreach (var asset in owned) UnityEngine.Object.DestroyImmediate(asset);
            UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(scene);
            RenderSettings.ambientMode = previousMode; RenderSettings.ambientLight = previousAmbient;
        }
        // Regenerate every existing derived size when the artwork changes.
        foreach (var kind in PlayerSettings.GetSupportedIconKinds(Target))
            foreach (var slot in PlayerSettings.GetPlatformIcons(Target, kind))
                if (slot.width != 1024 || slot.height != 1024) ResizeMaster(PathFor(slot.width, slot.height), slot.width, slot.height);
        Import(MasterPath);
    }

    static GameObject Sphere(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere); obj.name = name;
        obj.transform.SetParent(parent, false); obj.transform.localPosition = position; obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        UnityEngine.Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }

    static void LightRig(Transform parent, string name, Vector3 angle, float intensity)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false); go.transform.localRotation = Quaternion.Euler(angle);
        var light = go.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = intensity; light.cullingMask = 1 << 31;
    }

    static void ResizeMaster(string path, int width, int height)
    {
        var source = new Texture2D(2, 2, TextureFormat.RGB24, false);
        source.LoadImage(File.ReadAllBytes(MasterPath));
        try { Save(source, path, width, height); } finally { UnityEngine.Object.DestroyImmediate(source); }
    }

    static void Save(Texture source, string path, int width, int height)
    {
        var active = RenderTexture.active;
        var resized = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        var image = new Texture2D(width, height, TextureFormat.RGB24, false);
        try
        {
            Graphics.Blit(source, resized); RenderTexture.active = resized;
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
        }
        finally { RenderTexture.active = active; RenderTexture.ReleaseTemporary(resized); UnityEngine.Object.DestroyImmediate(image); }
    }

    [MenuItem("PinPop/iOS/Validate saved app icons")]
    public static void Validate()
    {
        var master = new Texture2D(2, 2, TextureFormat.RGB24, false);
        master.LoadImage(File.ReadAllBytes(MasterPath));
        int pink = 0, white = 0, purple = 0;
        foreach (var pixel in master.GetPixels32())
        {
            if (pixel.r > 90 && pixel.r > pixel.g * 1.25f && pixel.r > pixel.b * .95f) pink++;
            if (pixel.r > 205 && pixel.g > 205 && pixel.b > 205) white++;
            if (pixel.r > 70 && pixel.b > pixel.g * 1.1f && pixel.r > pixel.g * 1.05f) purple++;
        }
        UnityEngine.Object.DestroyImmediate(master);
        if (pink < 1000 || white < 500 || purple < 500) throw new InvalidOperationException("Rendered icon is missing its bowling ball or character pins");
        int count = 0;
        var rows = new List<string>();
        foreach (var kind in PlayerSettings.GetSupportedIconKinds(Target))
        {
            foreach (var slot in PlayerSettings.GetPlatformIcons(Target, kind))
            {
                var texture = slot.GetTexture();
                if (texture == null || texture.width != slot.width || texture.height != slot.height)
                    throw new InvalidOperationException("Missing or incorrectly sized iOS icon: " + kind + " / " + slot.width + "x" + slot.height);
                string path = AssetDatabase.GetAssetPath(texture); var bytes = File.ReadAllBytes(path);
                if (bytes[25] != 2) throw new InvalidOperationException("iOS PNG must be RGB without alpha: " + path);
                count++; rows.Add(kind + " / " + slot.width + "x" + slot.height + " / " + path);
            }
        }
        if (count == 0) throw new InvalidOperationException("iOS platform icon slots unavailable");
        Directory.CreateDirectory("Documentation");
        File.WriteAllText("Documentation/ios-icon-verification.txt", count + " iOS icon slots verified.\n" + string.Join("\n", rows));
        Debug.Log("PINPOP: " + count + " saved iOS icon slots verified.");
    }
}
