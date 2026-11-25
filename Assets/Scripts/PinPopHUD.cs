using System.Collections;
using UnityEngine;
namespace PinPop {
 public sealed class PinPopHUD:MonoBehaviour {
  public const string TutorialSeenKey="PinPop.HowTo.Seen.v1";
  PinPopGame game;HtmlReferenceHUD view;string helpReturnOverlay="";
  public Camera UICamera=>view.UICamera;
  public bool HelpVisible=>view.HelpVisible;public bool ResultVisible=>view.ResultVisible;public bool HomeVisible=>view.HomeVisible;public bool SwapShopVisible=>view.SwapShopVisible;public bool ShopVisible=>view.ShopVisible;public bool SettingsVisible=>view.SettingsVisible;
  public int GetPreviewColor(int index)=>view.NextColors[index];
  public void Build(PinPopGame owner){game=owner;view=gameObject.AddComponent<HtmlReferenceHUD>();view.Build(this,owner);}
  public IEnumerator BuildAsync(PinPopGame owner){game=owner;view=gameObject.AddComponent<HtmlReferenceHUD>();yield return view.BuildAsync(this,owner);}
  public void Refresh(){view.Refresh();}public void SetPower(float value){view.SetPower(value);}
  public void ShowHome(){helpReturnOverlay="";view.Home(true);view.Refresh();}public void ShowGame(){helpReturnOverlay="";view.Home(false);view.Refresh();}
  public void ShowSettings(){if(game.TrayOverflow||game.Transitioning||game.Finished||view.ResultVisible||view.ShopVisible||view.SwapShopVisible||view.HelpVisible)return;game.CancelPull();Time.timeScale=0;view.Overlay(game.InMenu?"settingsHome":"settingsGame");}
  public void ShowShop(){if(game.Transitioning||game.Busy||game.Finished||view.ResultVisible||view.SettingsVisible||view.SwapShopVisible||view.HelpVisible)return;game.CancelPull();Time.timeScale=0;view.Overlay("shop");}
  public bool TryShowFirstPlayHelp(){
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(DevicePolishReview.Active)return false;
#endif
if(game.Level!=0||game.InMenu||game.Transitioning||game.Finished||PlayerPrefs.GetInt(TutorialSeenKey,0)==1)return false;if(game.HighestLevel>0){PlayerPrefs.SetInt(TutorialSeenKey,1);PlayerPrefs.Save();return false;}ShowHelp();if(!view.HelpVisible)return false;PlayerPrefs.SetInt(TutorialSeenKey,1);PlayerPrefs.Save();return true;}
  public void ShowHelp(){if(game.Transitioning||game.Finished||view.ResultVisible||view.ShopVisible||view.SwapShopVisible||view.HelpVisible)return;helpReturnOverlay=view.SettingsVisible?(game.InMenu?"settingsHome":"settingsGame"):"";game.CancelPull();Time.timeScale=0;view.Overlay("help");}
  public void ShowSwapShop(){if(game.InMenu||game.Transitioning||game.Busy||game.Finished||view.ResultVisible||view.SettingsVisible||view.ShopVisible||view.HelpVisible||view.SwapShopVisible)return;game.CancelPull();Time.timeScale=0;view.Overlay("swapBuy");}
  public void CloseOverlays(){if(game.Transitioning||view.ResultVisible)return;if(view.HelpVisible&&!string.IsNullOrEmpty(helpReturnOverlay)){string back=helpReturnOverlay;helpReturnOverlay="";Time.timeScale=0;view.Overlay(back);}else{helpReturnOverlay="";Time.timeScale=1;view.Overlay("");}if(UnityEngine.EventSystems.EventSystem.current!=null)UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);}
  public void ReturnHome(){if(game.Transitioning||game.RewardedVideo!=null&&game.RewardedVideo.Busy)return;CloseOverlays();int next=game.Won?game.Level+1:game.Level;game.SetMenu(true);game.LoadLevel(next);ShowHome();}
  public void PlayLevel(int level){if(!game.Transitioning&&(game.RewardedVideo==null||!game.RewardedVideo.Busy))StartCoroutine(Load(AdventureRules.ClampLevel(level)));}
  IEnumerator Load(int level)
  {
   game.CancelPull();game.StopAllCoroutines();Time.timeScale=0;game.Transitioning=true;view.Curtain(true,level);
   // Present the curtain before building the next board. Reduced motion changes animation, not readability.
   yield return new WaitForEndOfFrame();
   float presentedAt=Time.realtimeSinceStartup;
   yield return new WaitForSecondsRealtime(.24f);
   yield return game.LoadLevelAsync(level);game.SetMenu(false);view.Home(false);
   while(Time.realtimeSinceStartup-presentedAt<BowlingTransition.Duration)yield return null;
   view.Curtain(false);game.Transitioning=false;
   if(UnityEngine.EventSystems.EventSystem.current!=null)UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);game.AnimateEntrance();game.PlayEntryChime();view.EntryBadge();view.Refresh();TryShowFirstPlayHelp();
  }
  public void RescueEffect(){view.RescueEffect();}
  public void RecoveryMessage(string text){view.RecoveryMessage(text);}
  public void CountEffect(){view.CountEffect();}public void PurchaseEffect(){view.PurchaseEffect();}
  public void ShowResult(bool won){view.Result(won);}public void HideResult(){view.HideResult();}public void AnimateQueue(){view.PopQueue();}public void Combo(string text){view.Banner(text);}public void Toast(string text,float duration=1.5f){view.Toast(text,duration);}
 }
}
