#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace PinPop {
 public sealed partial class CarnivalUIReview {
  void PurchaseWallet(int coins,int swaps=3){new PinPopEconomy(new EconomySave{coins=coins,swaps=swaps,highestLevel=40}).Save();game.ReloadEconomy();}
  void InspectPurchase(Button button,bool small,string context){
   Canvas.ForceUpdateCanvases();var view=button.GetComponent<CarnivalPurchaseButton>();var surface=(Image)button.targetGraphic;var label=button.GetComponentInChildren<Text>();
   Check(view!=null&&view.IsPurchase&&surface.sprite.name=="button-green",context+": shared green purchase artwork");
   Check(button.GetComponent<HtmlButtonFeedback>()!=null&&button.GetComponent<PopupControlState>()!=null,context+": shared press and disabled feedback");
   Check(label.text==GameLocalization.Text("SATIN AL")+" · "+PinPopEconomy.Compact(view.Price)&&label.cachedTextGenerator.lineCount==1,context+": localized purchase caption and price fit one line");
   var icon=button.GetComponentInChildren<CrispIcon>();Check(icon!=null&&icon.Symbol==CrispIcon.Kind.Coin&&!icon.raycastTarget,context+": live coin icon follows the price");
   var visible=Bounds(surface.rectTransform);var text=Bounds(label.rectTransform);var currency=Bounds(icon.rectTransform);Check(text.xMin>=visible.xMin-1&&currency.xMax<=visible.xMax+1&&text.xMax<=currency.xMin+1,context+": label and currency do not overlap or escape artwork");
   if(!small){var popup=button.GetComponentInParent<CarnivalPopup>();Check(popup.Content.sizeDelta==new Vector2(322,420)&&popup.CardTop==230,context+": shared panel size and heading position");Check(button.GetComponent<RectTransform>().anchoredPosition==new Vector2(59,-668),context+": shared bottom action position");}
  }
  IEnumerator PurchaseChecks(){
   var adventure=game.GetComponent<AdventureHUD>();
   foreach(string language in new[]{"tr","en"})foreach(var size in new[]{new Vector2Int(320,568),new Vector2Int(390,844),new Vector2Int(1170,2532)}){
    GameLocalization.SetPreference(language);Size(size);yield return new WaitForSecondsRealtime(.2f);PurchaseWallet(1000);PlayerPrefs.SetString(PinPopAdventure.SaveKey,JsonUtility.ToJson(new AdventureSave{lessons=511}));game.Adventure.Reload();Board(11);yield return null;
    string suffix=language+"-"+size.x;
    game.Hud.ShowSwapShop();yield return null;FindPopup("Custom").Settle();yield return null;Inspect("Custom");var swap=GameObject.Find("SWAP_BUY_CONFIRM").GetComponent<Button>();InspectPurchase(swap,false,"Swap / "+suffix);Capture("swap-"+suffix);
    int moves=game.Moves,attempt=game.AttemptId;Click("SWAP_BUY_CONFIRM");yield return null;Check(game.Coins==990&&game.SwapCredits==4&&game.Hud.SwapShopVisible&&game.Paused&&game.Moves==moves&&game.AttemptId==attempt,"Swap purchase charges once, preserves turn and leaves popup open / "+suffix);Click("SWAP_BUY_CANCEL");Check(!game.Paused,"Swap cross resumes gameplay / "+suffix);
    for(int id=0;id<4;id++){
     adventure.ShowBooster(id);yield return null;FindPopup("Custom").Settle();yield return null;Inspect("Custom");var buy=GameObject.Find("BOOSTER_BUY").GetComponent<Button>();InspectPurchase(buy,false,"Helper "+id+" / "+suffix);Capture("helper-"+id+"-"+suffix);
     int balance=game.Coins,stock=game.Adventure.Data.stock[id];Click("BOOSTER_BUY");yield return null;Check(adventure.ModalVisible&&game.Paused&&game.Coins==balance-AdventureRules.Prices[id]&&game.Adventure.Data.stock[id]==stock+1,"Helper purchase charges once, adds inventory and stays open: "+id+" / "+suffix);Click("ADVENTURE_CLOSE");
    }
    for(int item=0;item<9;item++)game.MatchTray.Items.Add(new PinPopMatchTray.Item{Id=200+item,Color=item%PinPopPalette.Count,Arrived=true,Presented=true,Age=1});
    adventure.ShowBooster(2);yield return null;FindPopup("Custom").Settle();yield return null;Inspect("Custom");Capture("pocket-full-grid-"+suffix);var last=GameObject.Find("POCKET_CHOICE_208").GetComponent<Button>();Check(Bounds(last.GetComponent<RectTransform>()).yMin>Bounds(FindPopup("Custom").Content).yMin,"Last row of a nine-item pocket choice fits inside the shared panel / "+suffix);Click("POCKET_CHOICE_208");Check(!adventure.ModalVisible&&!game.Paused&&game.Adventure.PocketColor==0,"Last-row pocket selection stays reachable and closes after use / "+suffix);game.MatchTray.Clear();
    game.Hud.ShowShop();yield return null;FindPopup("Custom").Settle();yield return null;Inspect("Custom");for(int i=1;i<4;i++)InspectPurchase(GameObject.Find("BALL_BUY_"+i).GetComponent<Button>(),true,"Catalog "+i+" / "+suffix);Capture("catalog-"+suffix);
    int before=game.Coins;Click("BALL_BUY_1");yield return null;Check(game.OwnsBall(1)&&game.SelectedBall==1&&game.Coins==before-PinPopEconomy.BallPrices[1]&&game.Hud.ShopVisible&&game.Paused,"Catalog purchase equips style and keeps catalog open / "+suffix);
    Check(!GameObject.Find("BALL_BUY_1").GetComponent<CarnivalPurchaseButton>().IsPurchase&&!GameObject.Find("BALL_BUY_1").GetComponent<Button>().interactable,"Owned selected style switches to neutral selected state / "+suffix);Click("BALL_BUY_0");Check(game.SelectedBall==0&&game.Coins==before-PinPopEconomy.BallPrices[1],"Selecting owned style does not charge again / "+suffix);Click("CLOSE_SHOP");
   }
   Size(new Vector2Int(390,844));yield return new WaitForSecondsRealtime(.2f);Board(11);PurchaseWallet(0);game.Hud.ShowSwapShop();yield return null;var blocked=GameObject.Find("SWAP_BUY_CONFIRM").GetComponent<Button>();Check(!blocked.interactable&&!game.BuySwap()&&game.Coins==0,"Zero balance disables and rejects swap purchase");Capture("swap-insufficient");game.Hud.CloseOverlays();
   PurchaseWallet(PinPopEconomy.SwapCost,998);game.Hud.ShowSwapShop();yield return null;Click("SWAP_BUY_CONFIRM");yield return null;Check(game.Coins==0&&game.SwapCredits==999&&!GameObject.Find("SWAP_BUY_CONFIRM").GetComponent<Button>().interactable&&game.Hud.SwapShopVisible,"Exact swap price purchases the last stock slot and disables further purchase");game.Hud.CloseOverlays();
   PurchaseWallet(1000,999);game.Hud.ShowSwapShop();yield return null;blocked=GameObject.Find("SWAP_BUY_CONFIRM").GetComponent<Button>();Check(!blocked.interactable&&!game.BuySwap()&&game.Coins==1000&&game.SwapCredits==999,"Full swap stock blocks purchase without charging");Capture("swap-full");game.Hud.CloseOverlays();
   for(int id=0;id<4;id++){
    PurchaseWallet(0);game.Adventure.Data.stock[id]=0;adventure.ShowBooster(id);yield return null;blocked=GameObject.Find("BOOSTER_BUY").GetComponent<Button>();Check(!blocked.interactable&&!game.Adventure.Buy(id)&&game.Coins==0&&game.Adventure.Data.stock[id]==0,"Insufficient helper purchase is disabled and rejected: "+id);Check(Mathf.Abs(blocked.targetGraphic.GetComponent<CanvasGroup>().alpha-.6f)<.001f,"Disabled helper label, coin and artwork dim together: "+id);Capture("helper-empty-"+id);adventure.Close();
    PurchaseWallet(AdventureRules.Prices[id]);game.Adventure.Data.stock[id]=998;adventure.ShowBooster(id);yield return null;Click("BOOSTER_BUY");yield return null;Check(game.Coins==0&&game.Adventure.Data.stock[id]==999&&!GameObject.Find("BOOSTER_BUY").GetComponent<Button>().interactable&&adventure.ModalVisible,"Exact helper price buys last stock slot and refreshes disabled state: "+id);adventure.Close();
    PurchaseWallet(1000);game.Adventure.Data.stock[id]=999;adventure.ShowBooster(id);yield return null;blocked=GameObject.Find("BOOSTER_BUY").GetComponent<Button>();Check(!blocked.interactable&&!game.Adventure.Buy(id)&&game.Coins==1000&&game.Adventure.Data.stock[id]==999,"Full helper stock blocks purchase without charging: "+id);Capture("helper-full-"+id);adventure.Close();game.Adventure.Data.stock[id]=3;
   }
   PurchaseWallet(0);game.Hud.ShowShop();yield return null;for(int i=1;i<4;i++)Check(!GameObject.Find("BALL_BUY_"+i).GetComponent<Button>().interactable&&!game.BuyBall(i)&&game.Coins==0,"Unowned catalog style blocks insufficient purchase: "+i);Capture("catalog-insufficient");game.Hud.CloseOverlays();
   PurchaseWallet(1000);CrispUI.ReviewReducedMotion=false;game.Hud.ShowSwapShop();yield return new WaitForSecondsRealtime(.7f);var press=GameObject.Find("SWAP_BUY_CONFIRM").GetComponent<HtmlButtonFeedback>();var pointer=new PointerEventData(events);press.OnPointerDown(pointer);yield return new WaitForSecondsRealtime(.12f);Check(press.Artwork.localScale.x<1&&game.Paused,"Purchase artwork responds to a press while gameplay is paused");press.OnPointerExit(pointer);yield return new WaitForSecondsRealtime(.2f);Check(Vector3.Distance(press.Artwork.localScale,Vector3.one)<.001f,"Interrupted purchase press restores the shared artwork");game.Hud.CloseOverlays();CrispUI.ReviewReducedMotion=true;
   yield return HelperUseChecks();
  }
 }
}
#endif
