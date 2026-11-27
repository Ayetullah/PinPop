using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PinPop {
 public sealed class PinPopRewardedVideo:MonoBehaviour {
  CarnivalPopup popup;PinPopGame game;AdMobRewardedProvider provider;Canvas canvas;VideoPlayer player;RawImage picture;Text status,title,loadingLabel;Image progress;Button cancel;bool started,focused=true,pendingReward;string completionMessage="";float watched,lastVideoTime;int attempt,request,layoutWidth,layoutHeight;readonly Dictionary<Text,int> textSizes=new Dictionary<Text,int>();
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
  public void Watch(){if(Busy||canvas!=null&&canvas.gameObject.activeSelf||!game.CanWatchForMoves)return;pendingReward=false;completionMessage="";Busy=true;attempt=game.AttemptId;started=false;watched=lastVideoTime=0;int token=++request;BuildPanel();canvas.gameObject.SetActive(true);layoutWidth=0;FitVideoUI();picture.gameObject.SetActive(provider==null);loadingLabel.gameObject.SetActive(true);game.Hud.Refresh();StartCoroutine(Begin(token));}
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
  void FitVideoUI(){if(canvas==null||layoutWidth==CrispUI.Width&&layoutHeight==CrispUI.Height)return;layoutWidth=CrispUI.Width;layoutHeight=CrispUI.Height;Canvas.ForceUpdateCanvases();}
  void Update(){if(Busy)FitVideoUI();if(!Busy||provider!=null||!started||player==null)return;if(player.texture!=null){picture.texture=player.texture;picture.color=Color.white;loadingLabel.gameObject.SetActive(false);}if(player.isPlaying&&focused&&player.texture!=null){float now=(float)player.time;watched+=Mathf.Max(0,Mathf.Min(now-lastVideoTime,Time.unscaledDeltaTime+.1f));lastVideoTime=now;}float fraction=player.length>0?(float)(player.time/player.length):0;progress.fillAmount=fraction;status.text=!focused?"Video duraklatıldı":GameLocalization.Format("{0} sn · Bitince +3 hamle",Mathf.Max(0,Mathf.CeilToInt((float)(player.length-player.time))));}
  public void Cancel(){if(!Busy)return;bool earned=pendingReward&&attempt==game.AttemptId;string message=completionMessage;Busy=false;pendingReward=false;request++;StopAllCoroutines();StopPlayer();if(canvas!=null)canvas.gameObject.SetActive(false);
   if(earned&&game.ClaimVideoMoves(attempt))return;if(attempt==game.AttemptId){game.Hud.RecoveryMessage(string.IsNullOrEmpty(message)?"Video tamamlanmadı. Hamle eklenmedi.":message);game.Hud.Refresh();}
  }
  void StopPlayer(){if(player!=null){player.Stop();Destroy(player);player=null;}started=false;}
  void Finish(bool earned,string message){if(!Busy)return;request++;StopAllCoroutines();StopPlayer();pendingReward=earned&&attempt==game.AttemptId;completionMessage=message;cancel.interactable=true;
   picture.gameObject.SetActive(false);loadingLabel.gameObject.SetActive(true);loadingLabel.text=pendingReward?"Video tamamlandı":"Video tamamlanmadı";title.text="ÖDÜLLÜ VİDEO";status.text=pendingReward?"Ödül hazır · +3 hamle":message;progress.fillAmount=pendingReward?1:0;
   // Completion/errors remain visible. Only the cross dismisses this owned window.
   game.Hud.Refresh();
  }
  void OnApplicationFocus(bool value){focused=value;if(Busy&&started&&player!=null){if(value)player.Play();else player.Pause();}}
  void OnApplicationPause(bool value){OnApplicationFocus(!value);}
  void OnDestroy(){if(player!=null)player.Stop();if(canvas!=null)Destroy(canvas.gameObject);}
  void BuildPanel(){if(canvas!=null){cancel.interactable=true;progress.fillAmount=0;picture.texture=null;picture.color=Art.Hex("2D203D");return;}
   var obj=new GameObject("Rewarded video screen",typeof(RectTransform),typeof(Canvas),typeof(GraphicRaycaster));obj.layer=5;canvas=obj.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=game.Hud.UICamera;canvas.planeDistance=.4f;canvas.sortingOrder=100;
   popup=CarnivalPopup.Custom(obj.transform,game,"ÖDÜLLÜ VİDEO",520,Cancel);popup.CloseButton.name="Cancel rewarded video";cancel=popup.CloseButton;title=popup.Title;title.fontSize=21;popup.Body.text="Tamamla, +3 hamleyle devam et";popup.Body.rectTransform.sizeDelta=new Vector2(274,32);
   popup.Plate("Video picture rim",popup.Content,62,129,198,326,"D9BA77",14);var viewport=popup.Plate("Video viewport",popup.Content,65,132,192,320,"2D203D",10);viewport.gameObject.AddComponent<Mask>().showMaskGraphic=true;var pic=CrispUI.Rect("Video picture",viewport.transform,0,0,192,320);picture=pic.gameObject.AddComponent<RawImage>();picture.color=Art.Hex("2D203D");picture.raycastTarget=false;
   loadingLabel=popup.Copy("Video loading",popup.Content,75,258,172,66,"Video hazırlanıyor…",17);loadingLabel.color=Art.Hex("FFF4DB");
   popup.Plate("Video progress track",popup.Content,43,465,236,7,"DCCCA6",4);progress=popup.Plate("Video progress",popup.Content,43,465,236,7,"96AF5D",4);progress.sprite=HelpDemoArt.Disc;progress.type=Image.Type.Filled;progress.fillMethod=Image.FillMethod.Horizontal;progress.fillAmount=0;
   status=popup.Copy("Video status",popup.Content,23,478,276,32,"Video hazırlanıyor…",13);
  }
  static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x+w/2-300,500-y-h/2);return r;}
  Text TextAt(Transform parent,string name,float x,float y,float w,float h,string value,int size){var t=Rect(parent,name,x,y,w,h).gameObject.AddComponent<CrispText>();t.font=Font.CreateDynamicFontFromOSFont(new[]{"Avenir Next","Arial"},size);t.fontSize=size;t.fontStyle=name=="Video title"||name=="Cancel label"?FontStyle.Bold:FontStyle.Normal;t.text=value;t.color=Art.Hex("FFF1CE");t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;textSizes[t]=size;return t;}
 }
}
