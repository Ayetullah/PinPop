using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// A prepared glyph strip replaces the Turkish word baked into the cart's plaque.
// Keep runtime work to a pixel copy, just like the existing prepared level digits.
public static class LocalizationAssets {
 public static void Prepare(){
  const string path="Assets/Resources/Localization/plaque-en.png";
  if(File.Exists(path))return;
  var obj=new GameObject("English plaque preparation");var prior=RenderTexture.active;var rt=new RenderTexture(512,96,24);Texture2D pixels=null;
  try {
   var camera=obj.AddComponent<Camera>();camera.orthographic=true;camera.orthographicSize=48;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;camera.cullingMask=1<<27;camera.transform.position=new Vector3(0,0,-10);camera.targetTexture=rt;
   var canvas=new GameObject("Plaque canvas",typeof(RectTransform),typeof(Canvas));canvas.layer=27;canvas.transform.SetParent(obj.transform,false);var c=canvas.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceCamera;c.worldCamera=camera;c.planeDistance=1;
   var text=PinPop.CrispUI.Text("English plaque heading",canvas.transform,0,0,512,96,"LEVEL",50,PinPop.Art.Hex("FFF1CE"));text.gameObject.layer=27;text.rectTransform.anchorMin=Vector2.zero;text.rectTransform.anchorMax=Vector2.one;text.rectTransform.offsetMin=text.rectTransform.offsetMax=Vector2.zero;
   for(int i=0;i<3;i++){text.cachedTextGenerator.Invalidate();Canvas.ForceUpdateCanvases();camera.Render();}RenderTexture.active=rt;pixels=new Texture2D(512,96,TextureFormat.RGBA32,false);pixels.ReadPixels(new Rect(0,0,512,96),0,0);pixels.Apply();Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllBytes(path,pixels.EncodeToPNG());
  } finally {RenderTexture.active=prior;Object.DestroyImmediate(obj);if(pixels!=null)Object.DestroyImmediate(pixels);rt.Release();Object.DestroyImmediate(rt);}
  AssetDatabase.Refresh();var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.isReadable=true;importer.npotScale=TextureImporterNPOTScale.None;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.wrapMode=TextureWrapMode.Clamp;var ios=importer.GetPlatformTextureSettings("iPhone");ios.overridden=true;ios.maxTextureSize=512;ios.format=TextureImporterFormat.RGBA32;importer.SetPlatformTextureSettings(ios);importer.SaveAndReimport();
 }
}
