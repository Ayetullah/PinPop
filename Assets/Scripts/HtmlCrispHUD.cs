using UnityEngine;
using UnityEngine.UI;

namespace PinPop {
 public sealed partial class HtmlReferenceHUD {
  bool crispReady;bool settingsFresh,lastMuted,lastReduced,settingsPrivacyVisible;readonly int[] shownStock={-1,-1,-1,-1};float unit,logicalHeight,safeTop,safeBottom,controlTop;
  RectTransform nativeGame,nativeHome,nativeWin;Text winDetail,winChest;CrispShape victoryBorder,victoryCard,chestPlate;readonly CrispIcon[] earnedStars=new CrispIcon[3];readonly Text[] boosterCounts=new Text[4];readonly Button[] boosterButtons=new Button[4];RectTransform firstQueueFrame;CrispShape swapStockBadge;Text swapStockLabel;int shownSwapStock=-1;
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
   settingsFresh=false;settingsPrivacyVisible=game.RewardedVideo!=null&&game.RewardedVideo.PrivacyRequired;shownSwapStock=-1;for(int i=0;i<4;i++)shownStock[i]=-1;unit=AppRect.width/390f;logicalHeight=AppRect.height/unit;safeTop=Mathf.Max(12,(CrispUI.Height-CrispUI.SafeArea.yMax)/unit);safeBottom=Mathf.Max(12,CrispUI.SafeArea.yMin/unit);controlTop=logicalHeight-safeBottom-76;
   foreach(string id in new[]{"home","game","win","settingsHome","settingsGame"})HideOld(id);
   nativeHome=NativeRoot("Crisp home",views["home"]);nativeGame=NativeRoot("Crisp game",views["game"]);nativeWin=NativeRoot("Crisp victory",views["win"]);
   StartupTiming.Begin("BuildNativeHome");BuildNativeHome();StartupTiming.End();if(spread)yield return null;StartupTiming.Begin("BuildNativeGame");BuildNativeGame();StartupTiming.End();if(spread)yield return null;StartupTiming.Begin("BuildNativeWin");BuildNativeWin();StartupTiming.End();if(spread)yield return null;StartupTiming.Begin("BuildNativeSettings(settingsHome)");BuildNativeSettings("settingsHome");StartupTiming.End();if(spread)yield return null;StartupTiming.Begin("BuildNativeSettings(settingsGame)");BuildNativeSettings("settingsGame");StartupTiming.End();
   // Shared overlays keep live text and use the same smooth panel edges.
   foreach(var image in root.GetComponentsInChildren<Image>(true))if(image.gameObject.name=="HTML raster")image.gameObject.SetActive(false);
   if(walletImages.TryGetValue("game/coins",out var wallet))wallet.gameObject.SetActive(false);foreach(string key in new[]{"home/homeCoins","home/homeSwaps"})if(walletImages.TryGetValue(key,out var w))w.gameObject.SetActive(false);
   if(nextGroup!=null)nextGroup.gameObject.SetActive(false);
   // The UI is composed by a separate camera after all scene effects.
   UICamera.allowMSAA=true;crispReady=true;RefreshCrispUI();
  }
  void BuildNativeHome(){
   NPanel("Home coin plate",nativeHome,14,safeTop,105,38,"FFF1D5",19);NIcon("Home coin",nativeHome,19,safeTop+5,28,CrispIcon.Kind.Coin);NativeField("home/homeCoins",nativeHome,52,safeTop,60,38,18,"294B61");
   NPanel("Home swaps plate",nativeHome,264,safeTop,112,38,"FFF1D5",19);NIcon("Home swap",nativeHome,269,safeTop+6,26,CrispIcon.Kind.Swap,"7465AE");NativeField("home/homeSwaps",nativeHome,298,safeTop,72,38,15,"294B61");
   NText("Logo shadow",nativeHome,16,safeTop+72,358,67,"PINPOP",58,"294B61");NText("Logo",nativeHome,14,safeTop+67,358,67,"PINPOP",58,"FFF1D5");NText("Festival subtitle",nativeHome,20,safeTop+140,350,26,"BOWLING FESTİVALİ",16,"294B61");
   float nav=logicalHeight-safeBottom-58;ReplaceButton("home","play",nativeHome,80,nav-108,230,56,"OYNA");NText("Home tagline",nativeHome,20,nav-47,350,30,"Aynı rengi bul, masayı temizle.",15,"294B61",false);
   NPanel("Navigation",nativeHome,8,nav-4,374,66,"294B61",14);ReplaceButton("home","homeShop",nativeHome,15,nav+2,114,52,"Mağaza","365B70",CrispIcon.Kind.Shop);ReplaceButton("home","homeTab",nativeHome,138,nav+2,114,52,"Harita","466E80",CrispIcon.Kind.Map);ReplaceButton("home","homeSettings",nativeHome,261,nav+2,114,52,"Ayarlar","365B70",CrispIcon.Kind.Gear);
  }
  void BuildNativeGame(){
   // Keep the information rail light; moves remain the primary number.
   NPanel("Moves card",nativeGame,12,safeTop,54,62,"294B61",14);
   NText("Move caption",nativeGame,14,safeTop+5,50,17,"HAMLE",11);
   NativeField("game/moves",nativeGame,14,safeTop+21,50,37,30);
   NativeField("game/level",nativeGame,83,safeTop+1,229,29,18,"294B61");
   NPanel("Progress track",nativeGame,90,safeTop+37,210,5,"91ABA0",3);
   progressFill=NPanel("Progress fill",nativeGame,90,safeTop+37,210,5,"E8B64D",3);
   var collected=NativeField("game/count",nativeGame,90,safeTop+47,210,20,13,"496966");collected.font=CrispUI.Regular;
   var settings=ReplaceButton("game","settings",nativeGame,336,safeTop+9,38,38,"","FFF1D5",CrispIcon.Kind.Gear);
   var shop=ReplaceButton("game","gameShop",nativeGame,336,safeTop+65,38,38,"","FFF1D5",CrispIcon.Kind.Shop);
   foreach(var b in new[]{settings,shop}){var art=(CrispShape)b.targetGraphic;art.Radius=19*unit;b.GetComponentInChildren<CrispIcon>().color=Art.Hex("36596A");}
   foreach(string name in new[]{"SETTINGS shadow","GAME_SHOP shadow"}){var shadow=nativeGame.Find(name).GetComponent<CrispShape>();shadow.Radius=19*unit;shadow.color=new Color(.18f,.29f,.26f,.12f);}
   NPanel("Coin plate",nativeGame,12,safeTop+74,108,27,"FFF1D5",14);
   NIcon("Coin",nativeGame,17,safeTop+77,21,CrispIcon.Kind.Coin);
   var coins=NativeField("game/coins",nativeGame,40,safeTop+74,74,27,14,"49614F");coins.font=CrispUI.Regular;

   // Translucent individual surfaces separate helpers from the grass without shifting the table.
   var rail=NRect("Helper rail",nativeGame,0,safeTop+120,54,216);
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
  void BuildNativeWin(){
   var shade=nativeWin.gameObject.AddComponent<Image>();shade.color=new Color(.04f,.10f,.14f,.72f);shade.raycastTarget=true;
   float y=Mathf.Max(safeTop+24,(logicalHeight-432)/2);victoryBorder=NPanel("Victory border",nativeWin,20,y,350,428,"DCCB9E",20);var card=victoryCard=NPanel("Victory card",nativeWin,22,y+2,346,424,"294B61",18);
   NText("Victory title",card.transform,16,19,260,40,"MASA TERTEMİZ!",25);
   for(int i=0;i<3;i++)earnedStars[i]=NIcon("Earned star "+i,card.transform,78+i*66,76,58,CrispIcon.Kind.Star,"F4BD49");
   NativeField("win/resultBody",card.transform,18,150,310,65,19);NativeField("win/resultReward",card.transform,18,222,310,58,23,"FFE4A1");winDetail=NText("Reward details",card.transform,18,286,310,44,"",15,"D3E4E4",false);
   chestPlate=NPanel("Chest reward plate",card.transform,18,335,310,48,"3D6176",12);var chest=NRect("Chest reward icon",chestPlate.transform,8,9,30,30).gameObject.AddComponent<Image>();chest.sprite=Resources.Load<Sprite>("MapArt/Icons/chest");chest.preserveAspect=true;chest.raycastTarget=false;winChest=NText("Chest rewards",chestPlate.transform,48,0,252,48,"",14,"FFE4A1");winChest.alignment=TextAnchor.MiddleLeft;chestPlate.gameObject.SetActive(false);
   ReplaceButton("win","continue",card.transform,22,353,302,54,"SONRAKİ BÖLÜM");ReplaceButton("win","resultHome",card.transform,290,10,44,44,"","416B7A",CrispIcon.Kind.Close);
   fireworks=CreateResultFX(views["win"],false,new Rect(20*600/390f,y*1000/logicalHeight,350*600/390f,428*1000/logicalHeight));flyingCoins=CreateResultFX(views["win"],true,new Rect(20*600/390f,y*1000/logicalHeight,350*600/390f,428*1000/logicalHeight));
  }
  void BuildNativeSettings(string id){var r=NativeRoot("Crisp settings",views[id]);var shade=r.gameObject.AddComponent<Image>();shade.color=new Color(.04f,.10f,.14f,.65f);shade.raycastTarget=true;bool playing=id=="settingsGame";float h=(playing?452:362)+(settingsPrivacyVisible?70:0),y=(logicalHeight-h)/2;NPanel("Settings border",r,20,y,350,h,"DCCB9E",18);var card=NPanel("Settings card",r,22,y+2,346,h-4,"294B61",16);NText("Settings title",card.transform,15,14,270,45,"AYARLAR",28);ReplaceButton(id,"closeSettings",card.transform,290,10,44,44,"","416B7A",CrispIcon.Kind.Close);
   ReplaceButton(id,"sound",card.transform,18,80,310,52,"","416B7A");ReplaceButton(id,"motion",card.transform,18,145,310,52,"","416B7A");
   BuildLanguageDropdown(card.transform);
   NButton("HELP_SETTINGS",card.transform,18,286,310,44,"NASIL OYNANIR?",owner.ShowHelp,"416B7A");
   if(playing){ReplaceButton(id,"restart",card.transform,18,350,150,48,"TEKRAR","416B7A");ReplaceButton(id,"goHome",card.transform,178,350,150,48,"ANA MENÜ","416B7A");}
   var privacy=adPrivacyButtons.Find(b=>b.name=="AD_PRIVACY_"+id);if(privacy!=null){privacy.transform.SetParent(card.transform,false);var pr=privacy.GetComponent<RectTransform>();pr.anchorMin=pr.anchorMax=pr.pivot=new Vector2(0,1);pr.anchoredPosition=new Vector2(18,-(playing?420:346))*unit;pr.sizeDelta=new Vector2(310,44)*unit;privacy.GetComponentInChildren<Text>().fontSize=Mathf.RoundToInt(15*unit);}
  }
  void BuildLanguageDropdown(Transform parent){
   // A single cream row keeps the label, current choice and disclosure together.
   NPanel("Language dropdown shadow",parent,18,216,310,56,"203E4A",11);
   var field=NPanel("LANGUAGE_DROPDOWN",parent,18,213,310,56,"FFF1D5",11);field.raycastTarget=true;
   var dropdown=field.gameObject.AddComponent<LanguageDropdown>();dropdown.targetGraphic=field;
   var colors=dropdown.colors;colors.highlightedColor=Color.white;colors.pressedColor=new Color(.86f,.91f,.92f);dropdown.colors=colors;
   NPanel("Language emblem",field.transform,10,11,34,34,"E5D3A5",17);
   NIcon("Language globe",field.transform,16,17,22,CrispIcon.Kind.Globe,"365B70");
   var languageLabel=NText("Language caption",field.transform,55,0,77,56,"Dil",15,"365B70");languageLabel.alignment=TextAnchor.MiddleLeft;
   var caption=NText("Language value",field.transform,132,0,134,56,"",18,"294B61");caption.alignment=TextAnchor.MiddleRight;dropdown.captionText=caption;
   dropdown.Arrow=NIcon("Language arrow",field.transform,278,18,20,CrispIcon.Kind.ChevronDown,"365B70").rectTransform;dropdown.Arrow.pivot=Vector2.one*.5f;dropdown.Arrow.anchoredPosition+=new Vector2(10,-10)*unit;
   float row=56*unit;
   var template=NPanel("Language list template",field.transform,0,0,310,Mathf.Min(4,GameLocalization.SupportedLanguages.Count)*56+12,"DCCB9E",12).rectTransform;
   template.anchorMin=new Vector2(0,0);template.anchorMax=new Vector2(1,0);template.pivot=new Vector2(.5f,1);template.anchoredPosition=new Vector2(0,-8*unit);template.sizeDelta=new Vector2(0,template.sizeDelta.y);template.GetComponent<Image>().raycastTarget=true;
   var surface=NPanel("Language list surface",template,2,2,306,template.rect.height/unit-4,"203E50",10).rectTransform;surface.anchorMin=Vector2.zero;surface.anchorMax=Vector2.one;surface.offsetMin=Vector2.one*2*unit;surface.offsetMax=-Vector2.one*2*unit;
   var viewport=NRect("Language viewport",template,0,0,310,0);viewport.anchorMin=Vector2.zero;viewport.anchorMax=Vector2.one;viewport.offsetMin=new Vector2(6,0)*unit;viewport.offsetMax=new Vector2(-6,0)*unit;viewport.gameObject.AddComponent<RectMask2D>();
   var content=NRect("Language content",viewport,0,0,0,56);content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);content.sizeDelta=new Vector2(0,row+12*unit);content.anchoredPosition=Vector2.zero;
   var item=NPanel("Language item",content,0,0,0,56,"294B61",8);var itemRect=item.rectTransform;itemRect.anchorMin=new Vector2(0,.5f);itemRect.anchorMax=new Vector2(1,.5f);itemRect.pivot=Vector2.one*.5f;itemRect.anchoredPosition=Vector2.zero;itemRect.sizeDelta=new Vector2(0,row);item.raycastTarget=true;
   var toggle=item.gameObject.AddComponent<Toggle>();toggle.targetGraphic=item;
   var selectedRow=NPanel("Selected language highlight",item.transform,0,0,298,56,"416B7A",8);selectedRow.rectTransform.anchorMin=Vector2.zero;selectedRow.rectTransform.anchorMax=Vector2.one;selectedRow.rectTransform.offsetMin=Vector2.zero;selectedRow.rectTransform.offsetMax=Vector2.zero;toggle.graphic=selectedRow;toggle.isOn=true;
   NPanel("Selected language badge",selectedRow.transform,258,15,26,26,"FFD15B",13);NIcon("Selected language check",selectedRow.transform,263,20,16,CrispIcon.Kind.Check,"294B61");
   var text=NText("Language item label",item.transform,16,0,230,56,"",18);text.alignment=TextAnchor.MiddleLeft;dropdown.itemText=text;dropdown.template=template;
   var scroll=template.gameObject.AddComponent<ScrollRect>();scroll.viewport=viewport;scroll.content=content;scroll.horizontal=false;scroll.vertical=true;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=24*unit;
   var options=new System.Collections.Generic.List<Dropdown.OptionData>();int selected=0;for(int i=0;i<GameLocalization.SupportedLanguages.Count;i++){var choice=GameLocalization.SupportedLanguages[i];options.Add(new Dropdown.OptionData(choice.nativeName));if(choice.code==GameLocalization.LanguageCode)selected=i;}dropdown.AddOptions(options);dropdown.SetValueWithoutNotify(selected);dropdown.RefreshShownValue();dropdown.onValueChanged.AddListener(index=>{if(index<GameLocalization.SupportedLanguages.Count)GameLocalization.SetPreference(GameLocalization.SupportedLanguages[index].code);});template.gameObject.SetActive(false);
  }
  void RefreshCrispUI(){if(!crispReady)return;
   progressFill.rectTransform.sizeDelta=new Vector2(210*unit*game.Cleared/Mathf.Max(1,game.Total),5*unit);progressFill.gameObject.SetActive(game.Cleared>0);
   if(!settingsFresh||lastMuted!=game.Muted||lastReduced!=game.ReducedMotion){settingsFresh=true;lastMuted=game.Muted;lastReduced=game.ReducedMotion;foreach(string id in new[]{"settingsHome","settingsGame"}){Set(id+"/sound",GameLocalization.Format("Ses efektleri  ·  {0}",game.Muted?"KAPALI":"AÇIK"));Set(id+"/motion",GameLocalization.Format("Hareketi azalt  ·  {0}",game.ReducedMotion?"AÇIK":"KAPALI"));}}
   for(int i=0;i<3;i++)NextBalls[i].transform.parent.gameObject.SetActive(NextBalls[i].gameObject.activeSelf);
   if(shownSwapStock!=game.SwapCredits){shownSwapStock=game.SwapCredits;bool empty=shownSwapStock<=0;swapStockBadge.color=Art.Hex(empty?"FFD15B":"D8C4F2");swapStockLabel.text=empty?"+":PinPopEconomy.Compact(shownSwapStock);swapStockLabel.fontSize=Mathf.RoundToInt((empty?20:13)*unit);}
   for(int i=0;i<4;i++){if(game.Adventure==null)continue;if(shownStock[i]!=game.Adventure.Data.stock[i]){shownStock[i]=game.Adventure.Data.stock[i];boosterCounts[i].text=shownStock[i].ToString();}boosterButtons[i].interactable=!game.Busy&&!game.IsPulling&&!game.Finished&&!game.Transitioning;}
  }
  void NativeResult(){if(!crispReady||!resultWin)return;int earned=game.BonusMoves>=2?3:game.BonusMoves==1?2:1;for(int i=0;i<3;i++){earnedStars[i].color=Art.Hex(i<earned?"F4BD49":"627C89");StartCoroutine(StarReveal(earnedStars[i].transform,i));}
   Set("win/resultReward",GameLocalization.Format("+{0} COIN\nBakiye: {1}",game.LastReward,PinPopEconomy.Compact(game.Coins)));winDetail.text=game.LastReward==0?"Bu bölümün ödülü zaten alındı.":game.VideoRescueUsed?"Video desteğiyle tamamlandı":GameLocalization.Format("Bölüm {0} + hamle bonusu {1}",game.LastBaseReward,game.LastMoveReward);
   bool chest=game.Adventure!=null&&game.Adventure.LastChest>0;float extra=chest?58:0,h=428+extra,y=Mathf.Max(safeTop+24,(logicalHeight-h-4)/2);victoryBorder.rectTransform.anchoredPosition=new Vector2(20,-y)*unit;victoryBorder.rectTransform.sizeDelta=new Vector2(350,h)*unit;victoryCard.rectTransform.anchoredPosition=new Vector2(22,-y-2)*unit;victoryCard.rectTransform.sizeDelta=new Vector2(346,h-4)*unit;chestPlate.gameObject.SetActive(chest);if(chest)winChest.text=GameLocalization.Format("SANDIK +{0} COIN · +1 {1}",game.Adventure.LastChestCoins,AdventureRules.Boosters[(game.Adventure.LastChest-1)%4]);PlaceResultButton("continue",353+extra);var area=new Rect(20*600/390f,y*1000/logicalHeight,350*600/390f,h*1000/logicalHeight);fireworks.SetProtectedCard(area);flyingCoins.SetProtectedCard(area);
  }
  void PlaceResultButton(string id,float y){var button=buttons["win/"+id];var hit=button.GetComponent<RectTransform>();float inset=(hit.rect.height-button.targetGraphic.rectTransform.rect.height)/2;hit.anchoredPosition=new Vector2(hit.anchoredPosition.x,-y*unit+inset);var shadow=victoryCard.transform.Find(button.name+" shadow").GetComponent<RectTransform>();shadow.anchoredPosition=new Vector2(shadow.anchoredPosition.x,-(y+3)*unit);}
 }
}
