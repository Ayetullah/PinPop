using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using PinPop;

public static class SplashArtworkPreparation {
 public static void Prepare(){
  const string native="Assets/PreparedNative",runtime="Assets/Resources/Startup";
  Directory.CreateDirectory(native);Directory.CreateDirectory(runtime);
  string hash=System.Convert.ToBase64String(SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes("v3-language-neutral"+File.ReadAllText("Assets/Scripts/PinPopSplash.cs")+AssetDatabase.GetAssetDependencyHash("Assets/Resources/PinPop-Bold.ttf"))));
  if(File.Exists(native+"/version.txt")&&File.ReadAllText(native+"/version.txt")==hash&&File.Exists(runtime+"/launch-title.png"))return;
  var obj=new GameObject("Native launch artwork preparation");var prior=RenderTexture.active;var rt=new RenderTexture(1800,3000,24,RenderTextureFormat.ARGB32);Texture2D image=null;
  try {
   var splash=obj.AddComponent<PinPopSplash>();splash.BuildEditorPreview();splash.FitTo(1800,3000);
   foreach(var graphic in obj.GetComponentsInChildren<Graphic>(true)){
    if(graphic.name=="Splash evening shade")File.WriteAllBytes(native+"/shade.png",((Image)graphic).sprite.texture.EncodeToPNG());
    if(!graphic.name.StartsWith("Splash logo")&&!graphic.name.StartsWith("Splash pin ")&&graphic.name!="Splash bowling ball")graphic.gameObject.SetActive(false);
   }
   var camera=splash.SplashCamera;camera.targetTexture=rt;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;for(int pass=0;pass<3;pass++){foreach(var text in obj.GetComponentsInChildren<Text>()){text.cachedTextGenerator.Invalidate();text.SetAllDirty();}Canvas.ForceUpdateCanvases();camera.Render();}RenderTexture.active=rt;
   image=new Texture2D(1800,3000,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,1800,3000),0,0);image.Apply();File.WriteAllBytes(native+"/foreground.png",image.EncodeToPNG());Object.DestroyImmediate(image);image=null;
   image=new Texture2D(1800,540,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,1350,1800,540),0,0);image.Apply();File.WriteAllBytes(runtime+"/launch-title.png",image.EncodeToPNG());
   File.WriteAllText(native+"/version.txt",hash);
  } finally {RenderTexture.active=prior;Object.DestroyImmediate(obj);rt.Release();Object.DestroyImmediate(rt);if(image!=null)Object.DestroyImmediate(image);}
  AssetDatabase.Refresh();var importer=(TextureImporter)AssetImporter.GetAtPath(runtime+"/launch-title.png");importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.mipmapEnabled=false;importer.isReadable=false;importer.alphaIsTransparency=true;importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.Uncompressed;var ios=importer.GetPlatformTextureSettings("iPhone");ios.overridden=true;ios.maxTextureSize=2048;ios.format=TextureImporterFormat.RGBA32;importer.SetPlatformTextureSettings(ios);importer.SaveAndReimport();
 }
}
