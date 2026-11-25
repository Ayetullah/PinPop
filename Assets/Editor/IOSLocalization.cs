using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class IOSLocalization {
 [PostProcessBuild(110)]public static void Apply(BuildTarget target,string output){
  if(target!=BuildTarget.iOS)return;
  var plist=new PlistDocument();string path=Path.Combine(output,"Info.plist");plist.ReadFromFile(path);
  plist.root.SetString("CFBundleDevelopmentRegion","en");var languages=plist.root.CreateArray("CFBundleLocalizations");languages.AddString("en");foreach(var choice in PinPop.GameLocalization.SupportedLanguages)if(choice.code!="en")languages.AddString(choice.code);plist.WriteToFile(path);
 }
}
