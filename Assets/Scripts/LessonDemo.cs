using UnityEngine;
using UnityEngine.UI;
namespace PinPop {
 // A loop showing the actual mechanic, rather than an unrelated character icon.
 public sealed class LessonDemo:MonoBehaviour {
  string kind;PinPopGame game;float began;Image ball,barrier,ice,band;Image[] pins,shards;Text caption;
  public static bool Supports(string title)=>title=="SEKME ENGELLERİ"||title=="BUZLU DUBA"||title=="BAĞLI ÜÇLÜ"||title=="HAREKETLİ BARİYER";
  public void Build(string title,PinPopGame owner){kind=title;game=owner;began=Time.unscaledTime;
   CrispUI.Panel("Demo frame",transform,0,0,460,190,Art.Hex("E9D9AD"),18);
   CrispUI.Panel("Demo wood",transform,8,8,444,174,Art.Hex("F5E7BF"),13);
   pins=new Image[kind=="BAĞLI ÜÇLÜ"?3:1];for(int i=0;i<pins.Length;i++){pins[i]=Picture("Demo target "+i,210+(i-(pins.Length-1)*.5f)*45,20,40,62,Resources.Load<Sprite>("PinIcons/pin-1"));}
   ball=Picture("Demo ball",209,123,36,36,BallLooks.Icon(0,1,128));
   barrier=CrispUI.Panel("Demo barrier",transform,175,93,110,21,Art.Hex("466F82"),5);CrispUI.Panel("Barrier stripe",barrier.transform,7,7,96,5,Art.Hex("FFF1CE"),2);
   ice=CrispUI.Panel("Demo ice",transform,207,21,46,65,new Color(.32f,.81f,.95f,.55f),8);
   band=CrispUI.Panel("Demo band",transform,164,57,132,12,Art.Hex("DAAB48"),4);
   shards=new Image[8];for(int i=0;i<shards.Length;i++)shards[i]=CrispUI.Panel("Ice shard "+i,transform,220,40,5,8,Art.Hex("86CFE8"),1);
   caption=CrispUI.Text("Demo action",transform,15,160,430,26,"",20,Art.Hex("294B61"));Apply();
  }
  Image Picture(string name,float x,float y,float w,float h,Sprite sprite){var r=CrispUI.Rect(name,transform,x,y,w,h);var image=r.gameObject.AddComponent<Image>();image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;return image;}
  void At(Transform t,float x,float y){t.GetComponent<RectTransform>().anchoredPosition=new Vector2(x,-y);}
  void Update(){if(game!=null)Apply();}
  void Apply(){float t=game.ReducedMotion?2.8f:Mathf.Repeat(Time.unscaledTime-began,5);bool first=t<2.3f;float p=Mathf.SmoothStep(0,1,Mathf.Clamp01((t-(first?.3f:2.6f))/1.1f));barrier.gameObject.SetActive(kind=="SEKME ENGELLERİ"||kind=="HAREKETLİ BARİYER");ice.gameObject.SetActive(kind=="BUZLU DUBA"&&t<1.45f);band.gameObject.SetActive(kind=="BAĞLI ÜÇLÜ"&&t<1.45f);
   for(int i=0;i<shards.Length;i++){bool visible=kind=="BUZLU DUBA"&&t>=1.4f&&t<2.1f;shards[i].gameObject.SetActive(visible);if(visible){float age=(t-1.4f)/.7f,a=i*Mathf.PI/4;At(shards[i].transform,226+Mathf.Cos(a)*age*65,44+Mathf.Sin(a)*age*45);var color=shards[i].color;color.a=1-age;shards[i].color=color;}}
   foreach(var pin in pins)pin.rectTransform.localRotation=Quaternion.identity;
   if(kind=="SEKME ENGELLERİ"){if(p<.6f)At(ball.transform,Mathf.Lerp(48,100,p/.6f),Mathf.Lerp(123,75,p/.6f));else At(ball.transform,Mathf.Lerp(100,209,(p-.6f)/.4f),Mathf.Lerp(75,35,(p-.6f)/.4f));caption.text="YANDAN AÇI BUL";}
   else if(kind=="HAREKETLİ BARİYER"){At(barrier.transform,175+Mathf.Sin(t*1.8f)*110,93);At(ball.transform,209,t<1.4f?123:Mathf.Lerp(123,32,Mathf.SmoothStep(0,1,(t-1.4f)/1.2f)));caption.text=t<1.4f?"GEÇİDİ BEKLE":"AÇILINCA YUVARLA";}
   else {At(ball.transform,209,Mathf.Lerp(123,54,p));caption.text=first?(kind=="BUZLU DUBA"?"İLK ATIŞ: BUZU KIR":"İLK ATIŞ: BAĞI KOPAR"):"SONRAKİ ATIŞ: DEVİR";if(!first)foreach(var pin in pins)pin.rectTransform.localRotation=Quaternion.Euler(0,0,-65*Mathf.SmoothStep(0,1,(t-3.4f)/.7f));}
  }
 }
}
