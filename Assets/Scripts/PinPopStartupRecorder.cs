using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace PinPop {
 // Explicit -splash-review tool: records this game's render output, never the desktop.
 public sealed class PinPopStartupRecorder:MonoBehaviour {
  [Serializable] public class Frame {public float time,cover,art,readyAge;public int cameras;public string path;}
  [Serializable] public class Review {public int width=600,height=1000;public bool noOverlap=true,noCameraGap=true,homeReached,walletPreserved;public List<Frame> frames=new List<Frame>();}
  IEnumerator Start(){var game=GetComponent<PinPopGame>();string folder=Environment.GetEnvironmentVariable("PINPOP_QA_DIR");if(string.IsNullOrEmpty(folder))folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../../../Documentation"));folder=Path.Combine(folder,"VideoReview/after");Directory.CreateDirectory(folder);var review=new Review();float start=Time.realtimeSinceStartup,next=0,homeAt=-1;int coins=game.Coins,swaps=game.SwapCredits,level=game.Level;
   while(Time.realtimeSinceStartup-start<8){yield return new WaitForEndOfFrame();float elapsed=Time.realtimeSinceStartup-start;if(!PinPopSplash.Visible&&homeAt<0)homeAt=elapsed;if(elapsed>=next){next=elapsed+.08f;var splash=PinPopSplash.Instance;var frame=new Frame{time=elapsed,cover=splash==null?0:splash.CoverOpacity,art=splash==null?0:splash.ArtworkOpacity,readyAge=splash==null?PinPopSplash.LastReadyDisplaySeconds:splash.ReadyDisplaySeconds,cameras=Camera.allCamerasCount,path="frame-"+review.frames.Count.ToString("D4")+".png"};review.noOverlap&=frame.cover>=.999f||frame.art<.001f;review.noCameraGap&=frame.cameras>0;SaveFrame(game,splash,Path.Combine(folder,frame.path));review.frames.Add(frame);}if(homeAt>=0&&elapsed-homeAt>.65f)break;}
   review.homeReached=!PinPopSplash.Visible&&game.InMenu&&game.Hud.HomeVisible;review.walletPreserved=game.Coins==coins&&game.SwapCredits==swaps&&game.Level==level;File.WriteAllText(Path.Combine(folder,"frames.json"),JsonUtility.ToJson(review,true));Debug.Log("PINPOP STARTUP REVIEW: home="+review.homeReached+" overlap="+!review.noOverlap+" cameraGap="+!review.noCameraGap);Application.Quit(review.homeReached&&review.noOverlap&&review.noCameraGap&&review.walletPreserved?0:1);
  }
  static void SaveFrame(PinPopGame game,PinPopSplash splash,string path){Canvas.ForceUpdateCanvases();var rt=new RenderTexture(600,1000,24);var old=RenderTexture.active;game.Camera.targetTexture=rt;game.Camera.Render();game.Hud.UICamera.targetTexture=rt;game.Hud.UICamera.Render();if(splash!=null){splash.SplashCamera.targetTexture=rt;splash.SplashCamera.Render();splash.SplashCamera.targetTexture=null;}RenderTexture.active=rt;var image=new Texture2D(600,1000,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,600,1000),0,0);image.Apply();File.WriteAllBytes(path,image.EncodeToPNG());game.Camera.targetTexture=null;game.Hud.UICamera.targetTexture=null;RenderTexture.active=old;rt.Release();Destroy(rt);Destroy(image);}
 }
}
