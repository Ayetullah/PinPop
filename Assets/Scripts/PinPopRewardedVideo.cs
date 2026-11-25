using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PinPop {
 public sealed class PinPopRewardedVideo:MonoBehaviour {
  PinPopGame game;AdMobRewardedProvider provider;Canvas canvas;VideoPlayer player;RawImage picture;Text status,title,loadingLabel;Image progress;Button cancel;bool started,focused=true;float watched,lastVideoTime;int attempt,request,layoutWidth,layoutHeight;readonly Dictionary<Text,int> textSizes=new Dictionary<Text,int>();
  public bool Busy {get;private set;}
  public bool PrivacyRequired=>provider!=null&&AdMobRewardedProvider.PrivacyRequired;
  public void SyncSound(){if(provider!=null)provider.SetMuted(game.Muted);}
  public void ShowPrivacyOptions(){if(provider!=null&&!Busy)provider.PrivacyOptions();}
  public bool Playing=>Busy&&player!=null&&player.isPlaying;
  public double VideoTime=>player==null?0:player.time;
  public void Build(PinPopGame model){game=model;}
  // Keep SDK startup out of the splash/home handoff. Ad IDs and reward rules are unchanged.
  void PrepareProvider(){
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
   if(provider==null){provider=gameObject.AddComponent<AdMobRewardedProvider>();provider.SetMuted(game.Muted);}
#endif
  }
  public void Watch(){if(Busy||!game.CanWatchForMoves)return;Busy=true;attempt=game.AttemptId;started=false;watched=lastVideoTime=0;int token=++request;BuildPanel();canvas.gameObject.SetActive(true);layoutWidth=0;FitVideoUI();picture.gameObject.SetActive(provider==null);loadingLabel.gameObject.SetActive(true);game.Hud.Refresh();StartCoroutine(Begin(token));}
  IEnumerator Begin(int token){
   yield return new WaitForEndOfFrame();if(token!=request)yield break;PrepareProvider();
   if(provider!=null){picture.gameObject.SetActive(false);title.text="ÖDÜLLÜ VİDEO";status.text="Video hazırlanıyor…";provider.Load();float start=Time.realtimeSinceStartup;while(token==request&&!provider.Ready&&Time.realtimeSinceStartup-start<12)yield return null;if(token!=request)yield break;
    if(!provider.Ready){Finish(false,"Video şu an hazır değil. Biraz sonra tekrar dene.");yield break;}cancel.interactable=false;
    if(!provider.Show(ok=>{if(token==request)Finish(ok,ok?"":"Video tamamlanmadı. Hamle eklenmedi.");}))Finish(false,"Video açılamadı. Tekrar deneyebilirsin.");yield break;
   }
   title.text="PINPOP TANITIMI";status.text="Video yükleniyor…";
   if(player!=null)Destroy(player);player=gameObject.AddComponent<VideoPlayer>();player.playOnAwake=false;player.isLooping=false;player.skipOnDrop=false;player.playbackSpeed=1;player.timeUpdateMode=VideoTimeUpdateMode.UnscaledGameTime;player.renderMode=VideoRenderMode.APIOnly;player.audioOutputMode=VideoAudioOutputMode.None;player.url=Path.Combine(Application.streamingAssetsPath,"pinpop-preview.mp4");
   player.errorReceived+=(source,message)=>{if(token==request)Finish(false,"Video açılamadı. Tekrar deneyebilirsin.");};
   player.loopPointReached+=source=>{if(token==request)Finish(watched>=source.length-.5f,"Video tamamlanmadı. Hamle eklenmedi.");};player.Prepare();float began=Time.realtimeSinceStartup;
   while(token==request&&!player.isPrepared&&Time.realtimeSinceStartup-began<12)yield return null;
   if(token!=request)yield break;if(!player.isPrepared){Finish(false,"Video yüklenemedi. Tekrar deneyebilirsin.");yield break;}picture.texture=player.texture;started=true;if(focused)player.Play();
  }
  void FitVideoUI(){if(canvas==null||layoutWidth==Screen.width&&layoutHeight==Screen.height)return;layoutWidth=Screen.width;layoutHeight=Screen.height;Canvas.ForceUpdateCanvases();float scale=Mathf.Max(.1f,canvas.scaleFactor);foreach(var pair in textSizes)pair.Key.fontSize=Mathf.Max(pair.Value,Mathf.CeilToInt(12/scale));var rect=cancel.GetComponent<RectTransform>();rect.sizeDelta=Vector2.one*Mathf.Max(84,44/scale);}
  void Update(){if(Busy)FitVideoUI();if(!Busy||provider!=null||!started||player==null)return;if(player.texture!=null){picture.texture=player.texture;picture.color=Color.white;loadingLabel.gameObject.SetActive(false);}if(player.isPlaying&&focused&&player.texture!=null){float now=(float)player.time;watched+=Mathf.Max(0,Mathf.Min(now-lastVideoTime,Time.unscaledDeltaTime+.1f));lastVideoTime=now;}float fraction=player.length>0?(float)(player.time/player.length):0;progress.fillAmount=fraction;status.text=!focused?"Video duraklatıldı":GameLocalization.Format("{0} sn · Bitince +3 hamle",Mathf.Max(0,Mathf.CeilToInt((float)(player.length-player.time))));}
  public void Cancel(){if(!Busy)return;Finish(false,"Video tamamlanmadı. Hamle eklenmedi.");}
  void Finish(bool earned,string message){if(!Busy)return;Busy=false;request++;StopAllCoroutines();if(player!=null){player.Stop();Destroy(player);player=null;}if(canvas!=null)canvas.gameObject.SetActive(false);bool same=attempt==game.AttemptId;
   if(earned&&same&&game.ClaimVideoMoves(attempt))return;if(same){game.Hud.RecoveryMessage(message);game.Hud.Refresh();}}
  void OnApplicationFocus(bool value){focused=value;if(Busy&&started&&player!=null){if(value)player.Play();else player.Pause();}}
  void OnApplicationPause(bool value){OnApplicationFocus(!value);}
  void LateUpdate(){if(Busy&&Input.GetKeyDown(KeyCode.Escape)&&cancel.interactable)Cancel();}
  void OnDestroy(){if(player!=null)player.Stop();if(canvas!=null)Destroy(canvas.gameObject);}
  void BuildPanel(){if(canvas!=null){cancel.interactable=true;progress.fillAmount=0;picture.texture=null;picture.color=Art.Hex("203F51");return;}
   var obj=new GameObject("Rewarded video screen",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvas=obj.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=100;var scaler=obj.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(600,1000);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
   var bg=obj.AddComponent<Image>();bg.color=Art.Hex("173448");var frame=new GameObject("Portrait video",typeof(RectTransform),typeof(AspectRatioFitter));frame.transform.SetParent(obj.transform,false);var fit=frame.GetComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=.6f;
   title=TextAt(frame.transform,"Video title",50,50,390,46,"PINPOP TANITIMI",26);TextAt(frame.transform,"Video reward",60,104,480,34,"Tamamla, +3 hamleyle devam et",16);
   var pic=Rect(frame.transform,"Video picture",105,155,390,650);picture=pic.gameObject.AddComponent<RawImage>();picture.color=Art.Hex("203F51");picture.raycastTarget=false;loadingLabel=TextAt(frame.transform,"Video loading",75,425,450,70,"Video hazırlanıyor…",22);
   var track=Rect(frame.transform,"Video progress track",80,840,440,7).gameObject.AddComponent<Image>();track.color=Art.Hex("3D6176");progress=Rect(frame.transform,"Video progress",80,840,440,7).gameObject.AddComponent<Image>();progress.sprite=Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,Texture2D.whiteTexture.width,Texture2D.whiteTexture.height),new Vector2(.5f,.5f));progress.type=Image.Type.Filled;progress.fillMethod=Image.FillMethod.Horizontal;progress.fillAmount=0;progress.color=Art.Hex("E5D29D");
   status=TextAt(frame.transform,"Video status",50,865,500,38,"Video hazırlanıyor…",18);
   var close=Rect(frame.transform,"Cancel rewarded video",488,28,84,84);var touch=close.gameObject.AddComponent<Image>();touch.color=Color.clear;touch.raycastTarget=true;
   var art=Rect(close,"Video close artwork",0,0,52,52);art.anchorMin=art.anchorMax=art.pivot=Vector2.one*.5f;art.anchoredPosition=Vector2.zero;var image=art.gameObject.AddComponent<CrispShape>();image.Radius=10;image.color=Art.Hex("466C82");image.raycastTarget=false;
   CrispUI.Icon("Video close cross",art,13,13,26,26,CrispIcon.Kind.Close,Art.Hex("FFF1CE"));cancel=close.gameObject.AddComponent<Button>();cancel.targetGraphic=image;cancel.onClick.AddListener(Cancel);close.gameObject.AddComponent<HtmlButtonFeedback>().Artwork=art;

  }
  static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x+w/2-300,500-y-h/2);return r;}
  Text TextAt(Transform parent,string name,float x,float y,float w,float h,string value,int size){var t=Rect(parent,name,x,y,w,h).gameObject.AddComponent<CrispText>();t.font=Font.CreateDynamicFontFromOSFont(new[]{"Avenir Next","Arial"},size);t.fontSize=size;t.fontStyle=name=="Video title"||name=="Cancel label"?FontStyle.Bold:FontStyle.Normal;t.text=value;t.color=Art.Hex("FFF1CE");t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;textSizes[t]=size;return t;}
 }
}
