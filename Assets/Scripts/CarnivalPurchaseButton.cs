using UnityEngine;
using UnityEngine.UI;

namespace PinPop {
 // One purchase presentation for swap, helpers and the compact catalog cards.
 // Economy logic remains with the caller; this component only presents its state.
 public sealed class CarnivalPurchaseButton:MonoBehaviour {
  Image surface;Text label;CrispIcon coin;Sprite green,cream;Shadow shadow;
  bool compact,selection;int price;string caption;float height;
  public int Price=>price;
  public bool IsPurchase=>!selection;
  public void Bind(Image artwork,Text text,CrispIcon symbol,Sprite neutral,bool small,float h){
   surface=artwork;label=text;coin=symbol;cream=neutral;green=artwork.sprite;compact=small;height=h;shadow=label.GetComponent<Shadow>();Refresh();
  }
  public void SetOffer(int value,string status=null){if(!selection&&price==value&&caption==status&&surface!=null)return;selection=false;price=value;caption=status;Refresh();}
  public void SetSelection(string value){if(selection&&caption==value)return;selection=true;caption=value;Refresh();}
  void OnEnable(){GameLocalization.Changed+=Refresh;Refresh();}
  void OnDisable(){GameLocalization.Changed-=Refresh;}
  void Refresh(){
   if(surface==null)return;
   surface.sprite=selection?cream:green;surface.type=selection?Image.Type.Sliced:Image.Type.Simple;
   bool priced=!selection&&string.IsNullOrEmpty(caption);coin.gameObject.SetActive(priced);
   label.text=priced?GameLocalization.Text("SATIN AL")+" · "+PinPopEconomy.Compact(price):GameLocalization.Text(caption??"SATIN AL");
   label.color=Art.Hex(selection?"79583E":"FFFBEA");if(shadow!=null)shadow.effectColor=selection?Color.clear:new Color(.2f,.16f,.15f,.55f);
   label.horizontalOverflow=HorizontalWrapMode.Overflow;label.verticalOverflow=VerticalWrapMode.Truncate;
   float symbol=compact?14:20,gap=compact?4:6,width=surface.rectTransform.rect.width,padding=compact?6:16;
   float available=width-padding*2-(priced?symbol+gap:0);int min=compact?10:15;label.fontSize=compact?12:19;
   while(label.fontSize>min&&label.preferredWidth>available)label.fontSize--;
   float textWidth=Mathf.Min(available,label.preferredWidth+2),total=textWidth+(priced?symbol+gap:0),left=(width-total)/2;
   label.rectTransform.anchoredPosition=new Vector2(left,0);label.rectTransform.sizeDelta=new Vector2(textWidth,height);
   coin.rectTransform.anchoredPosition=new Vector2(left+textWidth+gap,-(height-symbol)/2);coin.rectTransform.sizeDelta=Vector2.one*symbol;
  }
 }
}
