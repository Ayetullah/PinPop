using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using PinPop;

public sealed class ProductionAssets : IPreprocessBuildWithReport {
 const string Root="Assets/Resources/Prepared";
 public int callbackOrder=>-100;
 public void OnPreprocessBuild(BuildReport report){Prepare();}
 [MenuItem("PinPop/Prepare production assets")]
 public static void Prepare(){
  Directory.CreateDirectory(Root);AssetDatabase.Refresh();
  string hash=AssetDatabase.GetAssetDependencyHash("Assets/Resources/HtmlReference/scene.json").ToString();
  string stamp=Root+"/world-version.txt";
  if(!File.Exists(Root+"/World.prefab")||!File.Exists(stamp)||File.ReadAllText(stamp)!=hash){
   var obj=new GameObject("Prepared carnival world");
   try {
    var source=obj.AddComponent<HtmlReferenceWorld>();source.BuildSource();
    int id=0;var filters=obj.GetComponentsInChildren<MeshFilter>(true);foreach(var mesh in filters.Select(x=>x.sharedMesh).Distinct().ToArray()){var saved=(Mesh)Save(mesh,"mesh-"+id++);foreach(var filter in filters)if(filter.sharedMesh==mesh)filter.sharedMesh=saved;}
    id=0;var renderers=obj.GetComponentsInChildren<Renderer>(true);var geometry=obj.GetComponent<PreparedWorld>();foreach(var mat in renderers.SelectMany(x=>x.sharedMaterials).Distinct().ToArray()){var saved=(Material)Save(mat,"surface-"+id++);if(geometry.Plaque==mat)geometry.Plaque=saved;foreach(var renderer in renderers){var assigned=renderer.sharedMaterials;for(int j=0;j<assigned.Length;j++)if(assigned[j]==mat)assigned[j]=saved;renderer.sharedMaterials=assigned;}}
    Object.DestroyImmediate(source);PrefabUtility.SaveAsPrefabAsset(obj,Root+"/World.prefab");File.WriteAllText(stamp,hash);
   } finally {Object.DestroyImmediate(obj);}
  }
  if(!File.Exists(Root+"/pin-mesh.asset"))Save(Art.PinMesh(),"pin-mesh");
  Directory.CreateDirectory(Root+"/Balls");
  for(int style=0;style<4;style++)for(int color=0;color<PinPopPalette.Count;color++){
   string icon=Root+"/Balls/icon-"+style+"-"+color+".png";
   if(!File.Exists(icon))File.WriteAllBytes(icon,BallLooks.GenerateIcon(style,color,512).texture.EncodeToPNG());
   if(style>0){string texture=Root+"/Balls/coat-"+style+"-"+color+".png";if(!File.Exists(texture))File.WriteAllBytes(texture,BallLooks.GenerateTexture(style,color).EncodeToPNG());}
  }
  Directory.CreateDirectory(Root+"/Help");
  string[] names={"wood","sky","card","plate","shadow","ring","disc"};Sprite[] art={HelpDemoArt.Wood,HelpDemoArt.Sky,HelpDemoArt.Card,HelpDemoArt.Plate,HelpDemoArt.Shadow,HelpDemoArt.Ring,HelpDemoArt.Disc};for(int i=0;i<names.Length;i++){string path=Root+"/Help/"+names[i]+".png";if(!File.Exists(path))File.WriteAllBytes(path,art[i].texture.EncodeToPNG());}
  AssetDatabase.Refresh();
  foreach(string path in Directory.GetFiles(Root+"/Balls","*.png").Concat(Directory.GetFiles(Root+"/Help","*.png"))){
   var importer=(TextureImporter)AssetImporter.GetAtPath(path);bool icon=path.Contains("/Help/")||Path.GetFileName(path).StartsWith("icon-");
   if(importer.userData=="PinPop prepared v1")continue;
   importer.textureType=icon?TextureImporterType.Sprite:TextureImporterType.Default;importer.spriteImportMode=SpriteImportMode.Single;importer.mipmapEnabled=!icon;importer.alphaIsTransparency=icon;importer.isReadable=false;importer.filterMode=icon?FilterMode.Bilinear:FilterMode.Trilinear;importer.wrapMode=icon?TextureWrapMode.Clamp:TextureWrapMode.Repeat;importer.maxTextureSize=512;importer.textureCompression=TextureImporterCompression.Uncompressed;
   var ios=importer.GetPlatformTextureSettings("iPhone");ios.overridden=true;ios.format=TextureImporterFormat.RGBA32;ios.maxTextureSize=512;importer.SetPlatformTextureSettings(ios);importer.userData="PinPop prepared v1";importer.SaveAndReimport();
  }
  LocalizationAssets.Prepare();SplashArtworkPreparation.Prepare();AssetDatabase.SaveAssets();Debug.Log("PINPOP: production assets prepared (world geometry and 32 ball icons).");
 }
 static Object Save(Object value,string name){string path=Root+"/"+name+".asset";var prior=AssetDatabase.LoadAssetAtPath<Object>(path);if(prior!=null){EditorUtility.CopySerialized(value,prior);return prior;}AssetDatabase.CreateAsset(value,path);return value;}
}
