#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace PinPop {
 // Audit fixture, compiled only in the isolated review player. Not a shipped game change.
 public sealed class DeepAuditReview:MonoBehaviour {
  [Serializable]public class Observation {public string id,detail;public bool reproduced;}
  [Serializable]public class Audit {public List<Observation> observations=new List<Observation>();public List<string> errors=new List<string>();public bool complete,restored;}
  [Serializable]class Saved {public string key,value;public bool integer,exists;}
  List<Saved> saved=new List<Saved>();Audit audit=new Audit();PinPopGame game;string folder;RenderTexture screen;bool restored;
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(Array.IndexOf(Environment.GetCommandLineArgs(),"-deep-audit-test")>=0)new GameObject("Deep audit").AddComponent<DeepAuditReview>();}
  void Observe(string id,bool reproduced,string detail){audit.observations.Add(new Observation{id=id,reproduced=reproduced,detail=detail});Debug.Log("AUDIT "+id+" reproduced="+reproduced+" "+detail);Save();}
  void Remember(string key,bool integer){saved.Add(new Saved{key=key,integer=integer,exists=PlayerPrefs.HasKey(key),value=integer?PlayerPrefs.GetInt(key).ToString():PlayerPrefs.GetString(key)});}
  IEnumerator Start(){
   folder=Environment.GetEnvironmentVariable("PINPOP_QA_DIR")??"/tmp/pinpop-deep-probes";Directory.CreateDirectory(folder);Application.logMessageReceived+=Log;
   while((game=FindFirstObjectByType<PinPopGame>())==null||!game.Initialized||PinPopSplash.Visible)yield return null;
   foreach(string k in new[]{PinPopEconomy.SaveKey,PinPopAdventure.SaveKey,GameLocalization.PreferenceKey})Remember(k,false);foreach(string k in new[]{"Level","Coins","SwapCredits","ReducedMotion","Muted",PinPopHUD.TutorialSeenKey})Remember(k,true);
   GameLocalization.SetPreference("tr");PlayerPrefs.SetInt("ReducedMotion",1);PlayerPrefs.SetInt(PinPopHUD.TutorialSeenKey,1);
   Size(1170,2532);yield return new WaitForSecondsRealtime(.3f);var adventure=game.GetComponent<AdventureHUD>();
   // An ordinary trajectory toward an existing pin behind the level-three bumper.
   Board(2);yield return null;game.SetLaunchX(-1.75f);var targetPoint=new Vector3(-1.75f,.4f,2);var origin=game.LaunchPosition;var prediction=game.PredictRoll(targetPoint,1,origin);
   RaycastHit hit;bool barrierHit=Physics.SphereCast(origin,.39f,(targetPoint-origin).normalized,out hit,10);
   var dir=(targetPoint-origin);dir.y=0;dir.Normalize();float predictedAlong=Vector3.Dot(prediction.Point-origin,dir);
   bool ignores=barrierHit&&hit.collider.name.Contains("bumper")&&predictedAlong>hit.distance+.3f;
   Observe("aim-through-bumper",ignores,"First physical contact="+(barrierHit?hit.collider.name:"none")+", distance="+(barrierHit?hit.distance:0)+", aim distance="+predictedAlong);
   var anchor=(Vector2)game.Camera.WorldToScreenPoint(origin);game.BeginPullForVerification(anchor);var aimDir=new Vector2(-dir.x, -dir.z).normalized;game.UpdatePull(anchor+new Vector2(aimDir.x*140*game.Camera.pixelWidth/600,aimDir.y*140*game.Camera.pixelHeight/1000));Capture("aim-bumper");game.CancelPull();
   // Twelve real pin collection callbacks in a single frame, as a multi-clear can produce.
   Board(11);yield return null;game.PrepareShotColor(-1);for(int i=0;i<12;i++)game.Pins[i].Body.position=new Vector3(game.Pins[i].Body.position.x,-2,game.Pins[i].Body.position.z);yield return null;Time.timeScale=0;game.MatchTray.Tick(.49f);yield return null;Canvas.ForceUpdateCanvases();
   var tray=Bounds(GameObject.Find("Collection tray").GetComponent<RectTransform>());int outside=0;foreach(var image in FindObjectsByType<Image>(FindObjectsSortMode.None))if(image.name.StartsWith("Collected color ")){var b=Bounds(image.rectTransform);if(b.xMax>tray.xMax+1||b.xMin<tray.xMin-1)outside++;}
   Observe("pending-matches-outside-tray",outside>0,"Collected="+game.Cleared+", items="+game.MatchTray.Items.Count+", unmatched="+game.MatchTray.LiveCount+", icons beyond tray="+outside);Capture("tray-multi-collect");int overlaps=0;var comboLabels=new List<Text>();foreach(var label in FindObjectsByType<Text>(FindObjectsSortMode.None))if(label.name=="Triple match")comboLabels.Add(label);for(int i=0;i<comboLabels.Count;i++)for(int j=0;j<i;j++)if(Bounds(comboLabels[i].rectTransform).Overlaps(Bounds(comboLabels[j].rectTransform)))overlaps++;Observe("combo-label-overlap",overlaps>0,"Simultaneous combo labels="+comboLabels.Count+", overlapping label pairs="+overlaps);
   // Fill ten unmatched slots using actual pin callbacks. No protected-color filtering is bypassed.
   Board(11);yield return null;game.PrepareShotColor(-1);for(int c=0;c<5;c++){var pins=game.Pins.FindAll(x=>x.ColorId==c);for(int i=0;i<2;i++)pins[i].Body.position=new Vector3(pins[i].Body.position.x,-2,pins[i].Body.position.z);}yield return null;yield return new WaitForSecondsRealtime(.3f);
   var card=Bounds(GameObject.Find("Recovery border").GetComponent<RectTransform>());var home=Bounds(GameObject.Find("RESULT_HOME").GetComponent<RectTransform>());string status=GameObject.Find("resultReward").GetComponent<Text>().text;
   Observe("overflow-failure-layout",game.TrayOverflow&&(home.yMin<card.yMin||status.Contains("1 kez")),"Overflow="+game.TrayOverflow+", home bottom="+home.yMin+", card bottom="+card.yMin+", status="+status+", arrived="+game.MatchTray.Items.FindAll(x=>x.Arrived).Count+"/"+game.MatchTray.Items.Count);Capture("overflow-result");
   // Repeated normal level builds: iced-pin materials must not accumulate after old pins are destroyed.
   Board(4);yield return null;int before=IceMaterials();for(int n=0;n<20;n++){game.LoadLevel(4);yield return null;}GC.Collect();GC.WaitForPendingFinalizers();yield return null;int after=IceMaterials();
   Observe("ice-material-growth",after-before>=60,"Ice material count after warmup="+before+", after twenty restarts="+after+", surviving ice shells="+game.Pins.FindAll(x=>x.Iced).Count);
   // At level one, previously seen how-to should still have an entry point if the player needs it again.
   Board(0);yield return null;int helpEntries=0;foreach(var b in FindObjectsByType<Button>(FindObjectsSortMode.None))if(b.name.Contains("HELP"))helpEntries++;
   game.Hud.ShowSettings();yield return new WaitForSecondsRealtime(.2f);foreach(var b in FindObjectsByType<Button>(FindObjectsSortMode.None))if(b.name.Contains("HELP"))helpEntries++;
   Observe("no-tutorial-replay-entry",helpEntries==0,"Visible HELP controls in gameplay and settings="+helpEntries+", tutorial already seen="+PlayerPrefs.GetInt(PinPopHUD.TutorialSeenKey));Capture("settings-no-help");game.Hud.CloseOverlays();
   // Verify real action hit-testing at the center, distinguishing old corner-based test failures.
   Board(19);yield return null;typeof(PinPopGame).GetProperty("Cleared").SetValue(game,game.Total);game.SendMessage("End",true);yield return new WaitForSecondsRealtime(.3f);var next=GameObject.Find("CONTINUE");bool center=Click(next);while(game.Transitioning)yield return null;Observe("continue-center-blocked",!center||game.Level!=20,"Center raycast="+center+", resulting level="+game.Level);
   Restore();audit.complete=true;Save();Application.Quit(audit.errors.Count==0?0:1);
  }
  void Board(int level){game.GetComponent<AdventureHUD>().Close();game.Hud.HideResult();game.Hud.CloseOverlays();game.SetMenu(false);game.LoadLevel(level);game.Hud.ShowGame();}
  int IceMaterials(){int count=0;foreach(var m in Resources.FindObjectsOfTypeAll<Material>())if(m.shader.name=="Sprites/Default"&&Mathf.Abs(m.color.a-.38f)<.001f&&Mathf.Abs(m.color.b-1)<.001f)count++;return count;}
  bool Click(GameObject expected){Canvas.ForceUpdateCanvases();var evt=new PointerEventData(EventSystem.current){position=Bounds(expected.GetComponent<RectTransform>()).center,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(evt,hits);var target=hits.Count==0?null:ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);if(target==expected)ExecuteEvents.Execute(target,evt,ExecuteEvents.pointerClickHandler);return target==expected;}
  Rect Bounds(RectTransform rect){var c=new Vector3[4];rect.GetWorldCorners(c);var a=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,c[0]);var b=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,c[2]);return Rect.MinMaxRect(a.x,a.y,b.x,b.y);}
  void Size(int w,int h){CrispUI.ReviewSize=new Vector2Int(w,h);CrispUI.ReviewSafeArea=new Rect(0,102,w,h-243);screen=new RenderTexture(w,h,24);screen.Create();game.Camera.targetTexture=screen;game.Hud.UICamera.targetTexture=screen;}
  void Capture(string name){Canvas.ForceUpdateCanvases();var old=RenderTexture.active;game.Camera.Render();game.Hud.UICamera.Render();RenderTexture.active=screen;var tex=new Texture2D(screen.width,screen.height,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,screen.width,screen.height),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(folder,name+".png"),tex.EncodeToPNG());Destroy(tex);RenderTexture.active=old;}
  void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error){audit.errors.Add(message+"\n"+stack);Restore();Save();Application.Quit(1);}}
  void Save(){File.WriteAllText(Path.Combine(folder,"observations.json"),JsonUtility.ToJson(audit,true));}
  void Restore(){if(restored)return;restored=true;foreach(var x in saved){if(!x.exists)PlayerPrefs.DeleteKey(x.key);else if(x.integer)PlayerPrefs.SetInt(x.key,int.Parse(x.value));else PlayerPrefs.SetString(x.key,x.value);}PlayerPrefs.Save();audit.restored=true;}
  void OnApplicationQuit(){Restore();}void OnDestroy(){Application.logMessageReceived-=Log;Restore();CrispUI.ReviewSize=null;CrispUI.ReviewSafeArea=null;}
 }
}
#endif
