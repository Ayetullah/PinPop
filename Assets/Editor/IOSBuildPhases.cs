using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class IOSBuildPhases {
 // Unity can append the same IL2CPP phase twice when updating an existing export.
 [PostProcessBuild(9999)]public static void Normalize(BuildTarget target,string output){
  if(target!=BuildTarget.iOS)return;string path=PBXProject.GetPBXProjectPath(output);string source=File.ReadAllText(path);
  string normalized=Regex.Replace(source,@"buildPhases = \(\s*([\s\S]*?)\);",match=>{
   var seen=new HashSet<string>();return Regex.Replace(match.Value,@"^[\t ]*([A-Fa-f0-9]{24})[^\r\n]*\r?\n",line=>seen.Add(line.Groups[1].Value)?line.Value:"",RegexOptions.Multiline);
  });
  if(normalized!=source)File.WriteAllText(path,normalized);
 }
}
