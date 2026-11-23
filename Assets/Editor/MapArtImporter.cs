using UnityEditor;
using UnityEngine;
public sealed class MapArtImporter:AssetPostprocessor {
 void OnPreprocessTexture(){if(!assetPath.StartsWith("Assets/Resources/MapArt/"))return;var i=(TextureImporter)assetImporter;i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=100;i.mipmapEnabled=false;i.alphaIsTransparency=true;i.isReadable=false;i.filterMode=FilterMode.Bilinear;i.maxTextureSize=assetPath.Contains("/Icons/")?256:2048;i.textureCompression=assetPath.Contains("/Icons/")?TextureImporterCompression.Uncompressed:TextureImporterCompression.CompressedHQ;}
}
