using UnityEngine;
using UnityEngine.UI;
namespace PinPop {
 [RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public sealed class HtmlResultFX:MaskableGraphic {
        protected override void OnEnable()
        {
            // Explicitly repair pre-existing editor objects before Graphic registers for raycasts.
            if (GetComponent<CanvasRenderer>() == null) gameObject.AddComponent<CanvasRenderer>();
            base.OnEnable();
        }
  PinPopGame game;bool coins;float started;Vector2 target;Rect card;
  public void Build(PinPopGame owner,bool flyingCoins,Vector2 wallet,Rect protectedCard){game=owner;coins=flyingCoins;target=wallet;card=protectedCard;raycastTarget=false;Restart();}
  public void Restart(){started=Time.unscaledTime;SetVerticesDirty();}
  public void SetProtectedCard(Rect area){card=area;SetVerticesDirty();}
  void Update(){SetVerticesDirty();}
  Vector2 Map(Vector2 p){var r=rectTransform.rect;return new Vector2(r.xMin+p.x*r.width/600,r.yMax-p.y*r.height/1000);}
  void Poly(VertexHelper vh,Color c,params Vector2[] p){int n=vh.currentVertCount;foreach(var v in p)vh.AddVert(Map(v),c,Vector2.zero);for(int i=1;i<p.Length-1;i++)vh.AddTriangle(n,n+i,n+i+1);}
  void Circle(VertexHelper vh,Vector2 c,float r,Color color){int n=vh.currentVertCount;vh.AddVert(Map(c),color,Vector2.zero);for(int i=0;i<=24;i++){float a=i*Mathf.PI/12;vh.AddVert(Map(c+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r),color,Vector2.zero);if(i>0)vh.AddTriangle(n,n+i,n+i+1);}}
  void Stroke(VertexHelper vh,Vector2 a,Vector2 b,Color color){if(card.Contains((a+b)*.5f))return;var v=(b-a).normalized;var p=new Vector2(-v.y,v.x);Poly(vh,color,a-p,a+p,b+p,b-p);}
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();if(game==null||game.ReducedMotion)return;float age=Time.unscaledTime-started;
   if(!coins){if(age>3)return;Vector2[] centers={new Vector2(90,245),new Vector2(490,225),new Vector2(100,520),new Vector2(510,510)};string[] colors={"FFF6A7","FFB3E9","A5EEFF","BAFFC2"};for(int k=0;k<4;k++){float p=(age+k*.3f)%1.25f,r=p*115;var color=Art.Hex(colors[k]);color.a=1-p/1.25f;for(int i=0;i<28;i++){float a=i*Mathf.PI*2/28;var dir=new Vector2(Mathf.Cos(a),Mathf.Sin(a));var center=centers[k]+Vector2.up*p*p*12;Stroke(vh,center+dir*r*.72f,center+dir*r,color);}}return;}
   for(int i=0;i<12;i++){float t=(age-.4f-i*.05f)/.95f;if(t<0||t>=1)continue;var origin=new Vector2(300,600);var scatter=origin+new Vector2((i-5)*10,-30);Vector2 p;float scale,alpha;if(t<.22f){float u=t/.22f;p=Vector2.Lerp(origin,scatter,u);scale=Mathf.Lerp(.4f,1.1f,u);alpha=u;}else{float u=Mathf.SmoothStep(0,1,(t-.22f)/.78f);p=Vector2.Lerp(scatter,target,u);scale=Mathf.Lerp(1.1f,.6f,u);alpha=Mathf.Lerp(1,.3f,u);}if(card.Contains(p))continue;var gold=Art.Hex("D6B45F");gold.a=alpha;Circle(vh,p,12*scale,gold);var light=Art.Hex("F1DA9B");light.a=alpha;Circle(vh,p,9*scale,light);var star=new Vector2[10];for(int n=0;n<10;n++){float a=-Mathf.PI/2+n*Mathf.PI/5;star[n]=p+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*(n%2==0?6:2.8f)*scale;}var ink=Art.Hex("96713D");ink.a=alpha;Poly(vh,ink,star);}
  }
 }
}
