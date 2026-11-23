using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;

namespace PinPop {
 // Official Google Mobile Ads 11.7.0 API. Only Android/iOS initialize the network.
 public sealed class AdMobRewardedProvider:MonoBehaviour {
  [Serializable] sealed class Config {public bool useTestAds=true;public string androidRewardedUnit="",iosRewardedUnit="";}
  RewardedAd ad,activeAd;Config config;bool initialized,initializing,consentUpdating,loading,showing;float loadedAt;int generation;
  public bool Ready=>ad!=null&&ad.CanShowAd()&&Time.realtimeSinceStartup-loadedAt<3300;
  public static bool PrivacyRequired=>ConsentInformation.PrivacyOptionsRequirementStatus==PrivacyOptionsRequirementStatus.Required;
  public void Initialize(){
   if(initialized||initializing||consentUpdating)return;consentUpdating=true;
   var asset=Resources.Load<TextAsset>("AdMobConfig");config=asset==null?new Config():JsonUtility.FromJson<Config>(asset.text);
   MobileAdsEventExecutor.Initialize();
   ConsentInformation.Update(new ConsentRequestParameters(),error=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{
    if(this==null)return;if(error!=null){consentUpdating=false;if(ConsentInformation.CanRequestAds())InitializeSdk();return;}
    ConsentForm.LoadAndShowConsentFormIfRequired(formError=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{if(this==null)return;consentUpdating=false;if(ConsentInformation.CanRequestAds())InitializeSdk();}));
   }));
  }
  void InitializeSdk(){if(initialized||initializing)return;initializing=true;MobileAds.Initialize(status=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{if(this==null)return;initializing=false;initialized=status!=null;if(initialized)Load();}));}
  public void Load(){
   if(!initialized){Initialize();return;}if(loading||showing||Ready)return;ad?.Destroy();ad=null;
#if UNITY_ANDROID
   string unit=config.useTestAds?"ca-app-pub-3940256099942544/5224354917":config.androidRewardedUnit;
#else
   string unit=config.useTestAds?"ca-app-pub-3940256099942544/1712485313":config.iosRewardedUnit;
#endif
   if(string.IsNullOrWhiteSpace(unit))return;loading=true;int token=++generation;
   RewardedAd.Load(unit,new AdRequest(),(loaded,error)=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{loading=false;if(this==null||token!=generation){loaded?.Destroy();return;}if(error!=null||loaded==null){loaded?.Destroy();return;}ad=loaded;loadedAt=Time.realtimeSinceStartup;}));
  }
  public bool Show(Action<bool> completed){
   if(!Ready||showing)return false;showing=true;bool earned=false,settled=false;var current=ad;activeAd=current;ad=null;
   Action<bool> finish=ok=>{if(settled)return;settled=true;showing=false;activeAd=null;current.Destroy();if(this==null)return;completed(ok);Load();};
   current.OnAdFullScreenContentClosed+=()=>MobileAdsEventExecutor.ExecuteInUpdate(()=>finish(earned));
   current.OnAdFullScreenContentFailed+=error=>MobileAdsEventExecutor.ExecuteInUpdate(()=>finish(false));
   current.Show(reward=>MobileAdsEventExecutor.ExecuteInUpdate(()=>earned=true));return true;
  }
  public void SetMuted(bool muted){MobileAds.SetApplicationMuted(muted);}
  public void PrivacyOptions(){ConsentForm.ShowPrivacyOptionsForm(error=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{if(ConsentInformation.CanRequestAds())Load();}));}
  void OnDestroy(){generation++;ad?.Destroy();activeAd?.Destroy();}
 }
}
