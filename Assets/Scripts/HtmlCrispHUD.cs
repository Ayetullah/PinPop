using UnityEngine;
using UnityEngine.UI;

namespace PinPop {
 public sealed partial class HtmlReferenceHUD {
  bool crispReady;bool settingsPrivacyVisible;readonly int[] shownStock={-1,-1,-1,-1};float unit,logicalHeight,safeTop,safeBottom,controlTop;
  RectTransform nativeGame,nativeHome;Text winDetail,winChest;readonly Text[] boosterCounts=new Text[4];readonly Button[] boosterButtons=new Button[4];RectTransform firstQueueFrame;CrispShape swapStockBadge;Text swapStockLabel;int shownSwapStock=-1;
  public bool CrispReady=>crispReady;
  public float GameplayWidthFraction=>.92f;
  public float GameplayProjectionOffsetX=>0;
  RectTransform NativeRoot(string name,Transform parent){var r=CrispUI.Rect(name,parent,AppRect.x,CrispUI.Height-AppRect.yMax,AppRect.width,AppRect.height);return r;}
  RectTransform NRect(string name,Transform p,float x,float y,float w,float h)=>CrispUI.Rect(name,p,x*unit,y*unit,w*unit,h*unit);
  CrispShape NPanel(string name,Transform p,float x,float y,float w,float h,string hex,float radius=12)=>CrispUI.Panel(name,p,x*unit,y*unit,w*unit,h*unit,Art.Hex(hex),radius*unit);
  Text NText(string name,Transform p,float x,float y,float w,float h,string value,float size,string hex="FFF4DB",bool bold=true)=>CrispUI.Text(name,p,x*unit,y*unit,w*unit,h*unit,value,size*unit,Art.Hex(hex),bold);
  CrispIcon NIcon(string name,Transform p,float x,float y,float size,CrispIcon.Kind kind,string hex="FFF4DB")=>CrispUI.Icon(name,p,x*unit,y*unit,size*unit,size*unit,kind,Art.Hex(hex));
  void HideOld(string view){foreach(Transform child in views[view])child.gameObject.SetActive(false);}
  Text NativeField(string key,Transform p,float x,float y,float w,float h,float size,string hex="FFF4DB"){if(fields.TryGetValue(key,out var old))old.gameObject.SetActive(false);var t=NText(key.Substring(key.IndexOf('/')+1),p,x,y,w,h,"",size,hex);fields[key]=t;return t;}
  Button NButton(string name,Transform p,float x,float y,float w,float h,string label,UnityEngine.Events.UnityAction click,string hex="517D65",CrispIcon.Kind? icon=null){NPanel(name+" shadow",p,x,y+3,w,h,"203E4A",10);var art=NPanel(name,p,x,y,w,h,hex,10);art.raycastTarget=true;GameObject hit=art.gameObject;{float hw=Mathf.Max(54,w),hh=Mathf.Max(54,h);var hr=NRect(name,p,x-(hw-w)/2,y-(hh-h)/2,hw,hh);hit=hr.gameObject;var image=hit.AddComponent<Image>();image.color=Color.clear;image.raycastTarget=true;art.name=name+" artwork";art.transform.SetParent(hr,false);art.rectTransform.anchoredPosition=new Vector2((hw-w)/2,-(hh-h)/2)*unit;}var b=hit.AddComponent<Button>();b.targetGraphic=art;var colors=b.colors;colors.highlightedColor=Color.white;colors.pressedColor=new Color(.78f,.88f,.9f);colors.disabledColor=new Color(.65f,.7f,.74f,1);b.colors=colors;b.onClick.AddListener(click);if(icon.HasValue)NIcon(name+" icon",art.transform,8,(h-26)/2,26,icon.Value);var t=NText(name+" label",art.transform,icon.HasValue?38:5,0,w-(icon.HasValue?43:10),h,label,17);b.gameObject.AddComponent<HtmlButtonFeedback>().Artwork=art.rectTransform;return b;}
  Button ReplaceButton(string view,string id,Transform p,float x,float y,float w,float h,string label,string hex="517D65",CrispIcon.Kind? icon=null){string key=view+"/"+id;if(buttons.TryGetValue(key,out var old))old.gameObject.SetActive(false);var b=NButton(Names.ContainsKey(id)?Names[id]:id,p,x,y,w,h,label,()=>Act(id),hex,icon);buttons[key]=b;buttonImages[key]=(Image)b.targetGraphic;if(!buttonSpecs.ContainsKey(key))buttonSpecs[key]=new ButtonData{id=id,rect=new RectData()};fields[key]=b.GetComponentInChildren<Text>();return b;}
  System.Collections.IEnumerator BuildCrispInterfaceRoutine(bool spread){
   settingsPrivacyVisible=game.RewardedVideo!=null&&game.RewardedVideo.PrivacyRequired;shownSwapStock=-1;for(int i=0;i<4;i++)shownStock[i]=-1;unit=AppRect.width/390f;logicalHeight=AppRect.height/unit;safeTop=Mathf.Max(12,(CrispUI.Height-CrispUI.SafeArea.yMax)/unit);safeBottom=Mathf.Max(12,CrispUI.SafeArea.yMin/unit);controlTop=logicalHeight-safeBottom-76;
   foreach(string id in new[]{"home","game","win","settingsHome","settingsGame"})HideOld(id);
   nativeHome=NativeRoot("Crisp home",views["home"]);nativeGame=NativeRoot("Crisp game",views["game"]);
   StartupTiming.Begin("BuildNativeHome");BuildNativeHome();StartupTiming.End();if(spread)yield return null;StartupTiming.Begin("BuildNativeGame");BuildNativeGame();StartupTiming.End();if(spread)yield return null;StartupTiming.Begin("BuildNativeWin");BuildNativeWin();StartupTiming.End();if(spread)yield return null;StartupTiming.Begin("BuildNativeSettings(settingsHome)");BuildNativeSettings("settingsHome");StartupTiming.End();if(spread)yield return null;StartupTiming.Begin("BuildNativeSettings(settingsGame)");BuildNativeSettings("settingsGame");StartupTiming.End();
   // Shared overlays keep live text and use the same smooth panel edges.
   foreach(var image in root.GetComponentsInChildren<Image>(true))if(image.gameObject.name=="HTML raster")image.gameObject.SetActive(false);
   if(walletImages.TryGetValue("game/coins",out var wallet))wallet.gameObject.SetActive(false);foreach(string key in new[]{"home/homeCoins","home/homeSwaps"})if(walletImages.TryGetValue(key,out var w))w.gameObject.SetActive(false);
   if(nextGroup!=null)nextGroup.gameObject.SetActive(false);
   // The UI is composed by a separate camera after all scene effects.
   UICamera.allowMSAA=true;crispReady=true;RefreshCrispUI();
  }
  void BuildNativeHome(){
   NPanel("Home coin plate",nativeHome,14,safeTop+3,92,38,"FFF1D5",19);NIcon("Home coin",nativeHome,19,safeTop+8,28,CrispIcon.Kind.Coin);var homeCoins=NativeField("home/homeCoins",nativeHome,49,safeTop+3,49,38,18,"294B61");homeCoins.resizeTextForBestFit=true;homeCoins.resizeTextMinSize=Mathf.RoundToInt(12*unit);homeCoins.resizeTextMaxSize=Mathf.RoundToInt(18*unit);
   var settings=ReplaceButton("home","homeSettings",nativeHome,332,safeTop,44,44,"","FFF1D5",CrispIcon.Kind.Gear);
   ((CrispShape)settings.targetGraphic).Radius=22*unit;var gear=settings.GetComponentInChildren<CrispIcon>();gear.color=Art.Hex("36596A");gear.rectTransform.anchoredPosition=new Vector2(9,-9)*unit;
   var shadow=nativeHome.Find("HOME_SETTINGS shadow").GetComponent<CrispShape>();shadow.Radius=22*unit;shadow.color=new Color(.18f,.29f,.26f,.12f);
   NText("Logo shadow",nativeHome,16,safeTop+72,358,67,"PINPOP",58,"294B61");NText("Logo",nativeHome,14,safeTop+67,358,67,"PINPOP",58,"FFF1D5");NText("Festival subtitle",nativeHome,20,safeTop+140,350,26,"BOWLING FESTİVALİ",16,"294B61");
   float nav=logicalHeight-safeBottom-58;ReplaceButton("home","play",nativeHome,80,nav-108,230,56,"OYNA");NText("Home tagline",nativeHome,20,nav-47,350,30,"Aynı rengi bul, masayı temizle.",15,"294B61",false);
   NPanel("Navigation",nativeHome,8,nav-4,374,66,"294B61",14);ReplaceButton("home","homeShop",nativeHome,15,nav+2,174,52,"Mağaza","365B70",CrispIcon.Kind.Shop);ReplaceButton("home","homeTab",nativeHome,201,nav+2,174,52,"Harita","466E80",CrispIcon.Kind.Map);
  }
  void BuildNativeGame(){
   NativeField("game/level",nativeGame,83,safeTop+1,224,29,18,"294B61");
   NPanel("Moves card",nativeGame,135,safeTop+46,120,36,"294B61",12);
   NText("Move caption",nativeGame,142,safeTop+51,62,25,"HAMLE",12);
   NativeField("game/moves",nativeGame,204,safeTop+47,44,34,24);
   var settings=ReplaceButton("game","settings",nativeGame,336,safeTop+9,38,38,"","FFF1D5",CrispIcon.Kind.Gear);
   ((CrispShape)settings.targetGraphic).Radius=19*unit;settings.GetComponentInChildren<CrispIcon>().color=Art.Hex("36596A");
   var settingsShadow=nativeGame.Find("SETTINGS shadow").GetComponent<CrispShape>();settingsShadow.Radius=19*unit;settingsShadow.color=new Color(.18f,.29f,.26f,.12f);
   NPanel("Coin plate",nativeGame,12,safeTop+9,88,38,"FFF1D5",19);
   NIcon("Coin",nativeGame,18,safeTop+15,26,CrispIcon.Kind.Coin);
   var coins=NativeField("game/coins",nativeGame,43,safeTop+9,49,38,14,"49614F");coins.font=CrispUI.Regular;

   // Translucent individual surfaces separate helpers from the grass without shifting the table.
   var rail=NRect("Helper rail",nativeGame,4,safeTop+57,54,216);
   for(int i=0;i<4;i++){
    int id=i;var target=NRect("BOOSTER_"+i,rail,0,i*54,54,54);var hit=target.gameObject.AddComponent<Image>();hit.color=Color.clear;hit.raycastTarget=true;
    var artwork=NRect("Helper artwork "+i,target,3,3,48,48);
    var rim=NPanel("Helper rim "+i,artwork,0,0,48,48,"FFF7DF",15);rim.color=new Color(1,.97f,.87f,.48f);
    var background=NPanel("Helper background "+i,artwork,1,1,46,46,"FFF3D9",14);background.color=new Color(1,.95f,.85f,.72f);
    NIcon("Helper symbol "+i,artwork,12,5,24,(CrispIcon.Kind)i,"294B61");
    var button=target.gameObject.AddComponent<Button>();button.targetGraphic=background;button.onClick.AddListener(()=>game.GetComponent<AdventureHUD>().ShowBooster(id));boosterButtons[i]=button;
    var colors=button.colors;colors.highlightedColor=Color.white;colors.pressedColor=new Color(.65f,.85f,.9f);colors.disabledColor=new Color(.65f,.7f,.72f);button.colors=colors;
    target.gameObject.AddComponent<HtmlButtonFeedback>().Artwork=artwork;
    boosterCounts[i]=NText("Booster stock "+i,artwork,8,27,32,18,"",12,"294B61");
   }
   var shadowDock=NPanel("Dock shadow",nativeGame,73,controlTop+3,244,68,"6E8065",18);shadowDock.color=new Color(.2f,.3f,.22f,.14f);
   NPanel("Dock rim",nativeGame,72,controlTop,246,68,"DCCA9F",18);
   var panel=NPanel("Control dock",nativeGame,73,controlTop+1,244,66,"FFF3D9",17);
   nextTitle=NText("Next title",panel.transform,7,2,114,21,"SIRADAKİ",13,"496966",false);
   NextBalls=new Image[3];NextNumbers=new Text[3];NextColors=new int[3];
   for(int i=0;i<3;i++){
    float x=9+i*36;
    var plate=NPanel("Next ball socket "+i,panel.transform,x,25,33,34,i==0?"E7C977":"FFF3D9",16);if(i==0)firstQueueFrame=plate.rectTransform;
    var image=NRect("Next bowling ball preview "+i,plate.transform,1,1,31,31).gameObject.AddComponent<Image>();image.preserveAspect=true;image.raycastTarget=false;NextBalls[i]=image;
    NPanel("Order badge "+i,plate.transform,21,21,16,16,"FFF9EC",8);NextNumbers[i]=NText("Order "+i,plate.transform,21,21,16,16,(i+1).ToString(),12,"36596A");
   }
   var swap=ReplaceButton("game","swap",panel.transform,144,14,80,40,"SWAP","8770AF",CrispIcon.Kind.Swap);
   var swapHit=swap.GetComponent<RectTransform>();swapHit.anchoredPosition+=new Vector2(-5,5)*unit;swapHit.sizeDelta=new Vector2(90,64)*unit;((RectTransform)swap.targetGraphic.transform).anchoredPosition=new Vector2(5,-12)*unit;
   var swapIcon=swap.GetComponentInChildren<CrispIcon>();swapIcon.rectTransform.anchoredPosition=new Vector2(8,-10)*unit;swapIcon.rectTransform.sizeDelta=new Vector2(20,20)*unit;
   var swapLabel=swap.GetComponentInChildren<Text>();swapLabel.rectTransform.anchoredPosition=new Vector2(30,0)*unit;swapLabel.rectTransform.sizeDelta=new Vector2(46,40)*unit;swapLabel.fontSize=Mathf.RoundToInt(14*unit);
   swapStockBadge=NPanel("Swap stock badge",swap.targetGraphic.transform,61,-8,24,24,"D8C4F2",12);
   swapStockLabel=NText("Swap stock",swap.targetGraphic.transform,61,-8,24,24,"",13,"4B3B67");
   // Replenish is the same button when empty; no permanent extra plus control.
   swapPlus.gameObject.SetActive(false);
   instructions=NText("Instructions",nativeGame,55,controlTop-30,280,22,"",14,"496966",false);
   toast.gameObject.SetActive(false);toast=NText("Toast",nativeGame,55,controlTop-49,280,44,"",16,"294B61");
  }
  CarnivalPopup victoryPopup;readonly System.Collections.Generic.Dictionary<string,CarnivalPopup> settingsPopups=new System.Collections.Generic.Dictionary<string,CarnivalPopup>();
  void RegisterCarnivalButton(string view,string id,Button button){string key=view+"/"+id;buttons[key]=button;buttonImages[key]=(Image)button.targetGraphic;if(!buttonSpecs.ContainsKey(key))buttonSpecs[key]=new ButtonData{id=id,rect=new RectData()};fields[key]=button.GetComponentInChildren<Text>();}
  void BuildNativeWin(){
   victoryPopup=CarnivalPopup.Create(views["win"],game,CarnivalPopup.ScreenKind.Success,owner.ReturnHome,ContinueCarnivalResult);
   victoryPopup.CloseButton.name="RESULT_HOME";RegisterCarnivalButton("win","continue",victoryPopup.Primary);RegisterCarnivalButton("win","resultHome",victoryPopup.CloseButton);
   fields["win/resultBody"]=victoryPopup.Body;fields["win/resultReward"]=victoryPopup.Reward;winDetail=victoryPopup.Detail;winChest=victoryPopup.Chest;
   fireworks=CreateResultFX(views["win"],false,new Rect(40,305,520,480));flyingCoins=CreateResultFX(views["win"],true,new Rect(40,305,520,480));
  }
  void BuildNativeSettings(string id){
   var popup=CarnivalPopup.Create(views[id],game,CarnivalPopup.ScreenKind.Settings,owner.CloseOverlays);settingsPopups[id]=popup;popup.CloseButton.name="CLOSE_SETTINGS";RegisterCarnivalButton(id,"closeSettings",popup.CloseButton);
   RegisterCarnivalButton(id,"sound",popup.Sound);
   bool playing=id=="settingsGame";popup.Restart.gameObject.SetActive(playing);popup.Restart.onClick.AddListener(ShowCarnivalRestart);if(!playing){popup.Help.gameObject.SetActive(false);popup.Help=popup.Choice("HELP_SETTINGS",popup.Content,23,239,276,44,"NASIL OYNANIR?",owner.ShowHelp);popup.Help.GetComponentInChildren<Text>().fontSize=13;}
   int privacyIndex=adPrivacyButtons.FindIndex(b=>b.name=="AD_PRIVACY_"+id);if(privacyIndex>=0){adPrivacyButtons[privacyIndex].gameObject.SetActive(false);adPrivacyButtons[privacyIndex]=popup.Choice("AD_PRIVACY_"+id,popup.Stage,72,592,246,61,"REKLAM TERCİHLERİ",()=>game.RewardedVideo.ShowPrivacyOptions());}

  }
  void RefreshCrispUI(){if(!crispReady)return;
   foreach(var popup in settingsPopups.Values)if(popup!=null)popup.RefreshSettings();
   for(int i=0;i<3;i++)NextBalls[i].transform.parent.gameObject.SetActive(NextBalls[i].gameObject.activeSelf);
   if(shownSwapStock!=game.SwapCredits){shownSwapStock=game.SwapCredits;bool empty=shownSwapStock<=0;swapStockBadge.color=Art.Hex(empty?"FFD15B":"D8C4F2");swapStockLabel.text=empty?"+":PinPopEconomy.Compact(shownSwapStock);swapStockLabel.fontSize=Mathf.RoundToInt((empty?20:13)*unit);}
   for(int i=0;i<4;i++){if(game.Adventure==null)continue;if(shownStock[i]!=game.Adventure.Data.stock[i]){shownStock[i]=game.Adventure.Data.stock[i];boosterCounts[i].text=shownStock[i].ToString();}boosterButtons[i].interactable=!game.Busy&&!game.IsPulling&&!game.Finished&&!game.Transitioning;}
  }
  void NativeResult(){if(!crispReady||!resultWin||victoryPopup==null)return;
   int earned=game.BonusMoves>=2?3:game.BonusMoves==1?2:1;for(int i=0;i<3;i++)victoryPopup.Stars[i].color=i<earned?Color.white:new Color(.45f,.39f,.5f,1);
   victoryPopup.Body.text=GameLocalization.Format("{0} duba toplandı.\n{1} hamle cebinde kaldı!",game.Total,game.Moves);
   victoryPopup.Reward.text="+"+PinPopEconomy.Compact(game.LastReward);winDetail.text=game.LastReward==0?"Bu bölümün ödülü zaten alındı.":game.VideoRescueUsed?"Video desteğiyle tamamlandı":GameLocalization.Format("Bölüm {0} + hamle bonusu {1}",game.LastBaseReward,game.LastMoveReward);
   bool chest=game.Adventure!=null&&game.Adventure.LastChest>0;winChest.gameObject.SetActive(chest);if(chest)winChest.text=GameLocalization.Format("SANDIK +{0} COIN · +1 {1}",game.Adventure.LastChestCoins,AdventureRules.Boosters[(game.Adventure.LastChest-1)%4]);
  }
 }
}
