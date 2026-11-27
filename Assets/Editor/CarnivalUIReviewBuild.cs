using System;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class CarnivalUIReviewBuild {
 [MenuItem("PinPop/Build carnival UI review")]
 public static void Export(){
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/PinPop.unity"},target=BuildTarget.StandaloneOSX,locationPathName="/tmp/PinPopCarnivalReview.app",options=BuildOptions.Development});
  if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Carnival review build failed: "+report.summary.result);
  UnityEngine.Debug.Log("PINPOP CARNIVAL REVIEW BUILD READY");
 }
}
