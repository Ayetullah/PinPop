using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PinPop {
 // Shared native implementation of Design/CarnivalUI. Only decorative artwork is
 // raster art (3x); all copy, buttons, switches, progress and layout remain live UI.
 public sealed class CarnivalPopup : MonoBehaviour {
  public enum ScreenKind { Success, Failure, Restart, Settings, Unlock, Progress, Custom }
  public ScreenKind Kind {get;private set;}
  public RectTransform Stage {get;private set;}
  public RectTransform Content=>card;
  public float CardTop=>-card.anchoredPosition.y;
  public bool CustomCream {get;private set;}
  public Button Primary,Secondary,CloseButton,Sound,Help,Restart;
  public Text Title,Body,Reward,Detail,Chest,Percentage;
  public LanguageDropdown Language;
  public readonly Image[] Stars=new Image[3];
  public readonly RectTransform[] Pins=new RectTransform[3];

  PinPopGame game;RectTransform viewport,card,emblem,hero,progressMask;readonly List<RectTransform> colorPins=new List<RectTransform>();Image soundTrack;RectTransform soundKnob,fill;Text soundState;CanvasGroup entrance;float began,progressTarget;bool soundOn;int shownLevel=-1,shownCoins=-1;Text level,coins,completedLevel;readonly List<Image> bulbs=new List<Image>();readonly List<Vector2> pinOrigins=new List<Vector2>();static readonly Dictionary<string,Sprite> art=new Dictionary<string,Sprite>();
  public static CarnivalPopup Create(Transform parent,PinPopGame owner,ScreenKind kind,UnityAction close,UnityAction action=null){
   var r=CrispUI.Rect("Carnival "+kind,parent,0,0,1,1);r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
   var result=r.gameObject.AddComponent<CarnivalPopup>();result.Build(owner,kind,close,action);return result;
  }
  static Sprite ArtSprite(string name){if(!art.TryGetValue(name,out var sprite)){sprite=Resources.Load<Sprite>("CarnivalUI/"+name);if(sprite==null)throw new InvalidOperationException("Missing CarnivalUI sprite: "+name);art[name]=sprite;}return sprite;}
  RectTransform R(string name,Transform p,float x,float y,float w,float h)=>CrispUI.Rect(name,p,x,y,w,h);
  Image Image(string name,Transform p,float x,float y,float w,float h,string asset){var i=R(name,p,x,y,w,h).gameObject.AddComponent<Image>();i.sprite=ArtSprite(asset);i.raycastTarget=false;return i;}
  CrispShape Panel(string name,Transform p,float x,float y,float w,float h,string color,float radius=12)=>CrispUI.Panel(name,p,x,y,w,h,Art.Hex(color),radius);
  Text Text(string name,Transform p,float x,float y,float w,float h,string value,float size,string color="FFF4DB",bool bold=true)=>CrispUI.Text(name,p,x,y,w,h,value,size,Art.Hex(color),bold);
  Text Heading(string name,Transform p,float x,float y,float w,float h,string value,float size,string color="FFF4DB"){
   var text=Text(name,p,x,y,w,h,value,size,color);var shadow=text.gameObject.AddComponent<Shadow>();shadow.effectColor=p==card?new Color(1,.98f,.88f,.75f):new Color(.15f,.05f,.2f,.8f);shadow.effectDistance=new Vector2(0,p==card?-1:-2);return text;
  }
  void Build(PinPopGame owner,ScreenKind kind,UnityAction close,UnityAction action){
   game=owner;Kind=kind;var blocker=gameObject.AddComponent<Image>();blocker.color=Art.Hex("241B30");blocker.raycastTarget=true;
   viewport=R("Carnival viewport",transform,0,0,1,1);viewport.gameObject.AddComponent<RectMask2D>();Stage=R("Carnival stage",viewport,0,0,390,780);
   Image("Bowling backdrop",Stage,0,0,390,780,"backdrop-bowling");
   Panel("Level chip",Stage,24,30,78,30,"291C3B",15);level=Text("Popup level",Stage,26,30,74,30,"",11,"D7C1E0");
   Panel("Balance chip",Stage,294,30,72,30,"291C3B",15);CrispUI.Icon("Popup coin",Stage,302,36,17,17,CrispIcon.Kind.Coin,Color.white);coins=Text("Popup balance",Stage,321,30,39,30,"",12,"FFE5A1");
   var brand=Text("Festival brand",Stage,20,737,350,32,"PINPOP\nBOWLING FESTİVALİ",10,"88788F");brand.lineSpacing=1.3f;
   entrance=R("Popup entrance",Stage,0,0,390,780).gameObject.AddComponent<CanvasGroup>();
   bool cream=true;CustomCream=true;float panelHeight=kind==ScreenKind.Settings?300:kind==ScreenKind.Progress||kind==ScreenKind.Unlock?400:370;float panelTop=kind==ScreenKind.Settings?274:241;
   card=Image(kind==ScreenKind.Settings?"Settings card":kind==ScreenKind.Success?"Victory card":kind==ScreenKind.Failure?"Recovery card":"Carnival card",entrance.transform,34,panelTop,322,panelHeight,"panel-settings").rectTransform;
   var panelImage=card.GetComponent<Image>();panelImage.sprite=SlicedArt("panel-settings");panelImage.type=UnityEngine.UI.Image.Type.Sliced;
   for(int side=0;side<2;side++)for(int i=0;i<11;i++){var glow=Panel("Light glow",card,side==0?-1:313,23+i*(panelHeight-54)/10,10,10,"FFCC56",5);glow.color=new Color(1,.75f,.3f,.13f);var lamp=Panel("Marquee light",card,side==0?2:316,26+i*(panelHeight-54)/10,4,4,"FFF0AE",2);bulbs.Add(lamp);}
   CloseButton=Button("CARNIVAL_CLOSE",card,258,0,64,64,"",close,false,true);CloseButton.targetGraphic.color=Color.clear;
   Panel("Close circle",CloseButton.targetGraphic.transform,18,17,29,29,cream?"E3CBA3":"432653",15);
   CrispUI.Icon("Close cross",CloseButton.targetGraphic.transform,25,24,15,15,CrispIcon.Kind.Close,Art.Hex(cream?"745E4A":"F9EAC9"));
   emblem=Image("Bowling emblem",entrance.transform,50,panelTop-98,290,106,"bowling-emblem").rectTransform;
   if(kind==ScreenKind.Success)BuildSuccess(action);
   if(kind==ScreenKind.Failure||kind==ScreenKind.Restart)BuildRetry(action);
   if(kind==ScreenKind.Settings)BuildSettings();
   if(kind==ScreenKind.Unlock||kind==ScreenKind.Progress)BuildFeature(action);
   // The close target is always above artwork and never obscured by the canopy.
   CloseButton.transform.SetAsLastSibling();Fit();RefreshHeader();began=Time.unscaledTime;
  }
  public Button Button(string name,Transform parent,float x,float y,float w,float h,string value,UnityAction click,bool coral=false,bool secondary=false){
   var r=R(name,parent,x,y,w,Mathf.Max(61,h));var hit=r.gameObject.AddComponent<Image>();hit.color=Color.clear;hit.raycastTarget=true;
   Image surface;if(secondary){bool plain=string.IsNullOrEmpty(value);surface=Panel(name+" artwork",r,0,0,w,h,plain?"FFFFFF":"D9BA77",12);if(plain)surface.color=Color.clear;else Panel(name+" inset",surface.transform,1.5f,1.5f,w-3,h-3,"FFF8E5",10);}else surface=Image(name+" artwork",r,0,0,w,h+7,coral?"button-coral":"button-green");
   var button=r.gameObject.AddComponent<Button>();button.targetGraphic=surface;var colors=button.colors;colors.highlightedColor=Color.white;colors.pressedColor=new Color(.8f,.83f,.88f);colors.disabledColor=new Color(.45f,.45f,.45f,.75f);button.colors=colors;if(click!=null)button.onClick.AddListener(click);
   var label=Text(name+" label",surface.transform,secondary?4:10,0,w-(secondary?8:20),h,value,secondary?13:22,secondary?"79583E":"FFFBEA");if(!secondary){var shadow=label.gameObject.AddComponent<Shadow>();shadow.effectDistance=new Vector2(0,-2);shadow.effectColor=new Color(.2f,.16f,.15f,.55f);}
   r.gameObject.AddComponent<HtmlButtonFeedback>().Artwork=surface.rectTransform;r.gameObject.AddComponent<PopupControlState>().Bind(button,surface.rectTransform);return button;
  }
  void BuildSuccess(UnityAction action){
   var ribbon=Image("Celebration ribbon",entrance.transform,81,70,225,57,"ribbon");ribbon.rectTransform.localEulerAngles=new Vector3(0,0,4);Heading("Celebration",ribbon.transform,4,0,217,51,"HARİKASIN!",25);Image("Ribbon star",ribbon.transform,-25,0,49,49,"star");
   Panel("Completed level plate",card,116,38,90,25,"E9D7AE",13);completedLevel=Text("Completed level",card,116,38,90,25,GameLocalization.Format("BÖLÜM {0}",game.Level+1),11,"79583E");
   Title=Heading("Victory title",card,22,79,278,73,"MASA\nTERTEMİZ!",32,"79583E");
   Body=Text("resultBody",card,29,163,264,43,"Renkleri buldun, masayı temizledin.",14,"725238");
   for(int i=0;i<3;i++){float size=i==1?37:28;Stars[i]=Image("Earned star "+i,card,109+i*37-(size-28)/2,215-(size-28)/2,size,size,"star");}
   Image("Reward coin",card,101,260,27,27,"coin");Reward=Heading("resultReward",card,133,254,95,39,"",27,"8A5A20");
   Detail=Text("Reward details",card,24,296,274,35,"",11,"725238");
   Chest=Text("Chest rewards",card,24,333,274,27,"",11,"79583E");
   Primary=Button("CONTINUE",entrance.transform,79,629,232,61,"SONRAKİ BÖLÜM",action);
  }
  void BuildRetry(UnityAction action){bool retry=Kind==ScreenKind.Failure;
   hero=Image(retry?"Broken heart":"Restart arrow",card,93,34,136,139,retry?"heart":"restart").rectTransform;
   Title=Heading(retry?"Recovery title":"Restart title",card,23,187,276,64,retry?"ÇOK YAKLAŞTIN!":"YENİDEN BAŞLAT?",26,"79583E");
   Body=Text("resultBody",card,28,251,266,61,retry?"":"Bu bölüme baştan başlayacaksın.\nMevcut denemen sıfırlanacak.",14,"725238");
   Primary=Button(retry?"CONTINUE":"CONFIRM_RESTART",entrance.transform,79,568,232,61,retry?"TEKRAR DENE":"YENİDEN BAŞLAT",action,true);
   if(retry)Secondary=Button("WATCH_VIDEO",entrance.transform,58,638,274,44,"VİDEO İZLE · +3 HAMLE",null,false,true);
   if(retry){var video=Image("Reward video icon",Secondary.targetGraphic.transform,30,14,16,16,"video");video.color=Art.Hex("79583E");}
   Detail=Text("Recovery status",entrance.transform,44,689,302,36,"",12,"D6BDDF");
  }
  void BuildSettings(){
   Title=Text("Settings title",card,30,28,262,45,"AYARLAR",25,"79583E");
   Sound=Switch("SOUND",card,27,87,"Ses efektleri",CrispIcon.Kind.Moves,()=>game.ToggleSound(),out soundTrack,out soundKnob,out soundState);
   BuildLanguage(card,27,159);
   Help=Button("HELP_SETTINGS",card,23,239,140,44,"NASIL OYNANIR?",()=>game.Hud.ShowHelp(),false,true);Help.GetComponentInChildren<Text>().color=Art.Hex("897050");Help.GetComponentInChildren<Text>().fontSize=11;
   Restart=Button("RESTART",card,164,239,140,44,"YENİDEN BAŞLAT",null,false,true);Restart.GetComponentInChildren<Text>().color=Art.Hex("897050");Restart.GetComponentInChildren<Text>().fontSize=11;
   RefreshSettings();
  }
  Button Switch(string name,Transform p,float x,float y,string label,CrispIcon.Kind icon,UnityAction action,out Image track,out RectTransform knob,out Text state){
   Panel(name+" shadow",p,x,y+3,268,61,"C7AE7B",14);var row=Panel(name,p,x,y,268,61,"D9BA77",14);Panel(name+" inset",row.transform,1.5f,1.5f,265,58,"F7ECD0",12);Panel(name+" emblem",row.transform,9,12.5f,36,36,"FFF8E5",18);row.raycastTarget=true;var button=row.gameObject.AddComponent<Button>();button.targetGraphic=row;button.onClick.AddListener(action);
   CrispUI.Icon(name+" symbol",row.transform,11,20,20,20,icon,Art.Hex("8B745D"));var text=Text(name+" caption",row.transform,53,0,132,61,label,14,"725B48");text.alignment=TextAnchor.MiddleLeft;
   var symbol=row.transform.Find(name+" symbol");if(symbol!=null)Destroy(symbol.gameObject);var glyph=Image(name+" symbol",row.transform,17,20,20,20,name=="SOUND"?"volume":"motion");glyph.color=Art.Hex("8B745D");
   track=Panel(name+" track",row.transform,192,16,64,29,"89CA49",15);state=Text(name+" state",track.transform,3,0,31,29,"",8,"FFFBEA");knob=Panel(name+" knob",track.transform,36,3,23,23,"FFFBEA",12).rectTransform;return button;
  }
  void BuildLanguage(Transform parent,float x,float y){
   Panel("Language row shadow",parent,x,y+3,268,64,"C7AE7B",15);
   var rim=Panel("Language row border",parent,x,y,268,64,"D9B970",15);
   var row=Panel("LANGUAGE_DROPDOWN",rim.transform,1.5f,1.5f,265,61,"F7ECD0",13);row.raycastTarget=true;
   Language=row.gameObject.AddComponent<LanguageDropdown>();Language.FocusRing=rim;
   Panel("Language emblem rim",row.transform,9,13,36,36,"DAB974",18);Panel("Language emblem",row.transform,10.5f,14.5f,33,33,"FFF7E1",17);
   CrispUI.Icon("Language globe",row.transform,17,21,20,20,CrispIcon.Kind.Globe,Art.Hex("8A632E"));
   var caption=Text("Language caption",row.transform,49,0,78,61,"Dil",14,"705036");caption.alignment=TextAnchor.MiddleLeft;
   var field=Panel("Language value border",row.transform,131,6,127,49,"E4C885",12);
   Panel("Language value inset",field.transform,1.5f,1.5f,124,46,"FFFAEC",10);
   Language.targetGraphic=field;var colors=Language.colors;colors.highlightedColor=Color.white;colors.pressedColor=new Color(.9f,.84f,.7f,1);Language.colors=colors;
   Language.captionText=Text("Language value",field.transform,5,0,82,49,"",16,"65452E");Language.captionText.alignment=TextAnchor.MiddleCenter;
   Panel("Language arrow divider",field.transform,91,13,1,23,"E8D8AF",0);
   var arrow=CrispUI.Icon("Language arrow",field.transform,102,17,15,15,CrispIcon.Kind.ChevronDown,Art.Hex("88602F")).rectTransform;arrow.pivot=Vector2.one*.5f;arrow.anchoredPosition+=new Vector2(7.5f,-7.5f);Language.Arrow=arrow;
   // The list spans the complete row. Full-size options replace the small,
   // detached value-only menu and keep the same touch area on narrow phones.
   var template=R("Language list template",row.transform,0,71,265,140);Panel("Language list shadow",template,0,4,265,140,"B99D70",15);
   Panel("Language list border",template,0,0,265,140,"D9B970",15);Panel("Language list surface",template,2,2,261,136,"FFF8E5",13);
   var viewport=R("Viewport",template,6,6,253,128);viewport.gameObject.AddComponent<RectMask2D>();
   var content=R("Content",viewport,0,0,253,64);content.pivot=new Vector2(.5f,1);content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.sizeDelta=new Vector2(0,64);content.anchoredPosition=Vector2.zero;
   var item=Panel("Language item",content,0,0,253,64,"FFF8E5",10);var ir=item.rectTransform;ir.anchorMin=new Vector2(0,.5f);ir.anchorMax=new Vector2(1,.5f);ir.pivot=Vector2.one*.5f;ir.anchoredPosition=Vector2.zero;ir.sizeDelta=new Vector2(0,64);item.raycastTarget=true;
   var toggle=item.gameObject.AddComponent<Toggle>();toggle.targetGraphic=item;
   var selected=Panel("Selected language highlight",item.transform,0,2,253,60,"E8D9AE",10);toggle.graphic=selected;toggle.isOn=true;
   Panel("Selected language badge",selected.transform,212,18,26,26,"7B9850",13);CrispUI.Icon("Selected language check",selected.transform,218,24,14,14,CrispIcon.Kind.Check,Art.Hex("FFFBE9"));
   Panel("Language option emblem",item.transform,10,17,30,30,"F2E4BE",15);CrispUI.Icon("Language option globe",item.transform,17,24,16,16,CrispIcon.Kind.Globe,Art.Hex("9B7844"));
   Language.itemText=Text("Language item label",item.transform,51,0,152,64,"",17,"65452E");Language.itemText.alignment=TextAnchor.MiddleLeft;Language.template=template;
   var scroll=template.gameObject.AddComponent<ScrollRect>();scroll.viewport=viewport;scroll.content=content;scroll.horizontal=false;scroll.vertical=true;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=24;
   Language.alphaFadeSpeed=.12f;var options=new List<Dropdown.OptionData>();int chosen=0;for(int i=0;i<GameLocalization.SupportedLanguages.Count;i++){var c=GameLocalization.SupportedLanguages[i];options.Add(new Dropdown.OptionData(c.nativeName));if(c.code==GameLocalization.LanguageCode)chosen=i;}Language.AddOptions(options);Language.SetValueWithoutNotify(chosen);Language.RefreshShownValue();Language.onValueChanged.AddListener(i=>GameLocalization.SetPreference(GameLocalization.SupportedLanguages[i].code));template.gameObject.SetActive(false);
  }
  void BuildFeature(UnityAction action){bool progress=Kind==ScreenKind.Progress;
   Title=Heading("Feature title",card,21,30,280,52,progress?"YENİ BİR SÜRPRİZ":"YENİ ÖZELLİK AÇILDI!",23,"79583E");
   if(!progress)Text("Unlock milestone",card,32,76,258,24,"BÖLÜM 7’DE AÇILDI",10,"79583E");
   float top=progress?92:108;var group=R("Linked triple",card,65,top,192,150);
   for(int i=0;i<3;i++){var pin=R("Linked pin "+i,group,i*62-25,0,112.5f,150).gameObject.AddComponent<Image>();pin.sprite=Resources.Load<Sprite>("PinIcons/pin-0");pin.preserveAspect=true;pin.raycastTarget=false;Pins[i]=pin.rectTransform;Pins[i].localEulerAngles=new Vector3(0,0,(1-i)*8);pinOrigins.Add(Pins[i].anchoredPosition);if(progress)pin.color=Art.Hex("8E7D96");}
   Image("Golden band",group,7,102,178,34,"binding");
   if(progress){
    // Reveal full-color artwork upward as real chapter progress increases.
    foreach(var pin in Pins)pin.GetComponent<Image>().color=Art.Hex("74667C");
    var clip=R("Colored reveal",group,0,150,192,0);clip.pivot=new Vector2(0,0);clip.anchoredPosition=new Vector2(0,-150);clip.gameObject.AddComponent<RectMask2D>();progressMask=clip;
    var reveal=R("Colored linked triple",clip,0,0,192,150);reveal.anchorMin=reveal.anchorMax=Vector2.zero;reveal.pivot=Vector2.zero;reveal.anchoredPosition=Vector2.zero;
    for(int i=0;i<3;i++){var pin=Instantiate(Pins[i],reveal,false);pin.name="Color reveal pin "+i;pin.GetComponent<Image>().color=Color.white;colorPins.Add(pin);}
    Image("Colored golden band",reveal,7,102,178,34,"binding");
    Panel("Feature lock",group,80,82,32,32,"76409E",10);var lockArt=Image("Lock",group,87,88,18,20,"lock");lockArt.color=Art.Hex("FFE5A3");
    Percentage=Heading("Feature percentage",card,70,230,182,58,"0%",40,"79583E");var track=Panel("Feature progress track",card,45,289,232,8,"2F193D",4);fill=Panel("Feature progress fill",track.transform,0,0,0,8,"E9B748",4).rectTransform;
   }
   float labelY=progress?307:270;Image("Feature name plaque",card,39,labelY-3,244,51,"plaque");Text("Feature name",card,44,labelY-3,234,48,"BAĞLI ÜÇLÜ",17,"79553D");
   Body=Text("Feature explanation",card,31,progress?358:326,260,44,progress?"Her bölüm seni yeni bir özelliğe yaklaştırır.":"Aynı renkte bir top bağı koparır.\nÜç duba sonraki atışta serbest kalır.",12,"725238");
   if(progress){Body.rectTransform.anchoredPosition=new Vector2(31,-356);Body.rectTransform.sizeDelta=new Vector2(260,32);}
   if(progress)Primary=Button("FEATURE_CONTINUE",entrance.transform,79,661,232,61,"DEVAM ET",action);
  }
  public static CarnivalPopup Custom(Transform parent,PinPopGame owner,string title,float height,UnityAction close,bool cream=true){
   var popup=Create(parent,owner,ScreenKind.Custom,close);popup.ConfigureCustom(title,height,cream);return popup;
  }
  public const float PurchasePanelHeight=420;
  public static CarnivalPopup Purchase(Transform parent,PinPopGame owner,string title,UnityAction close)=>Custom(parent,owner,title,PurchasePanelHeight,close);
  void ConfigureCustom(string title,float height,bool cream){
   CustomCream=cream;float top=Mathf.Clamp(440-height/2,180,241);card.anchoredPosition=new Vector2(34,-top);card.sizeDelta=new Vector2(322,height);var image=card.GetComponent<Image>();image.sprite=SlicedArt(cream?"panel-settings":"panel");image.type=UnityEngine.UI.Image.Type.Sliced;
   int glowIndex=0;foreach(var glow in card.GetComponentsInChildren<CrispShape>())if(glow.name=="Light glow"){var point=glow.rectTransform.anchoredPosition;point.y=-(23+(glowIndex++%11)*(height-54)/10);glow.rectTransform.anchoredPosition=point;}
   emblem.anchoredPosition=new Vector2(50,-top+98);for(int i=0;i<bulbs.Count;i++){var point=bulbs[i].rectTransform.anchoredPosition;point.y=-(26+(i%11)*(height-54)/10);bulbs[i].rectTransform.anchoredPosition=point;}
   foreach(var icon in CloseButton.GetComponentsInChildren<CrispIcon>())icon.color=Art.Hex(cream?"745E4A":"F9EAC9");var circle=CloseButton.targetGraphic.transform.Find("Close circle").GetComponent<Image>();circle.color=Art.Hex(cream?"E3CBA3":"432653");
   Title=Text("Popup title",card,21,43,280,44,title,23,cream?"79583E":"FFF4DB");Body=Text("Popup explanation",card,24,92,274,56,"",13,cream?"795E48":"E4D3ED");
  }
  static Sprite SlicedArt(string name){string key=name+" sliced";if(!art.TryGetValue(key,out var sprite)){var original=ArtSprite(name);sprite=Sprite.Create(original.texture,original.rect,Vector2.one*.5f,300,0,SpriteMeshType.FullRect,Vector4.one*72);art[key]=sprite;}return sprite;}
  public Button BottomAction(string name,string label,UnityAction action,bool coral=false){return Primary=Button(name,entrance.transform,59,CardTop+card.rect.height+18,272,61,label,action,coral);}
  public Button PurchaseAction(string name,int price,UnityAction action){return Primary=PurchaseButton(name,entrance.transform,59,CardTop+card.rect.height+18,272,61,price,action);}
  public Button PurchaseButton(string name,Transform parent,float x,float y,float w,float h,int price,UnityAction action){
   var button=Button(name,parent,x,y,w,h,"SATIN AL",action);var surface=(Image)button.targetGraphic;var text=button.GetComponentInChildren<Text>();
   var coin=CrispUI.Icon(name+" coin",surface.transform,0,0,20,20,CrispIcon.Kind.Coin,Color.white);
   var view=button.gameObject.AddComponent<CarnivalPurchaseButton>();view.Bind(surface,text,coin,SlicedArt("panel-settings"),w<200,h);view.SetOffer(price);return button;
  }
  public Text Copy(string name,Transform p,float x,float y,float w,float h,string value,float size=14,bool bold=true)=>Text(name,p,x,y,w,h,value,size,CustomCream?"725238":"E4D3ED",bold);
  public Image Picture(string name,Transform p,float x,float y,float w,float h,Sprite sprite){var image=R(name,p,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;return image;}
  public CrispShape Plate(string name,Transform p,float x,float y,float w,float h,string hex="FFF8E5",float radius=12)=>Panel(name,p,x,y,w,h,hex,radius);
  public Button Choice(string name,Transform parent,float x,float y,float w,float h,string label,UnityAction action,bool accent=false){
   var hit=R(name,parent,x,y,w,Mathf.Max(61,h));var touch=hit.gameObject.AddComponent<Image>();touch.color=Color.clear;touch.raycastTarget=true;
   var rim=Panel(name+" rim",hit,0,2,w,h,"BCA06C",12);var edge=Panel(name+" artwork",hit,0,0,w,h,accent?"8CA859":"D9BA77",12);Panel(name+" inset",edge.transform,1.5f,1.5f,w-3,h-3,accent?"EAF1CE":"FFF8E5",10);
   var button=hit.gameObject.AddComponent<Button>();button.targetGraphic=edge;var colors=button.colors;colors.highlightedColor=Color.white;colors.pressedColor=new Color(.85f,.8f,.7f);colors.disabledColor=new Color(.7f,.66f,.59f,.8f);button.colors=colors;if(action!=null)button.onClick.AddListener(action);
   Text(name+" label",edge.transform,5,0,w-10,h,label,15,"705036");hit.gameObject.AddComponent<HtmlButtonFeedback>().Artwork=edge.rectTransform;hit.gameObject.AddComponent<PopupControlState>().Bind(button,edge.rectTransform);return button;
  }
  public void Settle(){began=Time.unscaledTime-1;if(entrance!=null)entrance.alpha=1;}
  public void SetProgress(float value){progressTarget=Mathf.Clamp01(value);began=Time.unscaledTime;}
  public void SetFeatureCopy(string title,string body){Title.text=title;Body.text=body;}
  public void RefreshSettings(){if(Sound==null)return;soundOn=!game.Muted;UpdateSwitch(soundTrack,soundKnob,soundState,soundOn);}
  void UpdateSwitch(Image track,RectTransform knob,Text state,bool on){track.color=Art.Hex(on?"89CA49":"AA9E80");knob.anchoredPosition=new Vector2(on?36:3,-3);state.rectTransform.anchoredPosition=new Vector2(on?3:29,0);state.text=on?"AÇIK":"KAPALI";}
  void Fit(){
   if(game==null||game.Hud==null)return;var view=game.GetComponent<HtmlReferenceHUD>();if(view==null)return;
   Rect app=view.AppRect,safe=CrispUI.SafeArea;
   float left=Mathf.Max(app.xMin,safe.xMin),right=Mathf.Min(app.xMax,safe.xMax);
   float bottom=Mathf.Max(app.yMin,safe.yMin),top=Mathf.Min(app.yMax,safe.yMax);
   float w=Mathf.Max(1,right-left),h=Mathf.Max(1,top-bottom);
   // Screen rectangles start at the bottom; our top-left anchored UI uses negative Y.
   viewport.anchoredPosition=new Vector2(left,-(CrispUI.Height-top));viewport.sizeDelta=new Vector2(w,h);
   float scale=Mathf.Min(w/390,h/780);Stage.localScale=Vector3.one*scale;
   Stage.anchoredPosition=new Vector2((w-390*scale)/2,-(h-780*scale)/2);
  }
  void RefreshHeader(){if(shownLevel!=game.Level){shownLevel=game.Level;level.text=GameLocalization.Format("BÖLÜM {0}",game.Level+1);if(completedLevel!=null)completedLevel.text=level.text;}if(shownCoins!=game.Coins){shownCoins=game.Coins;coins.text=PinPopEconomy.Compact(game.Coins);}}
  void OnEnable(){began=Time.unscaledTime;if(entrance!=null)entrance.alpha=game!=null&&game.ReducedMotion?1:0;}
  void LateUpdate(){if(game==null||Stage==null)return;Fit();RefreshHeader();if(Sound!=null&&soundOn==game.Muted)RefreshSettings();float age=Time.unscaledTime-began;bool still=game.ReducedMotion;float p=still?1:Mathf.Clamp01(age/.65f);float ease=1-Mathf.Pow(1-p,3);entrance.alpha=p;entrance.transform.localScale=Vector3.one*Mathf.Lerp(.94f,1,ease);entrance.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-19*(1-ease));
   for(int i=0;i<bulbs.Count;i++){var c=bulbs[i].color;c.a=still?1:.72f+.28f*Mathf.Sin(age*2+i*.24f);bulbs[i].color=c;}
   if(hero!=null){float t=still?1:Mathf.Clamp01(age/.9f);hero.localScale=Vector3.one*Mathf.Lerp(.7f,1,1-Mathf.Pow(1-t,3));hero.localEulerAngles=new Vector3(0,0,(Kind==ScreenKind.Restart?70:8)*(1-t));}
   for(int i=0;i<3;i++){if(Stars[i]!=null){float t=still?1:Mathf.Clamp01((age-.5f-i*.14f)/.5f);Stars[i].transform.localScale=Vector3.one*(1+2.18f*Mathf.Pow(t-1,3)+1.18f*Mathf.Pow(t-1,2));}if(Pins[i]!=null){Pins[i].anchoredPosition=pinOrigins[i]+Vector2.up*(still?0:Mathf.Sin(age*1.75f+i*.7f)*3);if(i<colorPins.Count)colorPins[i].anchoredPosition=Pins[i].anchoredPosition;}}
   if(Percentage!=null){float value=progressTarget*(still?1:1-Mathf.Pow(1-Mathf.Clamp01(age/.95f),3));Percentage.text=Mathf.RoundToInt(value*100)+"%";fill.sizeDelta=new Vector2(232*value,8);if(progressMask!=null)progressMask.sizeDelta=new Vector2(192,150*value);}
  }
 }
}
