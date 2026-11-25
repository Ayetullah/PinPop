using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public static class IOSLaunchScreen {
 [PostProcessBuild(90)] public static void Apply(BuildTarget target,string output){if(target!=BuildTarget.iOS)return;
  string catalog=Path.Combine(output,"Unity-iPhone/Images.xcassets");
  AddImage(catalog,"PinPopLaunch",File.ReadAllBytes("Assets/Resources/MapArt/region-gate.png"));
  AddImage(catalog,"PinPopForeground",File.ReadAllBytes("Assets/PreparedNative/foreground.png"));
  AddImage(catalog,"PinPopShade",File.ReadAllBytes("Assets/PreparedNative/shade.png"));
  const string name="PinPopLaunch.storyboard";File.WriteAllText(Path.Combine(output,name),Storyboard());
  var project=new PBXProject();string pbx=PBXProject.GetPBXProjectPath(output);project.ReadFromFile(pbx);string guid=project.FindFileGuidByProjectPath(name);if(string.IsNullOrEmpty(guid)){guid=project.AddFile(name,name,PBXSourceTree.Source);project.AddFileToBuild(project.GetUnityMainTargetGuid(),guid);}project.WriteToFile(pbx);
  var plist=new PlistDocument();string info=Path.Combine(output,"Info.plist");plist.ReadFromFile(info);plist.root.SetString("UILaunchStoryboardName","PinPopLaunch");plist.root.SetString("UILaunchStoryboardName~ipad","PinPopLaunch");plist.root.SetString("UILaunchStoryboardName~iphone","PinPopLaunch");plist.root.SetString("UILaunchStoryboardName~ipod","PinPopLaunch");plist.WriteToFile(info);
 }
 static void AddImage(string root,string name,byte[] bytes){string path=Path.Combine(root,name+".imageset");Directory.CreateDirectory(path);File.WriteAllBytes(Path.Combine(path,name+".png"),bytes);File.WriteAllText(Path.Combine(path,"Contents.json"),"{\"images\":[{\"filename\":\""+name+".png\",\"idiom\":\"universal\"}],\"info\":{\"author\":\"xcode\",\"version\":1}}");}
 static string Storyboard(){return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<document type=""com.apple.InterfaceBuilder3.CocoaTouch.Storyboard.XIB"" version=""3.0"" toolsVersion=""23727"" targetRuntime=""iOS.CocoaTouch"" propertyAccessControl=""none"" useAutolayout=""YES"" launchScreen=""YES"" useTraitCollections=""YES"" useSafeAreas=""YES"" colorMatched=""YES"" initialViewController=""launch-vc"">
<device id=""retina6_1"" orientation=""portrait"" appearance=""light""/><dependencies><plugIn identifier=""com.apple.InterfaceBuilder.IBCocoaTouchPlugin"" version=""23721""/></dependencies>
<scenes><scene sceneID=""launch-scene""><objects><viewController id=""launch-vc"" sceneMemberID=""viewController""><view key=""view"" contentMode=""scaleToFill"" id=""launch-view""><rect key=""frame"" x=""0"" y=""0"" width=""390"" height=""844""/><autoresizingMask key=""autoresizingMask"" widthSizable=""YES"" heightSizable=""YES""/><subviews>
"+Picture("launch-art","PinPopLaunch","scaleAspectFill")+Picture("launch-shade","PinPopShade","scaleToFill")+Picture("launch-foreground","PinPopForeground","scaleAspectFit")+@"
</subviews><viewLayoutGuide key=""safeArea"" id=""launch-safe""/><color key=""backgroundColor"" red=""0.76"" green=""0.84"" blue=""0.79"" alpha=""1"" colorSpace=""custom"" customColorSpace=""sRGB""/><constraints>
"+Fill("launch-art")+Fill("launch-shade")+Fill("launch-foreground")+@"
</constraints></view></viewController><placeholder placeholderIdentifier=""IBFirstResponder"" id=""launch-responder"" sceneMemberID=""firstResponder""/></objects></scene></scenes><resources><image name=""PinPopLaunch"" width=""1024"" height=""1536""/><image name=""PinPopShade"" width=""1"" height=""64""/><image name=""PinPopForeground"" width=""1800"" height=""3000""/></resources></document>";}
 static string Picture(string id,string image,string mode)=>"<imageView clipsSubviews=\"YES\" userInteractionEnabled=\"NO\" contentMode=\""+mode+"\" image=\""+image+"\" translatesAutoresizingMaskIntoConstraints=\"NO\" id=\""+id+"\"><rect key=\"frame\" x=\"0\" y=\"0\" width=\"390\" height=\"844\"/></imageView>";
 static string Fill(string id)=>"<constraint firstItem=\""+id+"\" firstAttribute=\"top\" secondItem=\"launch-view\" secondAttribute=\"top\" id=\""+id+"-top\"/><constraint firstItem=\""+id+"\" firstAttribute=\"bottom\" secondItem=\"launch-view\" secondAttribute=\"bottom\" id=\""+id+"-bottom\"/><constraint firstItem=\""+id+"\" firstAttribute=\"leading\" secondItem=\"launch-view\" secondAttribute=\"leading\" id=\""+id+"-left\"/><constraint firstItem=\""+id+"\" firstAttribute=\"trailing\" secondItem=\"launch-view\" secondAttribute=\"trailing\" id=\""+id+"-right\"/>";
}
