#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace PinPop {
 // Opt-in device diagnostics. Absent from a production player; never awards or spends currency.
 public sealed class DevicePolishReview:MonoBehaviour {
  [Serializable] public class Section {public string name;public int frames,over50ms,over100ms;public float medianMs,p95Ms,p99Ms,maxMs;[NonSerialized]public List<float> samples=new List<float>();}
  [Serializable] public class Work {public string name;public float ms;}
  [Serializable] public class Hitch {public float time,ms,splashAge;public string stage;}
  [Serializable] public class Report {public string device,version,language,deviceLanguage,aimGeometry;public int width,height;public float readyAtSeconds,homeAtSeconds;public bool complete,initialReducedMotion,walletPreserved,languagePreferencePreserved;public List<Section> sections=new List<Section>();public List<string> errors=new List<string>();public List<Hitch> hitches=new List<Hitch>();public List<Work> work=new List<Work>();}
  public static bool Active {get;private set;}static DevicePolishReview current;
  public static void RecordWork(string name,float ms){if(current!=null&&ms>=1)current.report.work.Add(new Work{name=name,ms=ms});}
  Report report=new Report();Section section;PinPopGame game;string folder;float began;int skip;bool recording=true,hadMotion,hadLanguage,restoreLanguage;string savedLanguage;int motion;UnityEngine.EventSystems.EventSystem events;bool eventsEnabled;
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Install(){string request=Path.Combine(Application.persistentDataPath,"DevicePolishReview.request");if(!File.Exists(request)&&Array.IndexOf(Environment.GetCommandLineArgs(),"-device-polish-review")<0)return;if(File.Exists(request))File.Delete(request);Active=true;var obj=new GameObject("Device polish diagnostics");DontDestroyOnLoad(obj);obj.AddComponent<DevicePolishReview>();}
  void Awake(){current=this;began=Time.realtimeSinceStartup;folder=Path.Combine(Application.persistentDataPath,"DevicePolishReview");Directory.CreateDirectory(folder);foreach(string previous in Directory.GetFiles(folder,"*.png"))File.Delete(previous);report.device=SystemInfo.deviceModel;report.version=Application.unityVersion;report.language=GameLocalization.LanguageCode;report.deviceLanguage=Application.systemLanguage.ToString();report.width=Screen.width;report.height=Screen.height;Application.logMessageReceived+=OnLog;Begin("startup");Save();}
  void Begin(string name){section=new Section{name=name};report.sections.Add(section);}
  void Update(){if(!recording)return;if(skip>0){skip--;return;}section?.samples.Add(Time.unscaledDeltaTime*1000);if(Time.unscaledDeltaTime>.033f)report.hitches.Add(new Hitch{time=Time.realtimeSinceStartup-began,ms=Time.unscaledDeltaTime*1000,splashAge=PinPopSplash.Instance==null?-1:PinPopSplash.Instance.DisplaySeconds,stage=PinPopSplash.Instance==null?section?.name:PinPopSplash.Instance.CurrentStage});}
  void OnLog(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert){report.errors.Add(message+"\n"+stack);Save();}}
  IEnumerator Observe(string name,float seconds){Begin(name);float end=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<end)yield return null;}
  IEnumerator Start(){
   while((game=FindFirstObjectByType<PinPopGame>())==null||!game.Initialized)yield return null;
   events=UnityEngine.EventSystems.EventSystem.current;if(events!=null){eventsEnabled=events.enabled;events.enabled=false;}report.readyAtSeconds=Time.realtimeSinceStartup-began;report.initialReducedMotion=game.ReducedMotion;hadMotion=PlayerPrefs.HasKey("ReducedMotion");motion=PlayerPrefs.GetInt("ReducedMotion",0);CrispUI.ReviewReducedMotion=false;
   // Do not capture during cold startup: PNG readback would add an artificial hitch.
   while(PinPopSplash.Visible){if(PinPopSplash.Instance.Ready&&section.name=="startup")Begin("splash-reveal");yield return null;}report.homeAtSeconds=Time.realtimeSinceStartup-began;
   int coins=game.Coins,swaps=game.SwapCredits,initialLevel=game.Level;
   yield return Observe("home",2);Capture("home");
   Begin("level-transition");game.Hud.PlayLevel(3);bool transitionCaptured=false;while(game.Transitioning){var curtain=FindFirstObjectByType<BowlingTransition>();if(!transitionCaptured&&curtain!=null&&curtain.Progress>=.56f){Capture("level-transition");transitionCaptured=true;}yield return null;}
   yield return Observe("board-entrance",1.1f);Capture("game");
   yield return Observe("game-idle",2);
   Begin("aim");Vector2 start=game.Camera.WorldToScreenPoint(game.LaunchPosition);game.BeginPullForVerification(start);float time=0;
   while(time<1.5f){time+=Time.unscaledDeltaTime;game.UpdatePull(start+Vector2.down*(12+128*(.2f+.8f*Mathf.PingPong(time,1)))*game.Camera.pixelHeight/1000f);yield return null;}if(!AimVisualAudit.Verify(game,out report.aimGeometry))report.errors.Add(report.aimGeometry);Capture("aim");game.CancelPull();
   Begin("shop-open");game.Hud.ShowShop();yield return new WaitForSecondsRealtime(1.2f);Capture("shop");game.Hud.CloseOverlays();
   Begin("settings-open");game.Hud.ShowSettings();yield return new WaitForSecondsRealtime(.7f);Capture("settings");hadLanguage=PlayerPrefs.HasKey(GameLocalization.PreferenceKey);savedLanguage=PlayerPrefs.GetString(GameLocalization.PreferenceKey);restoreLanguage=true;
   foreach(string code in new[]{"en","tr"}){Begin("settings-language-"+code);Tap(GameObject.Find("LANGUAGE_DROPDOWN"));yield return new WaitForSecondsRealtime(.2f);var popup=GameObject.Find("Dropdown List");if(popup==null){report.errors.Add("Language dropdown did not open");continue;}if(code=="en")Capture("settings-language-list");var option=Array.Find(popup.GetComponentsInChildren<Toggle>(),x=>x.GetComponentInChildren<Text>().text==GameLocalization.NativeLanguageName(code));if(option==null)report.errors.Add("Language option missing: "+code);else Tap(option.gameObject);yield return new WaitForSecondsRealtime(.5f);if(GameLocalization.PreferenceCode!=code||GameLocalization.LanguageCode!=code||!game.Hud.SettingsVisible||!game.Paused||GameObject.Find("Dropdown List")!=null)report.errors.Add("Language option failed to update paused settings: "+code);Capture("settings-"+code);}
   RestoreLanguage();yield return null;game.Hud.CloseOverlays();
   var adventure=game.GetComponent<AdventureHUD>();
   for(int id=0;id<4;id++){Begin("helper-"+id);adventure.ShowBooster(id);yield return new WaitForSecondsRealtime(.7f);Capture("helper-"+id);adventure.Close();}
   Begin("map-open");adventure.ShowMap();yield return new WaitForSecondsRealtime(1.3f);Capture("map");adventure.Close();
   var lesson=typeof(AdventureHUD).GetMethod("PresentLesson",BindingFlags.NonPublic|BindingFlags.Instance);int index=0;
   foreach(string title in new[]{"SEKME ENGELLERİ","BUZLU DUBA","BAĞLI ÜÇLÜ","HAREKETLİ BARİYER"}){Begin("lesson-"+index);lesson.Invoke(adventure,new object[]{title+"\nDoğru rengi seç, doğru anda yuvarla."});yield return new WaitForSecondsRealtime(2);Capture("lesson-"+index++);adventure.Close();}
   game.LoadLevel(0);
   for(int step=0;step<3;step++){
    Begin(step==0?"how-to":"how-to-close-"+step);game.Hud.ShowHelp();yield return new WaitForSecondsRealtime(.4f);var tutorial=FindFirstObjectByType<AnimatedHowTo>();tutorial.Select(step);yield return new WaitForSecondsRealtime(.4f);Capture(step==0?"how-to":"how-to-"+step);
    var button=GameObject.Find("HELP_CLOSE");var rect=button.GetComponent<RectTransform>();var corners=new Vector3[4];rect.GetWorldCorners(corners);var low=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[0]);var high=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[2]);var safe=Screen.safeArea;
    if(!safe.Contains(low)||!safe.Contains(high))report.errors.Add("Tutorial close cross is outside the device safe area");
    var pointer=new UnityEngine.EventSystems.PointerEventData(events){position=(low+high)*.5f,button=UnityEngine.EventSystems.PointerEventData.InputButton.Left};var hits=new List<UnityEngine.EventSystems.RaycastResult>();Canvas.ForceUpdateCanvases();events.RaycastAll(pointer,hits);var target=hits.Count>0?UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<UnityEngine.EventSystems.IPointerClickHandler>(hits[0].gameObject):null;
    int moves=game.Moves,balance=game.Coins;var queue=game.Queue.ToArray();
    if(target==null||target!=button)report.errors.Add("Tutorial close cross did not receive the pointer in scene "+step);else UnityEngine.EventSystems.ExecuteEvents.Execute(target,pointer,UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
    bool unchanged=game.Moves==moves&&game.Coins==balance&&game.Queue.Count==queue.Length;for(int i=0;i<queue.Length&&unchanged;i++)unchanged=queue[i]==game.Queue[i];
    if(game.Hud.HelpVisible||game.Paused||!unchanged)report.errors.Add("Closing tutorial did not resume unchanged gameplay in scene "+step);game.Hud.CloseOverlays();
   }
   Begin("result");game.Hud.ShowResult(true);yield return new WaitForSecondsRealtime(1.5f);Capture("result");game.Hud.HideResult();
   Begin("milestone-result");int priorChest=game.Adventure.LastChest,priorChestCoins=game.Adventure.LastChestCoins;typeof(PinPopAdventure).GetProperty("LastChest").SetValue(game.Adventure,4);typeof(PinPopAdventure).GetProperty("LastChestCoins").SetValue(game.Adventure,100);game.Hud.ShowResult(true);yield return new WaitForSecondsRealtime(.7f);Capture("milestone-result");game.Hud.HideResult();typeof(PinPopAdventure).GetProperty("LastChest").SetValue(game.Adventure,priorChest);typeof(PinPopAdventure).GetProperty("LastChestCoins").SetValue(game.Adventure,priorChestCoins);
   Begin("failure");game.Hud.ShowResult(false);yield return new WaitForSecondsRealtime(.7f);Capture("failure");game.Hud.HideResult();
   Begin("swap-shop");game.Hud.ShowSwapShop();yield return new WaitForSecondsRealtime(.7f);Capture("swap-shop");game.Hud.CloseOverlays();
   game.SetMenu(true);game.LoadLevel(initialLevel);game.Hud.ShowHome();report.walletPreserved=coins==game.Coins&&swaps==game.SwapCredits;report.languagePreferencePreserved=hadLanguage==PlayerPrefs.HasKey(GameLocalization.PreferenceKey)&&savedLanguage==PlayerPrefs.GetString(GameLocalization.PreferenceKey);RestoreMotion();recording=false;report.complete=true;Save();Active=false;if(events!=null)events.enabled=eventsEnabled;Debug.Log("PINPOP DEVICE POLISH REVIEW COMPLETE: "+folder);
  }
  void Capture(string name,bool splash=false){
   skip=3;Canvas.ForceUpdateCanvases();int w=Screen.width,h=Screen.height;var rt=RenderTexture.GetTemporary(w,h,24);var prior=RenderTexture.active;Texture2D image=null;
   try {foreach(var camera in new[]{game.Camera,game.Hud.UICamera,splash?PinPopSplash.Instance?.SplashCamera:null}){if(camera==null)continue;var target=camera.targetTexture;camera.targetTexture=rt;camera.Render();camera.targetTexture=target;}RenderTexture.active=rt;image=new Texture2D(w,h,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,w,h),0,0);image.Apply();File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG());}
   finally {RenderTexture.active=prior;RenderTexture.ReleaseTemporary(rt);if(image!=null)Destroy(image);}
   Save();
  }
  void Save(){foreach(var item in report.sections){var values=new List<float>(item.samples);values.Sort();item.frames=values.Count;if(values.Count==0)continue;item.medianMs=values[values.Count/2];item.p95Ms=values[Mathf.Min(values.Count-1,Mathf.FloorToInt(values.Count*.95f))];item.p99Ms=values[Mathf.Min(values.Count-1,Mathf.FloorToInt(values.Count*.99f))];item.maxMs=values[values.Count-1];item.over50ms=values.FindAll(x=>x>50).Count;item.over100ms=values.FindAll(x=>x>100).Count;}File.WriteAllText(Path.Combine(folder,"frame-health.json"),JsonUtility.ToJson(report,true));}
  void Tap(GameObject expected){Canvas.ForceUpdateCanvases();if(expected==null){report.errors.Add("Missing language control");return;}var rect=expected.GetComponent<RectTransform>();var pointer=new UnityEngine.EventSystems.PointerEventData(events){position=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,rect.TransformPoint(rect.rect.center)),button=UnityEngine.EventSystems.PointerEventData.InputButton.Left};var hits=new List<UnityEngine.EventSystems.RaycastResult>();events.RaycastAll(pointer,hits);var target=hits.Count==0?null:UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<UnityEngine.EventSystems.IPointerClickHandler>(hits[0].gameObject);if(target!=expected)report.errors.Add("Language control did not receive pointer: "+expected.name);else UnityEngine.EventSystems.ExecuteEvents.Execute(target,pointer,UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);}
  void RestoreMotion(){if(hadMotion)CrispUI.ReviewReducedMotion=(motion)==1;else CrispUI.ReviewReducedMotion=null;}
  void RestoreLanguage(){if(!restoreLanguage)return;GameLocalization.SetPreference(hadLanguage&&savedLanguage=="en"?"en":hadLanguage&&savedLanguage=="tr"?"tr":"auto");if(!hadLanguage)PlayerPrefs.DeleteKey(GameLocalization.PreferenceKey);else PlayerPrefs.SetString(GameLocalization.PreferenceKey,savedLanguage);PlayerPrefs.Save();restoreLanguage=false;}
  void OnDestroy(){Application.logMessageReceived-=OnLog;if(game!=null){RestoreMotion();RestoreLanguage();}Active=false;if(events!=null)events.enabled=eventsEnabled;}
 }
}
#endif
