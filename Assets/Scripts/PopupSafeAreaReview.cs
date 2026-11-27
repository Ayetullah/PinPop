#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace PinPop {
 public sealed partial class CarnivalUIReview {
  bool FullyInside(Rect bounds,Rect area)=>bounds.xMin>=area.xMin-1&&bounds.xMax<=area.xMax+1&&bounds.yMin>=area.yMin-1&&bounds.yMax<=area.yMax+1;
  void InspectSafeFrame(CarnivalPopup popup,string context){
   var emblem=popup.Stage.Find("Popup entrance/Bowling emblem");var backdrop=popup.Stage.Find("Bowling backdrop");
   Check(emblem!=null&&emblem.GetComponent<Image>().sprite.name=="bowling-emblem"&&!emblem.GetComponent<Image>().raycastTarget,context+": shared bowling emblem is present and cannot block touches");
   Check(backdrop!=null&&backdrop.GetComponent<Image>().sprite.name=="backdrop-bowling",context+": shared bowling background is present");
   Check(popup.Content.GetComponent<Image>().sprite.texture.name=="panel-settings",context+": shared cream-gold panel");
   Check(popup.Title==null||popup.Title.color.grayscale<.6f,context+": heading uses dark ink on cream");
   Check(popup.Stage.Find("Ferris wheel silhouette")==null&&popup.Stage.Find("Popup entrance/Festival canopy")==null,context+": no previous wheel or tent artwork");
   var closeArt=popup.CloseButton.targetGraphic.transform.Find("Close circle") as RectTransform;
   Check(emblem!=null&&closeArt!=null&&!Bounds(emblem.GetComponent<RectTransform>()).Overlaps(Bounds(closeArt)),context+": emblem cannot cover close artwork");
   Rect safe=CrispUI.SafeArea,app=game.GetComponent<HtmlReferenceHUD>().AppRect;
   Rect available=Rect.MinMaxRect(Mathf.Max(safe.xMin,app.xMin),Mathf.Max(safe.yMin,app.yMin),Mathf.Min(safe.xMax,app.xMax),Mathf.Min(safe.yMax,app.yMax));
   Check(FullyInside(Bounds(popup.Stage),available),context+": entire stage fits the actual safe area and game viewport");
   foreach(var rect in popup.GetComponentsInChildren<RectTransform>()){
    if(rect.name!="Popup level"&&rect.name!="Popup balance"&&rect.name!="Popup coin"&&rect.name!="Bowling emblem"&&rect.name!="Festival brand")continue;
    Check(FullyInside(Bounds(rect),available),context+": complete visible bounds of "+rect.name);
    var graphic=rect.GetComponent<Graphic>();Check(graphic!=null&&!graphic.canvasRenderer.cull,context+": header/artwork is not clipped: "+rect.name);
   }
   Check(FullyInside(Bounds(popup.Content),available)&&FullyInside(Bounds(popup.CloseButton.GetComponent<RectTransform>()),available),context+": panel and full close target fit");
  }
  IEnumerator CaptureSafePopup(string kind,string name){
   yield return null;var popup=FindPopup(kind);Check(popup!=null,"Safe-area fixture exists: "+name);if(popup==null)yield break;popup.Settle();yield return null;Inspect(kind);Capture(name);
  }
  IEnumerator PopupSafeAreaChecks(){
   var sizes=new[]{new Vector2Int(320,568),new Vector2Int(390,844),new Vector2Int(1170,2532),new Vector2Int(393,852),new Vector2Int(390,844)};
   var safeAreas=new[]{new Rect(0,0,320,568),new Rect(0,34,390,763),new Rect(0,102,1170,2289),new Rect(0,34,393,759),new Rect(16,34,358,763)};
   var adventure=game.GetComponent<AdventureHUD>();var view=game.GetComponent<HtmlReferenceHUD>();
   foreach(string language in new[]{"tr","en"})for(int sample=0;sample<sizes.Length;sample++){
    GameLocalization.SetPreference(language);Size(sizes[sample]);CrispUI.ReviewSafeArea=safeAreas[sample];yield return new WaitForSecondsRealtime(.2f);
    PurchaseWallet(1000);PlayerPrefs.SetString(PinPopAdventure.SaveKey,JsonUtility.ToJson(new AdventureSave{lessons=511}));game.Adventure.Reload();Board(11);yield return null;
    string suffix=language+"-"+sizes[sample].x+"-safe-"+sample;
    game.Hud.ShowShop();yield return CaptureSafePopup("Custom","shop-"+suffix);Click("CLOSE_SHOP");
    game.Hud.ShowSettings();yield return CaptureSafePopup("Settings","settings-game-"+suffix);
    Click("LANGUAGE_DROPDOWN");yield return new WaitForSecondsRealtime(.2f);var list=GameObject.Find("Dropdown List");Check(list!=null&&FullyInside(Bounds(list.GetComponent<RectTransform>()),CrispUI.SafeArea),"Expanded language list remains inside the corrected safe area / "+suffix);Click("LANGUAGE_LIST_CLOSE");yield return new WaitForSecondsRealtime(.2f);Click("CLOSE_SETTINGS");
    game.SetMenu(true);game.Hud.ShowHome();game.Hud.ShowSettings();yield return CaptureSafePopup("Settings","settings-home-"+suffix);Click("CLOSE_SETTINGS");game.SetMenu(false);game.Hud.ShowGame();
    game.Hud.ShowSwapShop();yield return CaptureSafePopup("Custom","swap-"+suffix);Click("SWAP_BUY_CANCEL");
    for(int id=0;id<4;id++){adventure.ShowBooster(id);yield return CaptureSafePopup("Custom","helper-"+id+"-"+suffix);Click("ADVENTURE_CLOSE");}
    game.Hud.ShowResult(false);yield return CaptureSafePopup("Failure","failure-"+suffix);game.Hud.HideResult();game.Hud.ShowResult(true);yield return CaptureSafePopup("Success","success-"+suffix);game.Hud.HideResult();
    view.Overlay("restart");yield return CaptureSafePopup("Restart","restart-"+suffix);game.Hud.CloseOverlays();
    view.Overlay("featureProgress");yield return CaptureSafePopup("Progress","progress-"+suffix);game.Hud.HideResult();Time.timeScale=1;
    typeof(AdventureHUD).GetMethod("PresentLesson",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(adventure,new object[]{GameLocalization.Text("BAĞLI ÜÇLÜ")+"\n"});yield return CaptureSafePopup("Unlock","unlock-"+suffix);Click("ADVENTURE_CLOSE");
   }
   // Exercise the entrance at a real notched resolution, not only its final pose.
   Size(new Vector2Int(1170,2532));yield return new WaitForSecondsRealtime(.2f);Board(11);CrispUI.ReviewReducedMotion=false;game.Hud.ShowShop();yield return new WaitForSecondsRealtime(.08f);Canvas.ForceUpdateCanvases();InspectSafeFrame(FindPopup("Custom"),"Animated shop entrance");yield return new WaitForSecondsRealtime(.7f);game.Hud.CloseOverlays();CrispUI.ReviewReducedMotion=true;
   yield return PurchaseChecks();
  }
 }
}
#endif
