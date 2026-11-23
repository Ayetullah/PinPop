using UnityEngine;
using UnityEngine.UI;
namespace PinPop {
 // Small code-native decorative layer. The interactive node hit areas never move.
 public sealed class CarnivalMapGraphic:MaskableGraphic {
  public int Region,Page,Highest,FirstLevel,LevelCount=20;public bool Effects,Reduced;float clock;
  protected override void OnPopulateMesh(VertexHelper v){v.Clear();if(!Effects){for(int i=0;i<LevelCount-1;i++){var a=CarnivalMapCatalog.ScrollStop(i);var b=CarnivalMapCatalog.ScrollStop(i+1);var prev=a;for(int j=1;j<=32;j++){float t=j/32f;var p=Curve(a,b,t);Line(v,prev+Vector2.down*3,p+Vector2.down*3,16,new Color(.19f,.28f,.27f,.23f));Line(v,prev,p,12,Art.Hex("A57E4E"));Line(v,prev,p,7,Art.Hex(Highest>=FirstLevel+i+1?"F4D591":"E4D8AF"));prev=p;}}return;}
   float time=Reduced?0:clock;var cream=Art.Hex("FFF0B6");var dark=Art.Hex("3D586C");
   // All regions have gently twinkling fairground bulbs along a string.
   for(int i=0;i<15;i++){float x=83+i*31,y=255+Mathf.Sin(i*.22f)*14;Disc(v,new Vector2(x,y),2.6f,new Color(1,.88f,.57f,Reduced?.7f:.55f+.35f*Mathf.Sin(time*1.7f+i)));}
   if(Region==0){for(int i=0;i<3;i++){float x=86+i*17,y=303-i*8+Mathf.Sin(time*.8f+i)*4;Line(v,new Vector2(x,y+15),new Vector2(108,326),1.7f,cream);Ellipse(v,new Vector2(x,y),new Vector2(12,16),Art.Colors[i]);Ellipse(v,new Vector2(x-3,y-5),new Vector2(2,4),new Color(1,1,1,.6f));}}
   if(Region==1){var center=new Vector2(440,297);Line(v,new Vector2(416,337),center,4,dark);Line(v,new Vector2(464,337),center,4,dark);Ring(v,center,31,3,cream);Ring(v,center,27,2,Art.Hex("67C0CB"));for(int i=0;i<8;i++){float a=i*Mathf.PI/4+time*.18f;var p=center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*31;Line(v,center,p,1.8f,cream);Quad(v,p+new Vector2(-5,0),new Vector2(10,9),Art.Colors[i]);Line(v,p+new Vector2(-5,2),p+new Vector2(5,2),2,cream);}Disc(v,center,5,Art.Hex("EEC978"));}
   if(Region==2){for(int i=0;i<9;i++){float x=85+i*48,y=282+Mathf.Sin(i*.3f)*9;Line(v,new Vector2(x,y),new Vector2(x+48,282+Mathf.Sin((i+1)*.3f)*9),1.7f,cream);Triangle(v,new Vector2(x+4,y+2),new Vector2(x+28,y+2),new Vector2(x+16+Mathf.Sin(time*1.3f+i)*3,y+18),Art.Colors[i%8]);}}
   if(Region==3){for(int k=0;k<2;k++){var center=new Vector2(k==0?104:484,k==0?280:303);float phase=Mathf.Repeat(time*.24f+k*.5f,1);for(int j=0;j<12;j++){float a=j*Mathf.PI/6;var p=new Vector2(Mathf.Cos(a),Mathf.Sin(a));Line(v,center+p*(8+phase*18),center+p*(14+phase*25),2,new Color(1,.82f,.51f,1-phase*.75f));}}for(int i=0;i<14;i++){float x=70+(i*67)%465,y=230+(i*31)%105;Disc(v,new Vector2(x,y),1.5f,new Color(1,.92f,.7f,.5f+.35f*Mathf.Sin(time+i)));}}
  }
  void Update(){if(!Effects||Reduced)return;clock=Time.unscaledTime;SetVerticesDirty();}
  public static Vector2 Curve(Vector2 a,Vector2 b,float t){float s=1-t;var c=new Vector2(a.x,(a.y+b.y)/2);var d=new Vector2(b.x,(a.y+b.y)/2);return s*s*s*a+3*s*s*t*c+3*s*t*t*d+t*t*t*b;}
  static void Triangle(VertexHelper v,Vector2 a,Vector2 b,Vector2 c,Color color){int n=v.currentVertCount;v.AddVert(new Vector3(a.x,-a.y,0),color,Vector2.zero);v.AddVert(new Vector3(b.x,-b.y,0),color,Vector2.zero);v.AddVert(new Vector3(c.x,-c.y,0),color,Vector2.zero);v.AddTriangle(n,n+1,n+2);}
  static void Quad(VertexHelper v,Vector2 a,Vector2 size,Color color){Triangle(v,a,a+Vector2.right*size.x,a+size,color);Triangle(v,a,a+size,a+Vector2.up*size.y,color);}
  static void Line(VertexHelper v,Vector2 a,Vector2 b,float width,Color color){var delta=b-a;if(delta.sqrMagnitude<.001f)return;var n=new Vector2(-delta.y,delta.x).normalized*width/2;Triangle(v,a-n,a+n,b+n,color);Triangle(v,a-n,b+n,b-n,color);}
  static void Disc(VertexHelper v,Vector2 center,float radius,Color color){Ellipse(v,center,Vector2.one*radius,color);}
  static void Ellipse(VertexHelper v,Vector2 center,Vector2 radius,Color color){for(int i=0;i<24;i++){float a=i*Mathf.PI/12,b=(i+1)*Mathf.PI/12;Triangle(v,center,center+new Vector2(Mathf.Cos(a)*radius.x,Mathf.Sin(a)*radius.y),center+new Vector2(Mathf.Cos(b)*radius.x,Mathf.Sin(b)*radius.y),color);}}
  static void Ring(VertexHelper v,Vector2 center,float radius,float width,Color color){for(int i=0;i<48;i++){float a=i*Mathf.PI/24,b=(i+1)*Mathf.PI/24;Line(v,center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius,center+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*radius,width,color);}}
 }
}
