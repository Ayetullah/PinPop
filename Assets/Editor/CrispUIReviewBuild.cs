using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
public static class CrispUIReviewBuild {
 public static void Export(){var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/PinPop.unity"},target=BuildTarget.StandaloneOSX,locationPathName="/tmp/PinPopPolishReview.app",options=BuildOptions.Development});if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Polish review build failed");}
}
