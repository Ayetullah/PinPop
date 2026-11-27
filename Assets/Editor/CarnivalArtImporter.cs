using UnityEditor;
using UnityEngine;

public sealed class CarnivalArtImporter : AssetPostprocessor {
 void OnPreprocessTexture(){if(!assetPath.StartsWith("Assets/Resources/CarnivalUI/"))return;var i=(TextureImporter)assetImporter;i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=100;i.mipmapEnabled=false;i.isReadable=false;i.npotScale=TextureImporterNPOTScale.None;i.textureCompression=TextureImporterCompression.Uncompressed;i.alphaIsTransparency=true;i.filterMode=FilterMode.Bilinear;i.wrapMode=TextureWrapMode.Clamp;i.maxTextureSize=4096;var ios=i.GetPlatformTextureSettings("iPhone");ios.overridden=true;ios.maxTextureSize=4096;ios.format=TextureImporterFormat.RGBA32;i.SetPlatformTextureSettings(ios);}
}
