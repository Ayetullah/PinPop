using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using UnityEngine.Rendering;

public static class IOSDeviceBuild
{
    public const string BundleId = "com.pinpop.colorbowling";
    public const string ScenePath = "Assets/Scenes/PinPop.unity";

    [MenuItem("PinPop/iOS/Configure real device testing")]
    public static void Configure()
    {
        var target = NamedBuildTarget.iOS;
        PlayerSettings.SetApplicationIdentifier(target, BundleId);
        PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(target, 1); // ARM64, not the simulator architecture.
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new[] { GraphicsDeviceType.Metal });
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.targetOSVersionString = "15.0";
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
        PlayerSettings.iOS.requiresFullScreen = true;
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        string team = Environment.GetEnvironmentVariable("PINPOP_APPLE_TEAM_ID");
        if (!string.IsNullOrWhiteSpace(team)) PlayerSettings.iOS.appleDeveloperTeamID = team.Trim();
        if (string.IsNullOrEmpty(PlayerSettings.iOS.buildNumber) || PlayerSettings.iOS.buildNumber == "0") PlayerSettings.iOS.buildNumber = "1";
        IOSAppIconSetup.ApplyIfPresent();
        RuntimeShaderBuild.EnsureIncluded();
        PlayerSettings.SplashScreen.show = false;
        AssetDatabase.SaveAssets();
        ValidateSettings();
    }

    [MenuItem("PinPop/iOS/Export development Xcode project")]
    public static void Export()
    {
        Configure();
        if (!File.Exists(ScenePath)) throw new InvalidOperationException("Missing playable scene: " + ScenePath);
        IOSAppIconSetup.Validate();
        var output = Environment.GetEnvironmentVariable("PINPOP_IOS_EXPORT_PATH");
        if (string.IsNullOrEmpty(output)) output = Path.GetFullPath("Builds/iOSDevice");
        Directory.CreateDirectory(output);
        var options = BuildOptions.Development | BuildOptions.AllowDebugging;
        if (File.Exists(PBXProject.GetPBXProjectPath(output))) options |= BuildOptions.AcceptExternalModificationsToPlayer;
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { ScenePath }, locationPathName = output,
            target = BuildTarget.iOS, options = options
        });
        if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("iOS export failed: " + report.summary.result);
        ValidateExport(output);
        Debug.Log("PINPOP: iOS development Xcode project exported to " + output);
    }

    public static void ExportDiagnostics(){
        Configure();string output=Environment.GetEnvironmentVariable("PINPOP_IOS_EXPORT_PATH")??Path.GetFullPath("Builds/iOSDevice");
        var options=BuildOptions.Development;if(File.Exists(PBXProject.GetPBXProjectPath(output)))options|=BuildOptions.AcceptExternalModificationsToPlayer;
        var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},target=BuildTarget.iOS,locationPathName=output,options=options});
        if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Diagnostic export failed");
    }

    [MenuItem("PinPop/iOS/Export release candidate (test ads)")]
    public static void ExportReleaseCandidate()
    {
        Configure();
        string output = Environment.GetEnvironmentVariable("PINPOP_IOS_RELEASE_PATH");
        if (string.IsNullOrEmpty(output)) output = Path.GetFullPath("Builds/iOSReleaseCandidate");
        Directory.CreateDirectory(output);
        var options = BuildOptions.None;
        if (File.Exists(PBXProject.GetPBXProjectPath(output))) options |= BuildOptions.AcceptExternalModificationsToPlayer;
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { ScenePath }, locationPathName = output, target = BuildTarget.iOS, options = options });
        if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Release candidate export failed");
        ValidateExport(output);
        string boot = File.ReadAllText(Path.Combine(output,"Data/boot.config"));
        if (boot.Contains("player-connection-debug=1") || boot.Contains("wait-for-managed-debugger=1")) throw new InvalidOperationException("Development connection leaked into release candidate");
        Directory.CreateDirectory("Documentation");
        File.WriteAllText("Documentation/ios-release-candidate.json", "{\"developmentBuild\":false,\"scriptDebugging\":false,\"testAds\":true,\"liveAdsExcludedByUser\":true,\"nativeBuildPerformed\":false}");
        Debug.Log("PINPOP: Release candidate exported (test ads intentionally retained): " + output);
    }

    public static void ValidateSettings()
    {
        var target = NamedBuildTarget.iOS;
        if (PlayerSettings.GetApplicationIdentifier(target) != BundleId ||
            PlayerSettings.GetScriptingBackend(target) != ScriptingImplementation.IL2CPP ||
            PlayerSettings.GetArchitecture(target) != 1 ||
            PlayerSettings.iOS.sdkVersion != iOSSdkVersion.DeviceSDK ||
            PlayerSettings.iOS.targetOSVersionString != "15.0" || !PlayerSettings.iOS.appleEnableAutomaticSigning)
            throw new InvalidOperationException("iOS device settings are incomplete");
        var config = Resources.Load<TextAsset>("AdMobConfig");
        if (config == null || !JsonUtility.FromJson<AdsConfig>(config.text).useTestAds)
            throw new InvalidOperationException("Enable AdMob test ads before exporting a device test build");
        Directory.CreateDirectory("Documentation");
        File.WriteAllText("Documentation/ios-device-settings-verification.json", JsonUtility.ToJson(new SettingsAudit {
            bundleId = BundleId, backend = "IL2CPP", architecture = "ARM64", minimumIOS = "15.0", sdk = "DeviceSDK",
            automaticSigning = true, developmentExport = true, testAds = true,
            teamConfigured = !string.IsNullOrWhiteSpace(PlayerSettings.iOS.appleDeveloperTeamID)
        }, true));
        Debug.Log("PINPOP: Real-device iOS settings verified.");
    }

    public static void ValidateExport(string output)
    {
        var plist = new PlistDocument(); plist.ReadFromFile(Path.Combine(output, "Info.plist"));
        if (plist.root["GADApplicationIdentifier"].AsString() != "ca-app-pub-3940256099942544~1458002511")
            throw new InvalidOperationException("Expected the existing Google iOS sample application ID in device test export");
        if (plist.root["SKAdNetworkItems"].AsArray().values.Count == 0)
            throw new InvalidOperationException("Ad SDK postprocessing did not add its network entries");
        string project = File.ReadAllText(PBXProject.GetPBXProjectPath(output));
        if (!project.Contains(BundleId) || !project.Contains("Automatic"))
            throw new InvalidOperationException("Missing bundle identifier or automatic signing in Xcode project");
        var icons = Path.Combine(output, "Unity-iPhone/Images.xcassets/AppIcon.appiconset");
        if (!File.Exists(Path.Combine(icons, "Contents.json")) || Directory.GetFiles(icons, "*.png").Length == 0)
            throw new InvalidOperationException("Exported Xcode icon catalog is empty");
        File.WriteAllText("Documentation/ios-device-export-verification.json", JsonUtility.ToJson(new ExportAudit {
            outputPath = output, bundleId = BundleId, infoPlist = true, testAdApplicationId = true,
            adNetworkEntries = true, iconCatalog = true, automaticSigning = true,
            nativeAccessibility = File.Exists(Path.Combine(output, "Libraries/Plugins/iOS/PinPopAccessibility.mm")),
            nativeDeviceBuildPerformed = false
        }, true));
    }

    [Serializable] class AdsConfig { public bool useTestAds; }
    [Serializable] class SettingsAudit {
        public string bundleId, backend, architecture, minimumIOS, sdk;
        public bool automaticSigning, developmentExport, testAds, teamConfigured;
    }
    [Serializable] class ExportAudit {
        public string outputPath, bundleId;
        public bool infoPlist, testAdApplicationId, adNetworkEntries, iconCatalog, automaticSigning, nativeAccessibility, nativeDeviceBuildPerformed;
    }
}
