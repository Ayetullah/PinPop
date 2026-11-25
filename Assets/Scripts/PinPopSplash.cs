using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
namespace PinPop {
 // Installed before the game scene, so the first application frame is branded.
 public sealed class PinPopSplash:MonoBehaviour {
  public const float MinimumDuration=.35f,ArtworkFadeDuration=.16f,FadeDuration=.42f;
  public static PinPopSplash Instance {get;private set;}
  public static bool Visible=>Instance!=null&&Instance.gameObject.activeInHierarchy;
  public static float LastDisplaySeconds {get;private set;}
  public static float LastReadyDisplaySeconds {get;private set;}
  public float ReadyDisplaySeconds=>readyPaint<0?0:Time.realtimeSinceStartup-readyPaint;
  public float ArtworkOpacity=>artFade==null?1:artFade.alpha;
  public float CoverOpacity=>fade==null?1:fade.alpha;
  public Camera SplashCamera {get;private set;}
  public bool Ready {get;private set;}
  public string CurrentStage {get;private set;}="İlk açılış çizimi";
  public bool Failed {get;private set;}
  public float DisplaySeconds=>firstPaint<0?0:Time.realtimeSinceStartup-firstPaint;
  float firstPaint=-1,readyPaint=-1,fadeStarted=-1,progress,shownProgress,motionClock;int fittedWidth,fittedHeight;CanvasGroup fade,artFade;bool editorPreview;RectTransform art,ball,backdrop,loadFill;RectTransform[] pins=new RectTransform[3];Texture2D shadeTexture;Sprite shadeSprite;Text status;Button retry;Image[] dots=new Image[3];bool reduced;Font font;
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void Install(){
   LastDisplaySeconds=LastReadyDisplaySeconds=0;foreach(var preview in FindObjectsByType<PinPopSplashPreview>(FindObjectsInactive.Include,FindObjectsSortMode.None))preview.gameObject.SetActive(false);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   var args=Environment.GetCommandLineArgs();if(Array.Exists(args,a=>a.EndsWith("-test")&&a!="-splash-test"))return;
#endif
   if(Instance==null)new GameObject("PinPop startup splash").AddComponent<PinPopSplash>();
  }
  void Awake(){Build(false);}
  public void BuildEditorPreview(){Build(true);}
  void Build(bool preview){if(SplashCamera!=null)return;editorPreview=preview;if(!preview){Instance=this;DontDestroyOnLoad(gameObject);}reduced=PlayerPrefs.GetInt("ReducedMotion",HtmlPlatformPreferences.ReduceMotion()?1:0)==1;
   SplashCamera=gameObject.AddComponent<Camera>();SplashCamera.depth=100;SplashCamera.clearFlags=CameraClearFlags.Depth;SplashCamera.cullingMask=1<<27;SplashCamera.orthographic=true;SplashCamera.orthographicSize=5;SplashCamera.nearClipPlane=.1f;SplashCamera.farClipPlane=10;SplashCamera.transform.position=new Vector3(0,0,-5);
   var go=new GameObject("Startup canvas",typeof(RectTransform),typeof(Canvas),typeof(GraphicRaycaster),typeof(CanvasGroup));go.layer=27;go.transform.SetParent(transform,false);var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=SplashCamera;canvas.planeDistance=1;canvas.sortingOrder=1000;fade=go.GetComponent<CanvasGroup>();
   var block=Image("Startup blocker",go.transform,0,0,1,1,Art.Hex("F4F0E4"));var br=block.rectTransform;br.anchorMin=Vector2.zero;br.anchorMax=Vector2.one;br.offsetMin=br.offsetMax=Vector2.zero;block.raycastTarget=true;
   art=Rect("Startup artwork",go.transform,0,0,600,1000);art.anchorMin=art.anchorMax=art.pivot=Vector2.one*.5f;art.anchoredPosition=Vector2.zero;artFade=art.gameObject.AddComponent<CanvasGroup>();
   var scenery=Image("Splash carnival",go.transform,0,0,600,1000,Color.white);scenery.sprite=Resources.Load<Sprite>("MapArt/region-gate");backdrop=scenery.rectTransform;backdrop.anchorMin=backdrop.anchorMax=backdrop.pivot=Vector2.one*.5f;backdrop.anchoredPosition=Vector2.zero;scenery.transform.SetSiblingIndex(1);
   shadeTexture=new Texture2D(1,64,TextureFormat.RGBA32,false);shadeTexture.wrapMode=TextureWrapMode.Clamp;for(int y=0;y<64;y++){float t=y/63f;shadeTexture.SetPixel(0,y,new Color(.08f,.18f,.22f,Mathf.Lerp(.88f,0,Mathf.SmoothStep(0,1,t))));}shadeTexture.Apply();shadeSprite=Sprite.Create(shadeTexture,new Rect(0,0,1,64),Vector2.one*.5f);var shade=Image("Splash evening shade",go.transform,0,0,1,1,Color.white);shade.sprite=shadeSprite;shade.rectTransform.anchorMin=Vector2.zero;shade.rectTransform.anchorMax=Vector2.one;shade.rectTransform.offsetMin=shade.rectTransform.offsetMax=Vector2.zero;shade.transform.SetSiblingIndex(2);
   font=CrispUI.Bold;
   var title=preview?null:Resources.Load<Sprite>("Startup/launch-title");
   if(title!=null)Image("Splash shared title",art,0,370,600,180,Color.white).sprite=title;
   else {Label("Splash logo depth",30,402,540,128,"PINPOP",100,"294B61");Label("Splash logo coral",30,394,540,128,"PINPOP",100,"E98272");Label("Splash logo",30,387,540,128,"PINPOP",100,"FFF1CE");}
   Label("Splash subtitle",80,511,440,34,"BOWLING FESTİVALİ",18,"FFF1CE");
   for(int i=0;i<3;i++){var pin=Image("Splash pin "+i,art,163+i*92,587+(i==1?-25:0),112,205,Color.white);pin.sprite=Resources.Load<Sprite>("PinIcons/pin-"+i);pin.preserveAspect=true;pins[i]=pin.rectTransform;}
   var b=Image("Splash bowling ball",art,190,701,106,106,Color.white);b.sprite=BallLooks.Icon(0,0);b.preserveAspect=true;ball=b.rectTransform;
   status=Label("Splash status",80,855,440,34,"Festival hazırlanıyor…",16,"FFF1CE");var track=Image("Splash loading track",art,180,904,240,8,new Color(1,1,1,.2f));var fill=Image("Splash loading fill",art,180,904,72,8,Art.Hex("F4BD49"));loadFill=fill.rectTransform;
   for(int i=0;i<3;i++){dots[i]=Image("Splash dot "+i,art,277+i*19,828,7,7,Art.Colors[i]);dots[i].sprite=HelpDemoArt.Disc;}
   var retryArt=Image("Splash retry",art,165,928,270,56,Art.Hex("70935E"));retry=retryArt.gameObject.AddComponent<Button>();retryArt.raycastTarget=true;retry.targetGraphic=retryArt;var text=Label("Splash retry label",0,0,270,56,"YENİDEN DENE",18,"FFF1CE");text.transform.SetParent(retryArt.transform,false);text.rectTransform.anchoredPosition=Vector2.zero;retry.onClick.AddListener(Retry);retry.gameObject.SetActive(false);
   if(!preview)Application.logMessageReceived+=OnLog;Fit();
  }
  public void SetProgress(float value,string message){if(Failed||Ready)return;progress=Mathf.Max(progress,Mathf.Clamp01(value));CurrentStage=message;status.text="Festival hazırlanıyor…";}
  public void MarkReady(){if(Failed)return;Ready=true;progress=1;status.text="Hazır!";}
  void OnPostRender(){if(editorPreview||!(Application.isEditor||UnityEngine.Rendering.SplashScreen.isFinished))return;if(firstPaint<0)firstPaint=Time.realtimeSinceStartup;if(Ready&&readyPaint<0)readyPaint=Time.realtimeSinceStartup;}
  void Update(){if(fittedWidth!=CrispUI.Width||fittedHeight!=CrispUI.Height)Fit();if(EventSystem.current!=null&&EventSystem.current.currentSelectedGameObject!=null&&!Failed)EventSystem.current.SetSelectedGameObject(null);
   if(Ready)motionClock+=Mathf.Min(Time.unscaledDeltaTime,.05f);float age=motionClock;ball.anchoredPosition=new Vector2(190+(reduced?0:Mathf.Sin(age*2)*12),-701);shownProgress=Mathf.MoveTowards(shownProgress,progress,Time.unscaledDeltaTime*1.5f);loadFill.sizeDelta=new Vector2(240*shownProgress,8);loadFill.anchoredPosition=new Vector2(180,-904);for(int i=0;i<3;i++){pins[i].localRotation=Quaternion.Euler(0,0,reduced||!Ready?0:Mathf.Sin(age*2+i*.9f)*3*Mathf.SmoothStep(0,1,ReadyDisplaySeconds/.18f));var c=Art.Colors[i];c.a=1;dots[i].color=c;}
   if(Ready&&!Failed&&readyPaint>=0&&ReadyDisplaySeconds>=MinimumDuration){if(fadeStarted<0)fadeStarted=Time.realtimeSinceStartup;float elapsed=Time.realtimeSinceStartup-fadeStarted;float a=Mathf.Clamp01(elapsed/ArtworkFadeDuration);artFade.alpha=1-a*a*(3-2*a);float p=Mathf.Clamp01((elapsed-ArtworkFadeDuration)/(FadeDuration-ArtworkFadeDuration));fade.alpha=1-p*p*(3-2*p);if(p>=1){LastDisplaySeconds=DisplaySeconds;LastReadyDisplaySeconds=ReadyDisplaySeconds;gameObject.SetActive(false);Destroy(gameObject);}}
  }
  void Fit(){FitTo(CrispUI.Width,CrispUI.Height);}
  public void FitTo(int width,int height){if(art==null)return;fittedWidth=width;fittedHeight=height;float fit=Mathf.Min(width/600f,height/1000f);art.localScale=Vector3.one*fit;if(backdrop!=null){float cover=Mathf.Max(width/1024f,height/1536f);backdrop.sizeDelta=new Vector2(1024,1536)*cover;}foreach(var text in art.GetComponentsInChildren<Text>(true)){int size=text.name.StartsWith("Splash logo")?100:text.name=="Splash subtitle"||text.name=="Splash retry label"?18:16;text.fontSize=Mathf.Max(Mathf.Max(size,19),Mathf.CeilToInt((text.name=="Splash studio"?10:12)/Mathf.Max(.1f,fit)));}}
  void OnLog(string message,string stack,LogType type){if(type!=LogType.Exception||Ready||Failed)return;Failed=true;if(EventSystem.current==null)new GameObject("Startup error input",typeof(EventSystem),typeof(StandaloneInputModule));status.text="Oyun yüklenemedi. Tekrar deneyebilirsin.";retry.gameObject.SetActive(true);}
  void Retry(){Failed=Ready=false;firstPaint=readyPaint=fadeStarted=-1;fade.alpha=artFade.alpha=1;retry.gameObject.SetActive(false);status.text="Festival hazırlanıyor…";SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}
  RectTransform Rect(string name,Transform parent,float x,float y,float w,float h){var o=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer));o.layer=27;var r=o.GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
  Image Image(string name,Transform parent,float x,float y,float w,float h,Color color){var image=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();image.color=color;image.raycastTarget=false;return image;}
  Text Label(string name,float x,float y,float w,float h,string value,int size,string color){var t=Rect(name,art,x,y,w,h).gameObject.AddComponent<CrispText>();t.font=font;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.text=value;t.color=Art.Hex(color);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;return t;}
  void OnDestroy(){if(shadeSprite!=null){if(Application.isPlaying)Destroy(shadeSprite);else DestroyImmediate(shadeSprite);}if(shadeTexture!=null){if(Application.isPlaying)Destroy(shadeTexture);else DestroyImmediate(shadeTexture);}Application.logMessageReceived-=OnLog;if(Instance==this)Instance=null;}
 }
}
