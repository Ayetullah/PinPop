using System;
using System.Runtime.InteropServices;
using UnityEngine;
namespace PinPop {
 // Platform accessibility preference only; there is no in-game motion setting.
 public static class HtmlPlatformPreferences {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
  [DllImport("/usr/lib/libobjc.A.dylib")] static extern IntPtr objc_getClass(string name);
  [DllImport("/usr/lib/libobjc.A.dylib")] static extern IntPtr sel_registerName(string name);
  [DllImport("/usr/lib/libobjc.A.dylib",EntryPoint="objc_msgSend")] static extern IntPtr Send(IntPtr target,IntPtr selector);
  [DllImport("/usr/lib/libobjc.A.dylib",EntryPoint="objc_msgSend")][return:MarshalAs(UnmanagedType.I1)] static extern bool SendBool(IntPtr target,IntPtr selector);
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
  [DllImport("user32.dll",SetLastError=true)] static extern bool SystemParametersInfo(uint action,uint parameter,out bool enabled,uint flags);
#elif UNITY_IOS && !UNITY_EDITOR
  [DllImport("__Internal")] static extern int PinPopReducedMotion();
#endif
  public static bool ReduceMotion(){try{
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
   var cls=objc_getClass("NSWorkspace");if(cls==IntPtr.Zero)return false;var workspace=Send(cls,sel_registerName("sharedWorkspace"));return workspace!=IntPtr.Zero&&SendBool(workspace,sel_registerName("accessibilityDisplayShouldReduceMotion"));
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
   return SystemParametersInfo(0x1042,0,out bool animations,0)&&!animations;
#elif UNITY_IOS && !UNITY_EDITOR
   return PinPopReducedMotion()!=0;
#elif UNITY_ANDROID && !UNITY_EDITOR
   using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var resolver=activity.Call<AndroidJavaObject>("getContentResolver"))using(var settings=new AndroidJavaClass("android.provider.Settings$Global"))return settings.CallStatic<float>("getFloat",resolver,"animator_duration_scale",1f)==0;
#else
   return false;
#endif
  }catch(DllNotFoundException){return false;}catch(EntryPointNotFoundException){return false;}
#if UNITY_ANDROID && !UNITY_EDITOR
   catch(AndroidJavaException){return false;}
#endif
  }
 }
}
