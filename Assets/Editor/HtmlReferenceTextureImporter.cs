using System.IO;
using UnityEditor;
using UnityEngine;

// Keep editor imports identical to the generated player build.
public sealed class HtmlReferenceTextureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith("Assets/Resources/HtmlReference/")) return;
        var importer = (TextureImporter)assetImporter;
        string name = Path.GetFileNameWithoutExtension(assetPath);
        importer.isReadable = name == "texture-4" || name.StartsWith("plaque-digit-");
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = !name.StartsWith("p");
        importer.alphaIsTransparency = true;
        importer.sRGBTexture = true;
    }
}
