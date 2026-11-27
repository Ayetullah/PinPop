#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace PinPop {
 public sealed partial class CarnivalUIReview {
  IEnumerator HelperUseChecks(){
   var adventure=game.GetComponent<AdventureHUD>();
   foreach(string language in new[]{"tr","en"})foreach(var size in new[]{new Vector2Int(320,568),new Vector2Int(390,844),new Vector2Int(1170,2532)}){
    GameLocalization.SetPreference(language);Size(size);yield return new WaitForSecondsRealtime(.25f);
    for(int id=0;id<4;id++){
     PlayerPrefs.SetInt("Level",0);new PinPopEconomy(new EconomySave{coins=500,swaps=3,highestLevel=40}).Save();game.ReloadEconomy();var data=new AdventureSave{lessons=511};data.stock[id]=0;PlayerPrefs.SetString(PinPopAdventure.SaveKey,JsonUtility.ToJson(data));game.Adventure.Reload();Board(11);yield return null;
     int moves=game.Moves,attempt=game.AttemptId;
     if(id==2)game.MatchTray.Items.Add(new PinPopMatchTray.Item{Id=100,Color=1,Arrived=true,Presented=true,Age=1});
     adventure.ShowBooster(id);yield return new WaitForSecondsRealtime(.15f);Click("BOOSTER_BUY");yield return null;
     Check(adventure.ModalVisible&&game.Paused&&game.Adventure.Data.stock[id]==1&&game.Coins==500-AdventureRules.Prices[id],"Purchase leaves helper popup open with exactly one item: "+id+" / "+language+" / "+size);
     Click(id==0?"COLOR_CHOICE_0":id==2?"POCKET_CHOICE_100":"BOOSTER_USE");
     Check(!adventure.ModalVisible&&!game.Paused&&GameObject.Find("ADVENTURE_CLOSE")==null,"Successful use closes helper popup and resumes gameplay: "+id);
     Check(game.Adventure.Data.stock[id]==0&&game.Coins==500-AdventureRules.Prices[id]&&game.AttemptId==attempt&&game.Moves==moves+(id==3?3:0),"One purchased helper consumed once; no extra charge or turn reset: "+id);
     Check(id==0?game.Queue[0]==0:id==1?game.Adventure.RainbowArmed:id==2?game.Adventure.PocketColor==1:game.Moves==moves+3,"Requested helper effect applied: "+id);
     // With no stock, even a stale/direct request must keep the modal paused.
     adventure.ShowBooster(id);yield return null;typeof(AdventureHUD).GetMethod("ApplyHelper",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(adventure,new object[]{id,0,100});
     Check(adventure.ModalVisible&&game.Paused&&game.Adventure.Data.stock[id]==0&&game.Moves==moves+(id==3?3:0),"Failed use leaves helper popup open without consuming resources: "+id);Click("ADVENTURE_CLOSE");Check(!adventure.ModalVisible&&!game.Paused,"Cross still cancels unused helper popup: "+id);
    }
   }
  }
  IEnumerator AllPopupChecks(){
   string economy=PlayerPrefs.GetString(PinPopEconomy.SaveKey),adventureData=PlayerPrefs.GetString(PinPopAdventure.SaveKey);int levelPref=PlayerPrefs.GetInt("Level");
   PlayerPrefs.SetInt("Level",0);new PinPopEconomy(new EconomySave{coins=2000,swaps=3,highestLevel=40}).Save();game.ReloadEconomy();var adventure=game.GetComponent<AdventureHUD>();
   foreach(string language in new[]{"tr","en"})foreach(var size in new[]{new Vector2Int(320,568),new Vector2Int(390,844),new Vector2Int(1170,2532)}){
    GameLocalization.SetPreference(language);Size(size);yield return new WaitForSecondsRealtime(.25f);PlayerPrefs.SetString(PinPopAdventure.SaveKey,JsonUtility.ToJson(new AdventureSave{lessons=511}));game.Adventure.Reload();Board(11);yield return null;
    string suffix=language+"-"+size.x;
    game.SetMenu(true);game.Hud.ShowHome();game.Hud.ShowSettings();yield return new WaitForSecondsRealtime(.25f);Inspect("Settings");Capture("settings-home-"+suffix);Click("HELP_SETTINGS");yield return new WaitForSecondsRealtime(.2f);Click("HELP_CLOSE");Check(game.Hud.SettingsVisible&&game.InMenu&&game.Paused,"Home tutorial returns to paused home settings");yield return new WaitForSecondsRealtime(.25f);Click("CLOSE_SETTINGS");Check(game.InMenu&&!game.Paused,"Home settings close preserves menu");game.SetMenu(false);game.Hud.ShowGame();
    int coins=game.Coins,moves=game.Moves,swaps=game.SwapCredits;
    game.Hud.ShowShop();yield return new WaitForSecondsRealtime(.3f);Inspect("Custom");Capture("shop-"+suffix);
    if(!game.OwnsBall(1)){Click("BALL_BUY_1");Check(game.OwnsBall(1)&&game.SelectedBall==1&&game.Coins==coins-PinPopEconomy.BallPrices[1],"Shop buy charges exactly one listed price and equips style");coins=game.Coins;}
    Click("BALL_BUY_0");Check(game.SelectedBall==0&&game.Coins==coins,"Owned ball selection is free");Click("CLOSE_SHOP");Check(!game.Hud.ShopVisible&&!game.Paused&&game.Moves==moves,"Shop close preserves ready turn");
    game.Hud.ShowSwapShop();yield return new WaitForSecondsRealtime(.3f);Inspect("Custom");Capture("swap-"+suffix);Click("SWAP_BUY_CONFIRM");Check(game.SwapCredits==swaps+1&&game.Coins==coins-10&&game.Moves==moves,"Swap popup purchases one credit without spending a move");coins=game.Coins;Check(game.Hud.SwapShopVisible&&game.Paused,"Swap purchase keeps popup open until cross");Click("SWAP_BUY_CANCEL");Check(!game.Hud.SwapShopVisible&&!game.Paused,"Swap close resumes play");
    for(int id=0;id<4;id++){
     Board(11);moves=game.Moves;int stock=game.Adventure.Data.stock[id];
     if(id==2)for(int i=0;i<9;i++)game.MatchTray.Items.Add(new PinPopMatchTray.Item{Id=100+i,Color=i%8,Arrived=true,Presented=true,Age=1});
     adventure.ShowBooster(id);yield return new WaitForSecondsRealtime(.25f);Inspect("Custom");Capture("helper-"+id+"-"+suffix);
     if(id==0){Click("COLOR_CHOICE_0");Check(!adventure.ModalVisible&&!game.Paused&&game.Queue[0]==0&&game.Adventure.Data.stock[id]==stock-1&&game.Moves==moves,"Color picker uses one helper, preserves moves");}
     else if(id==2){Click("POCKET_CHOICE_100");Check(!adventure.ModalVisible&&!game.Paused&&game.Adventure.PocketColor==0&&game.Adventure.Data.stock[id]==stock-1&&game.Moves==moves,"Pocket choice extracts requested figure, spends one helper");}
     else {Click("BOOSTER_USE");Check(!adventure.ModalVisible&&!game.Paused&&game.Adventure.Data.stock[id]==stock-1&&game.Moves==moves+(id==3?3:0)&&(id!=1||game.Adventure.RainbowArmed),"Helper use applies once: "+id);}
     yield return null;Check(!adventure.ModalVisible&&!game.Paused,"Successful helper use closes popup and resumes gameplay");
    }
    Board(11);int beforeStock=game.Adventure.Data.stock[3];adventure.ShowBooster(3);yield return null;Click("BOOSTER_BUY");yield return new WaitForSecondsRealtime(.2f);Check(game.Adventure.Data.stock[3]==beforeStock+1&&game.Coins==coins-AdventureRules.Prices[3],"Helper buy updates stock and charges once");coins=game.Coins;Click("ADVENTURE_CLOSE");
    adventure.ShowBooster(2);yield return new WaitForSecondsRealtime(.2f);Inspect("Custom");Capture("pocket-empty-"+suffix);Click("ADVENTURE_CLOSE");
    string[] lessonTitles={"SEKME ENGELLERİ","BUZLU DUBA","BAĞLI ÜÇLÜ","HAREKETLİ BARİYER",GameLocalization.Format("{0} AÇILDI",Art.Names[3])};string[] lessonBodies={"Topu ön kenarda sağa–sola kaydır. Engellerin yanından açı bularak at.","Aynı renkte bir top çarptığında buz kırılır.\nDuba sonraki atışta hareket edebilir.","Aynı renkte bir top bağı koparır.\nÜç duba sonraki atışta serbest kalır.","Geçit açılana kadar bekle, ardından atışını bırak.","Toplanan üç aynı figür birleşip silinir."};
    for(int i=0;i<lessonTitles.Length;i++){if(i==4)Board(2);typeof(AdventureHUD).GetMethod("PresentLesson",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(adventure,new object[]{lessonTitles[i]+"\n"+lessonBodies[i]});yield return new WaitForSecondsRealtime(.25f);Inspect(i==2?"Unlock":"Custom");Capture("lesson-"+i+"-"+suffix);Click("ADVENTURE_CLOSE");Check(!adventure.ModalVisible&&!game.Paused,"Lesson close resumes unmodified board: "+i);}
    Board(11);game.Hud.ShowHelp();yield return new WaitForSecondsRealtime(.2f);var help=FindFirstObjectByType<AnimatedHowTo>();for(int step=0;step<3;step++){help.Select(step);yield return new WaitForSecondsRealtime(.2f);Inspect("Custom");Capture("howto-"+step+"-"+suffix);}typeof(AnimatedHowTo).GetField("started",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(help,Time.unscaledTime-help.Duration);yield return null;yield return null;Check(game.Hud.HelpVisible&&help.Step==2,"Tutorial last page loops without automatic dismissal");var swipe=new UnityEngine.EventSystems.PointerEventData(events){position=new Vector2(200,200),pointerId=-1,button=UnityEngine.EventSystems.PointerEventData.InputButton.Left};help.OnPointerDown(swipe);swipe.position=new Vector2(50,200);help.OnPointerUp(swipe);Check(game.Hud.HelpVisible&&help.Step==2,"Final tutorial swipe does not dismiss popup");Click("HELP_CLOSE");Check(!game.Hud.HelpVisible&&!game.Paused,"Animated tutorial closes to gameplay");
    adventure.ShowMap();yield return new WaitForSecondsRealtime(.3f);Capture("map-"+suffix);Check(FindFirstObjectByType<CarnivalMapView>()!=null,"Map overlay retains scrolling region navigation");Click("ADVENTURE_CLOSE");Check(!adventure.ModalVisible&&!game.Paused,"Map cross resumes gameplay");
    Board(3);typeof(PinPopGame).GetProperty("Moves").SetValue(game,0);game.SendMessage("End",false);yield return new WaitForSecondsRealtime(.2f);Click("WATCH_VIDEO");yield return new WaitForSecondsRealtime(.2f);Inspect("Custom");Capture("video-preparing-"+suffix);Click("Cancel rewarded video");Check(!game.RewardedVideo.Busy&&game.Finished&&!game.VideoRescueUsed&&game.Coins==coins&&game.Moves==0,"Video preparation cross cancels without a reward");game.Hud.HideResult();
   }
   Board(3);typeof(PinPopGame).GetProperty("Moves").SetValue(game,0);game.SendMessage("End",false);yield return null;Click("WATCH_VIDEO");typeof(PinPopRewardedVideo).GetMethod("Finish",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game.RewardedVideo,new object[]{true,""});yield return new WaitForSecondsRealtime(.3f);Check(game.RewardedVideo.Busy&&game.Finished&&game.Moves==0&&GameObject.Find("Cancel rewarded video")!=null,"Video completion waits for cross before dismissing/granting moves");Capture("video-completed-cross-only");Click("Cancel rewarded video");Check(!game.RewardedVideo.Busy&&!game.Finished&&game.Moves==3&&game.VideoRescueUsed,"Completed video cross grants the pending reward exactly once");
   // Insufficient funds/stock are visible states, never silent actionable offers.
   Board(11);new PinPopEconomy(new EconomySave{coins=0,swaps=999,highestLevel=40}).Save();game.ReloadEconomy();game.Hud.ShowShop();yield return null;Check(!GameObject.Find("BALL_BUY_1").GetComponent<Button>().interactable,"Unowned style is disabled at zero balance");Capture("shop-insufficient");game.Hud.CloseOverlays();game.Hud.ShowSwapShop();yield return null;Check(!GameObject.Find("SWAP_BUY_CONFIRM").GetComponent<Button>().interactable,"Full swap inventory blocks purchase");Capture("swap-full");game.Hud.CloseOverlays();game.Adventure.Data.stock[1]=0;game.Adventure.Save();adventure.ShowBooster(1);yield return null;Check(!GameObject.Find("BOOSTER_USE").GetComponent<Button>().interactable&&!GameObject.Find("BOOSTER_BUY").GetComponent<Button>().interactable,"Empty helper and zero coins disable both actions");Capture("helper-empty");adventure.Close();
   Board(11);CrispUI.ReviewReducedMotion=false;game.Hud.ShowShop();var shell=FindPopup("Custom");var entrance=shell.Stage.Find("Popup entrance").GetComponent<CanvasGroup>();float entranceBefore=entrance.alpha;yield return new WaitForSecondsRealtime(.7f);Check(entranceBefore<1&&entrance.alpha>=.99f&&game.Paused,"Shared popup entrance advances while physics is paused");Capture("shop-normal-motion");game.Hud.CloseOverlays();
   game.Hud.ShowHelp();yield return new WaitForSecondsRealtime(.7f);var animated=FindFirstObjectByType<AnimatedHowTo>();float demoBefore=animated.DemoTime;var touch=GameObject.Find("Demo touch").GetComponent<RectTransform>();var touchBefore=touch.anchoredPosition;int movesBefore=game.Moves,coinsBefore=game.Coins;yield return new WaitForSecondsRealtime(.6f);Check(animated.DemoTime>demoBefore&&touch.anchoredPosition!=touchBefore&&game.Paused,"Tutorial gesture animation advances inside polished paused popup");Check(game.Moves==movesBefore&&game.Coins==coinsBefore,"Tutorial animation cannot spend live moves or coins");Capture("howto-normal-motion");game.Hud.CloseOverlays();CrispUI.ReviewReducedMotion=true;
   PlayerPrefs.SetString(PinPopEconomy.SaveKey,economy);PlayerPrefs.SetString(PinPopAdventure.SaveKey,adventureData);PlayerPrefs.SetInt("Level",levelPref);game.ReloadEconomy();game.Adventure.Reload();game.Hud.HideResult();game.Hud.ShowGame();
   Check(PlayerPrefs.GetString(PinPopEconomy.SaveKey)==economy&&PlayerPrefs.GetString(PinPopAdventure.SaveKey)==adventureData,"Expanded popup checks restore their temporary wallet and helper changes");
  }
 }
}
#endif
