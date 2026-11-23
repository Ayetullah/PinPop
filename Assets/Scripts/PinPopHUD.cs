using System.Collections;
using UnityEngine;
namespace PinPop {
 public sealed class PinPopHUD:MonoBehaviour {
  public const string TutorialSeenKey="PinPop.HowTo.Seen.v1";
  PinPopGame game;HtmlReferenceHUD view;
  public Camera UICamera=>view.UICamera;
  public bool HelpVisible=>view.HelpVisible;public bool ResultVisible=>view.ResultVisible;public bool HomeVisible=>view.HomeVisible;public bool SwapShopVisible=>view.SwapShopVisible;public bool ShopVisible=>view.ShopVisible;public bool SettingsVisible=>view.SettingsVisible;
  public int GetPreviewColor(int index)=>view.NextColors[index];
  public void Build(PinPopGame owner){game=owner;view=gameObject.AddComponent<HtmlReferenceHUD>();view.Build(this,owner);}
  public void Refresh(){view.Refresh();}public void SetPower(float value){view.SetPower(value);}
  public void ShowHome(){view.Home(true);view.Refresh();}public void ShowGame(){view.Home(false);view.Refresh();}
  public void ShowSettings(){if(game.Transitioning||game.Finished||view.ResultVisible||view.ShopVisible||view.SwapShopVisible||view.HelpVisible)return;game.CancelPull();Time.timeScale=0;view.Overlay(game.InMenu?"settingsHome":"settingsGame");}
  public void ShowShop(){if(game.Transitioning||game.Busy||game.Finished||view.ResultVisible||view.SettingsVisible||view.SwapShopVisible||view.HelpVisible)return;game.CancelPull();Time.timeScale=0;view.Overlay("shop");}
  public bool TryShowFirstPlayHelp(){if(game.Level!=0||game.InMenu||game.Transitioning||game.Finished||PlayerPrefs.GetInt(TutorialSeenKey,0)==1)return false;if(game.HighestLevel>0){PlayerPrefs.SetInt(TutorialSeenKey,1);PlayerPrefs.Save();return false;}ShowHelp();if(!view.HelpVisible)return false;PlayerPrefs.SetInt(TutorialSeenKey,1);PlayerPrefs.Save();return true;}
  public void ShowHelp(){if(game.Level!=0||game.Transitioning||game.Finished||view.ResultVisible||view.SettingsVisible||view.ShopVisible||view.SwapShopVisible||view.HelpVisible)return;game.CancelPull();Time.timeScale=0;view.Overlay("help");}
  public void ShowSwapShop(){if(game.InMenu||game.Transitioning||game.Busy||game.Finished||view.ResultVisible||view.SettingsVisible||view.ShopVisible||view.HelpVisible||view.SwapShopVisible)return;game.CancelPull();Time.timeScale=0;view.Overlay("swapBuy");}
  public void CloseOverlays(){if(game.Transitioning||view.ResultVisible)return;Time.timeScale=1;view.Overlay("");if(UnityEngine.EventSystems.EventSystem.current!=null)UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);}
  public void ReturnHome(){if(game.Transitioning||game.RewardedVideo!=null&&game.RewardedVideo.Busy)return;CloseOverlays();int next=game.Won?game.Level+1:game.Level;game.SetMenu(true);game.LoadLevel(next);ShowHome();}
  public void PlayLevel(int level){if(!game.Transitioning&&(game.RewardedVideo==null||!game.RewardedVideo.Busy))StartCoroutine(Load(AdventureRules.ClampLevel(level)));}
  IEnumerator Load(int level)
  {
   game.CancelPull();game.StopAllCoroutines();Time.timeScale=0;game.Transitioning=true;view.Curtain(true,level);
   // Present the curtain before building the next board. Reduced motion changes animation, not readability.
   yield return new WaitForEndOfFrame();
   float presentedAt=Time.realtimeSinceStartup;
   yield return new WaitForSecondsRealtime(.24f);
   game.LoadLevel(level);game.SetMenu(false);view.Home(false);
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
