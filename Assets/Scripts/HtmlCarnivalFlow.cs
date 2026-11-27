using UnityEngine;

namespace PinPop {
 public sealed partial class HtmlReferenceHUD {
  CarnivalPopup restartPopup,featureProgressPopup;
  void BuildCarnivalFlow(){
   var restart=Frame("Restart confirmation",root);views["restart"]=restart;
   restartPopup=CarnivalPopup.Create(restart,game,CarnivalPopup.ScreenKind.Restart,CancelCarnivalRestart,()=>owner.PlayLevel(game.Level));
   restartPopup.CloseButton.name="RESTART_CLOSE";
   var progress=Frame("Feature progress",root);views["featureProgress"]=progress;
   featureProgressPopup=CarnivalPopup.Create(progress,game,CarnivalPopup.ScreenKind.Progress,()=>{overlay="win";UpdateVisible();},()=>owner.PlayLevel(game.Level+1));
   featureProgressPopup.CloseButton.name="FEATURE_PROGRESS_CLOSE";featureProgressPopup.SetProgress(Mathf.Clamp01((game.Level+1)/6f));
  }
  void ShowCarnivalRestart(){if(game.InMenu||game.Finished||game.Transitioning)return;game.CancelPull();Time.timeScale=0;Overlay("restart");}
  public bool CancelRestartIfVisible(){if(overlay!="restart")return false;CancelCarnivalRestart();return true;}
  void CancelCarnivalRestart(){if(overlay!="restart")return;Time.timeScale=0;Overlay("settingsGame");}
  void ContinueCarnivalResult(){
   if(game.Transitioning)return;
   if(resultWin&&AdventureRules.FinalLevel(game.Level)){owner.ReturnHome();game.GetComponent<AdventureHUD>().ShowMap();return;}
   // Real progression: the band becomes playable in level 7. Replays after the
   // lesson was seen must never reopen an obsolete unlock teaser.
   if(resultWin&&game.Level<6&&game.HighestLevel<=6&&(game.Adventure.Data.lessons&(1<<7))==0){featureProgressPopup.SetProgress((game.Level+1)/6f);Time.timeScale=0;Overlay("featureProgress");return;}
   owner.PlayLevel(resultWin?game.Level+1:game.Level);
  }
 }
}
