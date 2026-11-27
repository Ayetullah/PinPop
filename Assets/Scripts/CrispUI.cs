using UnityEngine;
using UnityEngine.UI;

namespace PinPop {
 // Pixel-space UI: fonts are rasterized at their displayed size, never enlarged screenshots.
 public static class CrispUI {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
  public static bool? ReviewReducedMotion;
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void ResetReviewMotion(){ReviewReducedMotion=null;}
  public static Rect? ReviewSafeArea;
  public static Vector2Int? ReviewSize;
#endif
  public static int Width {get {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(ReviewSize.HasValue)return ReviewSize.Value.x;
#endif
   return Screen.width;
  }}
  public static int Height {get {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(ReviewSize.HasValue)return ReviewSize.Value.y;
#endif
   return Screen.height;
  }}
  public static Rect SafeArea {get {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(ReviewSafeArea.HasValue)return ReviewSafeArea.Value;
#endif
   return Screen.safeArea;
  }}
  public static Color InkOn(Color background){var light=Art.Hex("FFF4DB");var dark=Art.Hex("294B61");float bg=Luminance(background),a=(Luminance(light)+.05f)/(bg+.05f),b=(bg+.05f)/(Luminance(dark)+.05f);return a>=b?light:dark;}
  static float Luminance(Color color){var c=color.linear;return c.r*.2126f+c.g*.7152f+c.b*.0722f;}
  static Font regular,bold;
  public static Font Regular=>regular!=null?regular:(regular=Resources.Load<Font>("PinPop-Regular"));
  public static Font Bold=>bold!=null?bold:(bold=Resources.Load<Font>("PinPop-Bold"));
  public static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h){var o=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer));o.layer=5;var r=o.GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
  public static CrispShape Panel(string name,Transform parent,float x,float y,float w,float h,Color color,float radius=12){var p=Rect(name,parent,x,y,w,h).gameObject.AddComponent<CrispShape>();p.color=color;p.Radius=radius;p.raycastTarget=false;return p;}
  public static Text Text(string name,Transform parent,float x,float y,float w,float h,string value,float size,Color color,bool bold=true){var t=Rect(name,parent,x,y,w,h).gameObject.AddComponent<CrispText>();t.font=bold?Bold:Regular;t.fontSize=Mathf.RoundToInt(size);t.text=value;t.color=color;t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;return t;}
  public static CrispIcon Icon(string name,Transform parent,float x,float y,float w,float h,CrispIcon.Kind kind,Color color){var i=Rect(name,parent,x,y,w,h).gameObject.AddComponent<CrispIcon>();i.Symbol=kind;i.color=color;i.raycastTarget=false;return i;}
 }
 public sealed class CrispShape:Image {
  public float Radius=12;
  protected override void OnPopulateMesh(VertexHelper vh){if(sprite!=null){base.OnPopulateMesh(vh);return;}vh.Clear();var r=rectTransform.rect;float radius=Mathf.Min(Radius,Mathf.Min(r.width,r.height)*.5f);const int steps=12;int n=4*(steps+1);var points=new Vector2[n];int j=0;for(int c=0;c<4;c++){var center=new Vector2(c==0||c==3?r.xMax-radius:r.xMin+radius,c<2?r.yMax-radius:r.yMin+radius);for(int k=0;k<=steps;k++){float a=(c*90+k*90f/steps)*Mathf.Deg2Rad;points[j++]=center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius;}}
   var tint=color;vh.AddVert(r.center,tint,Vector2.zero);for(int i=0;i<n;i++)vh.AddVert(points[i],tint,Vector2.zero);for(int i=0;i<n;i++)vh.AddTriangle(0,1+i,1+(i+1)%n);
   float feather=1;var clear=tint;clear.a=0;for(int i=0;i<n;i++){var prev=points[(i+n-1)%n];var next=points[(i+1)%n];var tangent=(next-prev).normalized;var normal=new Vector2(tangent.y,-tangent.x); // perimeter is counterclockwise
    vh.AddVert(points[i]+normal*feather,clear,Vector2.zero);}
   for(int i=0;i<n;i++){int next=(i+1)%n;vh.AddTriangle(1+i,n+1+i,n+1+next);vh.AddTriangle(1+i,n+1+next,1+next);}
  }
 }
 public sealed class CrispIcon:MaskableGraphic {
  public enum Kind {Palette,Rainbow,Pocket,Moves,Star,Swap,Gear,Shop,Home,Coin,Close,Map,ChevronDown,Check,Globe}
  public Kind Symbol;
  Vector2 Map(Vector2 p){var r=rectTransform.rect;float s=Mathf.Min(r.width,r.height)/100;return r.center+(p-Vector2.one*50)*s;}
  void Poly(VertexHelper v,Color c,params Vector2[] p){
   int n=v.currentVertCount;Vector2 center=Vector2.zero;float area=0;foreach(var x in p)center+=x;center/=p.Length;v.AddVert(Map(center),c,Vector2.zero);
   for(int i=0;i<p.Length;i++){v.AddVert(Map(p[i]),c,Vector2.zero);var next=p[(i+1)%p.Length];area+=p[i].x*next.y-next.x*p[i].y;}
   for(int i=0;i<p.Length;i++)v.AddTriangle(n,n+1+i,n+1+(i+1)%p.Length);
   var clear=c;clear.a=0;float density=canvas==null?1:Mathf.Max(.1f,Mathf.Abs(rectTransform.lossyScale.x/canvas.transform.lossyScale.x));float feather=1/density;
   for(int i=0;i<p.Length;i++){var tangent=(Map(p[(i+1)%p.Length])-Map(p[(i+p.Length-1)%p.Length])).normalized;var normal=new Vector2(tangent.y,-tangent.x)*(area>=0?1:-1);v.AddVert(Map(p[i])+normal*feather,clear,Vector2.zero);}
   for(int i=0;i<p.Length;i++){int next=(i+1)%p.Length;v.AddTriangle(n+1+i,n+1+p.Length+i,n+1+p.Length+next);v.AddTriangle(n+1+i,n+1+p.Length+next,n+1+next);}
  }
  void Disc(VertexHelper v,float x,float y,float radius,Color c){var points=new Vector2[64];for(int i=0;i<points.Length;i++){float a=i*Mathf.PI*2/points.Length;points[i]=new Vector2(x+Mathf.Cos(a)*radius,y+Mathf.Sin(a)*radius);}Poly(v,c,points);}
  void Line(VertexHelper v,Vector2 a,Vector2 b,float width,Color c){var side=new Vector2(-(b-a).y,(b-a).x).normalized*width*.5f;Poly(v,c,a+side,b+side,b-side,a-side);Disc(v,a.x,a.y,width*.5f,c);Disc(v,b.x,b.y,width*.5f,c);}
  void Arc(VertexHelper v,float radius,float start,float end,float thickness,Color c){
   const int steps=36;int first=v.currentVertCount;var clear=c;clear.a=0;float pixel=100/Mathf.Max(1,Mathf.Min(rectTransform.rect.width,rectTransform.rect.height));
   for(int i=0;i<=steps;i++){float angle=Mathf.Lerp(start,end,i/(float)steps)*Mathf.Deg2Rad;var d=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));var center=new Vector2(50,45);v.AddVert(Map(center+d*(radius-thickness*.5f-pixel)),clear,Vector2.zero);v.AddVert(Map(center+d*(radius-thickness*.5f)),c,Vector2.zero);v.AddVert(Map(center+d*(radius+thickness*.5f)),c,Vector2.zero);v.AddVert(Map(center+d*(radius+thickness*.5f+pixel)),clear,Vector2.zero);if(i>0){int n=first+(i-1)*4;for(int strip=0;strip<3;strip++){v.AddTriangle(n+strip,n+strip+4,n+strip+1);v.AddTriangle(n+strip+1,n+strip+4,n+strip+5);}}}
   float a=start*Mathf.Deg2Rad,b=end*Mathf.Deg2Rad;Disc(v,50+Mathf.Cos(a)*radius,45+Mathf.Sin(a)*radius,thickness*.5f,c);Disc(v,50+Mathf.Cos(b)*radius,45+Mathf.Sin(b)*radius,thickness*.5f,c);
  }
  protected override void OnPopulateMesh(VertexHelper v){v.Clear();var c=color;var dark=Art.Hex("294B61");
   switch(Symbol){
    case Kind.Palette:Disc(v,50,50,39,c);Disc(v,32,61,9,Art.Colors[0]);Disc(v,57,72,9,Art.Colors[1]);Disc(v,75,48,9,Art.Colors[2]);Disc(v,49,31,9,Art.Hex("F4BD49"));Disc(v,25,35,8,dark);break;
    case Kind.Rainbow:Arc(v,37,0,180,12,Art.Hex("EF8B9C"));Arc(v,24,0,180,11,Art.Hex("F4BD49"));Arc(v,12,0,180,10,Art.Hex("66D3CE"));Disc(v,13,39,13,c);Disc(v,87,39,13,c);break;
    case Kind.Pocket:Poly(v,c,new Vector2(15,62),new Vector2(85,62),new Vector2(80,23),new Vector2(50,10),new Vector2(20,23));Line(v,new Vector2(20,63),new Vector2(80,63),7,Art.Hex("F4BD49"));Disc(v,50,82,9,c);Line(v,new Vector2(50,74),new Vector2(50,51),10,c);break;
    case Kind.Moves:Arc(v,35,35,320,9,c);Poly(v,c,new Vector2(84,66),new Vector2(87,89),new Vector2(63,80));Line(v,new Vector2(37,46),new Vector2(63,46),7,c);Line(v,new Vector2(50,33),new Vector2(50,59),7,c);break;
    case Kind.Star:case Kind.Coin:if(Symbol==Kind.Coin){Disc(v,50,50,44,Art.Hex("F4BD49"));c=Art.Hex("946627");}var pts=new Vector2[10];for(int i=0;i<10;i++){float a=(90+i*36)*Mathf.Deg2Rad,r=i%2==0?40:19;pts[i]=new Vector2(50+Mathf.Cos(a)*r,50+Mathf.Sin(a)*r);}Poly(v,c,pts);break;
    case Kind.Swap:Line(v,new Vector2(18,67),new Vector2(79,67),9,c);Poly(v,c,new Vector2(69,83),new Vector2(91,67),new Vector2(69,51));Line(v,new Vector2(82,32),new Vector2(21,32),9,c);Poly(v,c,new Vector2(31,48),new Vector2(9,32),new Vector2(31,16));break;
    case Kind.Gear:for(int i=0;i<8;i++){float a=i*Mathf.PI/4;Line(v,new Vector2(50+Mathf.Cos(a)*24,50+Mathf.Sin(a)*24),new Vector2(50+Mathf.Cos(a)*38,50+Mathf.Sin(a)*38),16,c);}Disc(v,50,50,31,c);Disc(v,50,50,16,dark);Disc(v,50,50,8,c);break;
    case Kind.Shop:Poly(v,c,new Vector2(18,59),new Vector2(82,59),new Vector2(72,82),new Vector2(28,82));Line(v,new Vector2(24,55),new Vector2(24,19),8,c);Line(v,new Vector2(76,55),new Vector2(76,19),8,c);Line(v,new Vector2(24,19),new Vector2(76,19),8,c);Line(v,new Vector2(45,20),new Vector2(45,43),8,c);break;
    case Kind.Home:Line(v,new Vector2(12,52),new Vector2(50,85),9,c);Line(v,new Vector2(50,85),new Vector2(88,52),9,c);Line(v,new Vector2(23,56),new Vector2(23,17),8,c);Line(v,new Vector2(77,56),new Vector2(77,17),8,c);Line(v,new Vector2(23,17),new Vector2(77,17),8,c);Line(v,new Vector2(50,17),new Vector2(50,42),10,c);break;
    case Kind.Map:Poly(v,c,new Vector2(12,20),new Vector2(35,30),new Vector2(65,18),new Vector2(88,29),new Vector2(88,82),new Vector2(65,71),new Vector2(35,83),new Vector2(12,73));Line(v,new Vector2(35,36),new Vector2(35,74),3,dark);Line(v,new Vector2(65,27),new Vector2(65,65),3,dark);Disc(v,51,54,7,dark);break;
    case Kind.Close:Line(v,new Vector2(27,27),new Vector2(73,73),10,c);Line(v,new Vector2(27,73),new Vector2(73,27),10,c);break;
    case Kind.ChevronDown:Line(v,new Vector2(25,62),new Vector2(50,37),9,c);Line(v,new Vector2(50,37),new Vector2(75,62),9,c);break;
    case Kind.Globe:
     foreach(float radius in new[]{38f,18f})for(int i=0;i<48;i++){float a=i*Mathf.PI*2/48,b=(i+1)*Mathf.PI*2/48;Poly(v,c,new Vector2(50+(radius+2)*Mathf.Cos(a),50+40*Mathf.Sin(a)),new Vector2(50+(radius+2)*Mathf.Cos(b),50+40*Mathf.Sin(b)),new Vector2(50+(radius-2)*Mathf.Cos(b),50+36*Mathf.Sin(b)),new Vector2(50+(radius-2)*Mathf.Cos(a),50+36*Mathf.Sin(a)));}
     Line(v,new Vector2(14,50),new Vector2(86,50),4,c);break;
    case Kind.Check:Line(v,new Vector2(23,49),new Vector2(43,29),10,c);Line(v,new Vector2(43,29),new Vector2(80,72),10,c);break;
   }
  }
 }
}
