using System;
using UnityEngine;
using UnityEngine.UI;
namespace PinPop {
 public sealed partial class AdventureHUD {
  CarnivalPopup OpenPolished(string title,float height=500){
   bool refresh=showing;if(showing)Close();game.CancelPull();Time.timeScale=0;showing=true;modalShade.gameObject.SetActive(true);
   carnivalLesson=CarnivalPopup.Custom(modalShade.transform.parent,game,title,height,Dismiss);modal=carnivalLesson.gameObject;carnivalLesson.CloseButton.name="ADVENTURE_CLOSE";if(refresh)carnivalLesson.Settle();return carnivalLesson;
  }
  void ShowPolishedLesson(string text){
   int split=text.IndexOf('\n');string title=split<0?text:text.Substring(0,split),body=split<0?"":text.Substring(split+1);var popup=OpenPolished(title,405);lesson=true;popup.Body.text=body;popup.Body.rectTransform.sizeDelta=new Vector2(274,86);popup.Title.fontSize=21;
   string canonical=null;foreach(string source in new[]{"SEKME ENGELLERİ","BUZLU DUBA","HAREKETLİ BARİYER"})if(title==source||title==GameLocalization.Text(source))canonical=source;
   if(canonical!=null){var at=CrispUI.Rect("Mechanic demonstration",popup.Content,22,212,460,190);at.localScale=Vector3.one*.6f;at.gameObject.AddComponent<LessonDemo>().Build(canonical,game);}
   else {popup.Plate("Character reveal rim",popup.Content,98,208,126,158,"D9BA77",24);popup.Plate("Character reveal surface",popup.Content,101,211,120,152,"FFF8E5",21);popup.Picture("Unlocked character",popup.Content,106,214,110,145,Resources.Load<Sprite>("PinIcons/pin-"+Mathf.Max(0,PinPopLayouts.ColorCount(game.Level)-1)));}
  }
  void ApplyHelper(int id,int color=-1,int token=-1){
   bool used;Time.timeScale=1;
   try{used=game.Adventure.Use(id,color,token);}finally{Time.timeScale=0;}
   if(used){if(id==0)game.PlayEntryChime();Close();}
  }
  void ShowPolishedBooster(int id){
   if(id<0||id>3||game.Busy||game.Finished||game.Transitioning||game.IsPulling)return;
   string[] descriptions={"Sıradaki topun rengini seç. Hamle harcamaz.\nMasada kalan renklerden birini seçebilirsin.","Bir sonraki top bütün renklere etki eder.\nDüşen dubalar kendi renkleriyle toplanır.","Bekleyen bir dubayı tek kişilik cebe taşı.\nAynı renkten iki duba gelince üçlü tamamlanır.","Mevcut masayı koruyarak üç hamle ekle.\nÖdüllü video hakkını tüketmez."};
   var popup=OpenPolished(GameLocalization.Text(AdventureRules.Boosters[id]).ToUpper(GameLocalization.Culture),CarnivalPopup.PurchasePanelHeight);popup.Body.text=descriptions[id];popup.Body.rectTransform.sizeDelta=new Vector2(274,77);
   popup.Plate("Helper stock border",popup.Content,62,178,198,34,"D9BA77",17);popup.Plate("Helper stock",popup.Content,64,180,194,30,"FFF8E5",15);popup.Copy("Helper inventory",popup.Content,65,179,192,32,GameLocalization.Format("Envanter: {0}",game.Adventure.Data.stock[id]),14);
   if(id==0){for(int c=0;c<PinPopLayouts.ColorCount(game.Level);c++){int color=c;float x=21+c%4*72,y=225+c/4*78;var button=popup.Choice("COLOR_CHOICE_"+c,popup.Content,x,y,64,68,"",()=>ApplyHelper(id,color));popup.Picture("Color ball "+c,button.targetGraphic.transform,15,4,34,34,BallLooks.Icon(game.SelectedBall,c));var label=button.GetComponentInChildren<Text>();label.text=Art.Names[c];label.fontSize=11;label.rectTransform.anchoredPosition=new Vector2(2,-39);label.rectTransform.sizeDelta=new Vector2(60,25);button.interactable=game.Adventure.Data.stock[id]>0&&game.Pins.Exists(p=>p!=null&&!p.Cleared&&p.ColorId==color);}}
   else if(id==2){int n=0;if(game.Adventure.PocketColor<0)foreach(var item in game.MatchTray.Items){if(item.Matched||!item.Arrived)continue;int token=item.Id;float x=19+n%4*73,y=213+n/4*64;var button=popup.Choice("POCKET_CHOICE_"+token,popup.Content,x,y,66,61,"",()=>ApplyHelper(id,-1,token));popup.Picture("Pocket figure",button.targetGraphic.transform,15,2,36,54,Resources.Load<Sprite>("PinIcons/pin-"+item.Color));button.interactable=game.Adventure.PocketColor<0&&game.Adventure.Data.stock[id]>0;n++;}
    if(n==0||game.Adventure.PocketColor>=0){popup.Plate("Pocket empty state",popup.Content,24,236,274,113,"FFF8E5",15);CrispUI.Icon("Pocket symbol",popup.Content,140,248,42,42,CrispIcon.Kind.Pocket,Art.Hex("AE8B47"));popup.Copy("Pocket availability",popup.Content,34,296,254,46,game.Adventure.PocketColor>=0?"Cep dolu; iki eşini toplayarak boşalt.":"Önce tepsiye bir duba topla.",13,false);}
   }else {popup.Plate("Helper symbol border",popup.Content,124,230,74,74,"D9BA77",37);popup.Plate("Helper symbol surface",popup.Content,127,233,68,68,"FFF8E5",34);CrispUI.Icon("Helper symbol",popup.Content,140,246,42,42,(CrispIcon.Kind)id,Art.Hex("8766AC"));popup.Choice("BOOSTER_USE",popup.Content,38,324,246,61,"KULLAN",()=>ApplyHelper(id),true).interactable=game.Adventure.Data.stock[id]>0&&(id!=1||!game.Adventure.RainbowArmed);}
   var buy=popup.PurchaseAction("BOOSTER_BUY",AdventureRules.Prices[id],()=>{if(game.Adventure.Buy(id)){ShowPolishedBooster(id);game.Hud.Toast(GameLocalization.Format("Envantere +1 {0}",AdventureRules.Boosters[id]),2);}});buy.interactable=game.Coins>=AdventureRules.Prices[id]&&game.Adventure.Data.stock[id]<999;
   if(game.Adventure.Data.stock[id]>=999)buy.GetComponent<CarnivalPurchaseButton>().SetOffer(AdventureRules.Prices[id],"LİMİTE ULAŞILDI");

  }
 }
}
