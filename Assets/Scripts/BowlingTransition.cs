using UnityEngine;
using UnityEngine.UI;

namespace PinPop {
 [RequireComponent(typeof(RectTransform),typeof(CanvasRenderer))]
 public sealed class BowlingTransition:MaskableGraphic {
  public const float Duration=1.4f;
  CanvasGroup fade;RectTransform art;Text levelLabel;float started,scale;bool reduced;
  readonly RectTransform[] pins=new RectTransform[4];RectTransform ball;
  readonly Vector2[] rests={new Vector2(160,592),new Vector2(282,592),new Vector2(353,600),new Vector2(449,592)};
  static readonly Color Teal=Art.Hex("25CCD0"),Pink=Art.Hex("F56298"),Gold=Art.Hex("FFD05B"),Purple=Art.Hex("AA82FA");
  public float Progress=>Mathf.Clamp01((Time.unscaledTime-started)/Duration);
  protected override void OnEnable(){if(GetComponent<CanvasRenderer>()==null)gameObject.AddComponent<CanvasRenderer>();base.OnEnable();}
  public void Build(Font font){
   fade=gameObject.AddComponent<CanvasGroup>();raycastTarget=true;
   art=CrispUI.Rect("Transition composition",transform,0,0,600,1000);art.anchorMin=art.anchorMax=art.pivot=Vector2.one*.5f;art.anchoredPosition=Vector2.zero;
   Label("PINPOP",190,26,"FFD05B");Label("YENİ MASA HAZIR",254,19,"5CE1DC");levelLabel=Label("BÖLÜM 1",304,48,"FFF6DF",72);
   Label("Renkler hazır. Sıra sende.",384,19,"DAEBF4",38,false);
   for(int i=0;i<4;i++){pins[i]=Picture("Transition character "+i,Resources.Load<Sprite>("PinIcons/pin-"+(i==0?2:i==3?1:0)),84,112);pins[i].pivot=new Vector2(.5f,0);}
   ball=Picture("Transition bowling ball",BallLooks.Icon(0,0,512),90,90);
   Label("AYNI RENK. TAM İSABET.",790,23,"FFD05B");Label("Çek, nişan al, yuvarla.",839,19,"E0EFF6",38,false);Label("BOWLING FESTİVALİ",925,19,"93B9D0",30,false);
   Fit();
   float density=Mathf.Clamp(Mathf.Ceil(scale*4)/4,1,4);
   foreach(var text in art.GetComponentsInChildren<Text>(true))text.font.RequestCharactersInTexture(text.text+"0123456789",Mathf.RoundToInt(text.fontSize*density),text.fontStyle);
  }
  Text Label(string value,float y,int size,string hex,float h=44,bool bold=true){return CrispUI.Text(value,art,35,y,530,h,value,Mathf.Max(19,size),Art.Hex(hex),bold);}
  RectTransform Picture(string name,Sprite sprite,float w,float h){var r=CrispUI.Rect(name,art,0,0,w,h);r.pivot=Vector2.one*.5f;var image=r.gameObject.AddComponent<Image>();image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;return r;}
  void Fit(){if(art==null)return;float next=Mathf.Min(rectTransform.rect.width/600,rectTransform.rect.height/1000);if(!Mathf.Approximately(next,scale)){scale=next;art.localScale=Vector3.one*scale;}}
  public void Show(int level,bool reduceMotion,float shownAt){started=shownAt;reduced=reduceMotion;levelLabel.text=GameLocalization.Format("BÖLÜM {0}",level+1);gameObject.SetActive(true);transform.SetAsLastSibling();ApplyVisualState();}
  void Update(){ApplyVisualState();}
  void ApplyVisualState(){if(fade==null)return;Fit();float p=Progress;fade.alpha=reduced?1:Mathf.SmoothStep(0,1,p/.10f)*(1-Mathf.SmoothStep(0,1,(p-.87f)/.13f));
   float t=reduced?.35f:p,hit=Mathf.SmoothStep(0,1,Mathf.Clamp01((t-.48f)/.32f));
   for(int i=0;i<4;i++){float impact=i==1||i==2?hit:0;float side=i==1?-1:1;float bounce=reduced?0:Mathf.Sin(Mathf.Clamp01(t/.24f)*Mathf.PI)*12;
    pins[i].anchoredPosition=new Vector2(rests[i].x+side*impact*48,-rests[i].y+impact*24+bounce);pins[i].localRotation=Quaternion.Euler(0,0,side*impact*72);pins[i].localScale=Vector3.one*(1-impact*.12f);}
   float roll=Mathf.SmoothStep(0,1,Mathf.Clamp01(t/.53f));ball.anchoredPosition=new Vector2(Mathf.Lerp(104,306,roll),-Mathf.Lerp(651,601,roll));ball.localRotation=Quaternion.Euler(0,0,-roll*175);SetVerticesDirty();
  }
  Vector2 Map(Vector2 p)=>rectTransform.rect.center+new Vector2(p.x-300,500-p.y)*scale;
  void Poly(VertexHelper v,Color c,params Vector2[] points){int first=v.currentVertCount;Vector2 center=Vector2.zero;foreach(var p in points)center+=p;center/=points.Length;v.AddVert(Map(center),c,Vector2.zero);for(int i=0;i<points.Length;i++)v.AddVert(Map(points[i]),c,Vector2.zero);for(int i=0;i<points.Length;i++)v.AddTriangle(first,first+1+i,first+1+(i+1)%points.Length);}
  void Oval(VertexHelper v,float x,float y,float rx,float ry,Color c){const int steps=48;int first=v.currentVertCount;var clear=c;clear.a=0;v.AddVert(Map(new Vector2(x,y)),c,Vector2.zero);float fringe=1/Mathf.Max(.1f,scale);
   for(int i=0;i<=steps;i++){float a=i*Mathf.PI*2/steps;v.AddVert(Map(new Vector2(x+Mathf.Cos(a)*rx,y+Mathf.Sin(a)*ry)),c,Vector2.zero);v.AddVert(Map(new Vector2(x+Mathf.Cos(a)*(rx+fringe),y+Mathf.Sin(a)*(ry+fringe))),clear,Vector2.zero);if(i>0){int n=first+1+(i-1)*2;v.AddTriangle(first,n,n+2);v.AddTriangle(n,n+1,n+2);v.AddTriangle(n+1,n+3,n+2);}}}
  void Line(VertexHelper v,Vector2 a,Vector2 b,float width,Color c){var side=new Vector2(-(b-a).y,(b-a).x).normalized*width*.5f;Poly(v,c,a+side,b+side,b-side,a-side);}
  void Star(VertexHelper v,float x,float y,float size,Color c){var p=new Vector2[10];for(int i=0;i<10;i++){float a=(i*36-90)*Mathf.Deg2Rad,r=i%2==0?size:size*.44f;p[i]=new Vector2(x+Mathf.Cos(a)*r,y+Mathf.Sin(a)*r);}Poly(v,c,p);}
  protected override void OnPopulateMesh(VertexHelper v){v.Clear();var r=rectTransform.rect;v.AddVert(new Vector2(r.xMin,r.yMin),Art.Hex("102A43"),Vector2.zero);v.AddVert(new Vector2(r.xMin,r.yMax),Art.Hex("244F72"),Vector2.zero);v.AddVert(new Vector2(r.xMax,r.yMax),Art.Hex("244F72"),Vector2.zero);v.AddVert(new Vector2(r.xMax,r.yMin),Art.Hex("102A43"),Vector2.zero);v.AddTriangle(0,1,2);v.AddTriangle(0,2,3);
   Oval(v,304,559,250,222,new Color(.10f,.82f,.82f,.12f));Oval(v,548,145,142,142,new Color(.65f,.40f,.96f,.10f));Oval(v,28,868,158,158,new Color(1,.34f,.56f,.08f));
   Vector2 previous=new Vector2(-20,84);for(int i=0;i<=12;i++){float x=i*50,y=82+28*Mathf.Sin(i/12f*Mathf.PI);var point=new Vector2(x,y);Line(v,previous,point,2,Art.Hex("8FA9BD"));previous=point;if(i>0&&i<12)Poly(v,i%3==0?Pink:i%3==1?Teal:Gold,new Vector2(x-15,y+3),new Vector2(x+15,y+3),new Vector2(x,y+34));}
   Oval(v,300,697,239,27,new Color(0,0,0,.17f));Poly(v,Art.Hex("9E693D"),new Vector2(90,538),new Vector2(510,538),new Vector2(553,682),new Vector2(47,682));Poly(v,Art.Hex("F5D18A"),new Vector2(90,526),new Vector2(510,526),new Vector2(553,669),new Vector2(47,669));
   for(int i=0;i<9;i++){float x=98+i*50;Line(v,new Vector2(x,530),new Vector2(x+(x-300)*.18f,662),1,new Color(.55f,.35f,.12f,.17f));}
   Line(v,new Vector2(91,531),new Vector2(52,664),4,Art.Hex("FFF0C2"));Line(v,new Vector2(509,531),new Vector2(548,664),4,Art.Hex("E6AE58"));
   foreach(var p in rests)Oval(v,p.x,p.y+2,39,10,new Color(.20f,.25f,.24f,.13f));
   float t=reduced?.35f:Progress,roll=Mathf.SmoothStep(0,1,Mathf.Clamp01(t/.53f)),hit=Mathf.Clamp01((t-.48f)/.32f);Oval(v,Mathf.Lerp(104,306,roll)+3,Mathf.Lerp(651,601,roll)+38,47,12,new Color(.14f,.20f,.23f,.18f));
   Star(v,112,443,12,Gold);Star(v,491,431,9,Teal);
   if(hit>0&&!reduced)for(int i=0;i<16;i++){float a=i*Mathf.PI*2/16;float distance=24+hit*140;var c=i%3==0?Pink:i%3==1?Gold:Teal;c.a=1-hit*.65f;Star(v,326+Mathf.Cos(a)*distance,520+Mathf.Sin(a)*distance*.65f,5+3*(i%2),c);}
   for(int i=0;i<3;i++){float pulse=reduced?1:1+.18f*Mathf.Sin(t*9-i);Oval(v,272+i*28,741,6*pulse,6*pulse,i==0?Pink:i==1?Teal:Purple);}
  }
 }
}
