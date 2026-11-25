using UnityEditor;
using UnityEngine;

public sealed class CrispUITextureImporter:AssetPostprocessor {
 void OnPreprocessTexture(){if(!assetPath.StartsWith("Assets/Resources/PinIcons/")&&!assetPath.StartsWith("Assets/Resources/MapArt/Icons/"))return;var i=(TextureImporter)assetImporter;i.mipmapEnabled=false;i.textureCompression=TextureImporterCompression.Uncompressed;i.filterMode=FilterMode.Bilinear;i.npotScale=TextureImporterNPOTScale.None;i.alphaIsTransparency=true;var ios=i.GetPlatformTextureSettings("iPhone");ios.overridden=true;ios.maxTextureSize=2048;ios.format=TextureImporterFormat.RGBA32;i.SetPlatformTextureSettings(ios);}
}
