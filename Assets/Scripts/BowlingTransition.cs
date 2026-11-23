using UnityEngine;
using UnityEngine.UI;

namespace PinPop
{
    // Native UI artwork: no browser, texture readback or additional asset imports.
    [RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public sealed class BowlingTransition : MaskableGraphic
    {
        protected override void OnEnable()
        {
            // Explicitly repair pre-existing editor objects before Graphic registers for raycasts.
            if (GetComponent<CanvasRenderer>() == null) gameObject.AddComponent<CanvasRenderer>();
            base.OnEnable();
        }
        public const float Duration = 1.4f;
        Font labelFont;
        CanvasGroup fade;
        Text levelLabel;
        float started;
        bool reduced;
        public float Progress => Mathf.Clamp01((Time.unscaledTime - started) / Duration);
        public void Build(Font font)
        {
            labelFont = font;fade=gameObject.AddComponent<CanvasGroup>();
            raycastTarget = true;
            Label("PINPOP", 212, 22, "547487");
            Label("SIRADAKİ ATIŞ", 260, 11, "7B8D91");
            levelLabel = Label("BÖLÜM 1", 307, 43, "294B61");
            Label("Renkler hazır. Sıra sende.", 370, 14, "718581", false);
            Label("AYNI RENK. TAM İSABET.", 800, 14, "3D6475");
            Label("Topu geriye çek, yönünü seç, yuvarla.", 833, 12, "7A877C", false);
            Label("B O W L I N G   F E S T İ V A L İ", 924, 9, "99A59B", false);
        }
        Text Label(string value, float y, int size, string color, bool bold = true)
        {
            var o = new GameObject(value, typeof(RectTransform), typeof(Text));o.layer = 5;
            var r = o.GetComponent<RectTransform>();r.SetParent(transform, false);
            r.anchorMin = new Vector2(.04f, 1 - (y + 50) / 1000);r.anchorMax = new Vector2(.96f, 1 - y / 1000);r.offsetMin = r.offsetMax = Vector2.zero;
            var t = o.GetComponent<Text>();t.text=value;t.font=labelFont;t.fontStyle=bold?FontStyle.Bold:FontStyle.Normal;t.fontSize=Mathf.RoundToInt(size * rectTransform.rect.width / 600);t.alignment=TextAnchor.MiddleCenter;t.color=Art.Hex(color);t.raycastTarget=false;
            t.resizeTextForBestFit=true;t.resizeTextMinSize=7;t.resizeTextMaxSize=t.fontSize;
            return t;
        }
        public void Show(int level, bool reduceMotion, float shownAt)
        {
            started=shownAt;reduced=reduceMotion;levelLabel.text="BÖLÜM " + (level+1);
            gameObject.SetActive(true);transform.SetAsLastSibling();ApplyVisualState();
        }
        void Update(){ApplyVisualState();}
        void ApplyVisualState(){float t=Progress;fade.alpha=reduced?1:Mathf.SmoothStep(0,1,t/.1f)*(1-Mathf.SmoothStep(0,1,(t-.87f)/.13f));SetVerticesDirty();}
        Vector2 Map(Vector2 p){var r=rectTransform.rect;return new Vector2(r.xMin+p.x*r.width/600,r.yMax-p.y*r.height/1000);}
        void Poly(VertexHelper vh, Color c, params Vector2[] points)
        {
            int n=vh.currentVertCount;foreach(var p in points)vh.AddVert(Map(p),c,Vector2.zero);
            for(int i=1;i<points.Length-1;i++)vh.AddTriangle(n,n+i,n+i+1);
        }
        void Box(VertexHelper vh,float x,float y,float w,float h,string c){Poly(vh,Art.Hex(c),new Vector2(x,y),new Vector2(x+w,y),new Vector2(x+w,y+h),new Vector2(x,y+h));}
        void Oval(VertexHelper vh,float x,float y,float rx,float ry,Color c)
        {
            int n=vh.currentVertCount;vh.AddVert(Map(new Vector2(x,y)),c,Vector2.zero);
            for(int i=0;i<=48;i++){float a=i*Mathf.PI*2/48;vh.AddVert(Map(new Vector2(x+Mathf.Cos(a)*rx,y+Mathf.Sin(a)*ry)),c,Vector2.zero);if(i>0)vh.AddTriangle(n,n+i,n+i+1);}
        }
        void Line(VertexHelper vh, Vector2 a, Vector2 b,float width,Color c){var d=(b-a).normalized;var side=new Vector2(-d.y,d.x)*width*.5f;Poly(vh,c,a-side,a+side,b+side,b-side);}
        void Star(VertexHelper vh,float x,float y,float size,Color c){Poly(vh,c,new Vector2(x,y-size),new Vector2(x+size*.25f,y-size*.25f),new Vector2(x+size,y),new Vector2(x+size*.25f,y+size*.25f),new Vector2(x,y+size),new Vector2(x-size*.25f,y+size*.25f),new Vector2(x-size,y),new Vector2(x-size*.25f,y-size*.25f));}
        Vector2 PinPoint(float x,float y,float px,float py,float angle){float a=angle*Mathf.Deg2Rad;return new Vector2(x+px*Mathf.Cos(a)-py*Mathf.Sin(a),y+px*Mathf.Sin(a)+py*Mathf.Cos(a));}
        void Pin(VertexHelper vh,float x,float y,string hex,float angle=0,float alpha=1)
        {
            Color body=Art.Hex(hex);body.a=alpha;Color white=Art.Hex("FFFDF4");white.a=alpha;
            float[] heights={0,7,22,42,62,76,90,100};float[] widths={13,22,23,19,11,8,8,10};
            for(int i=0;i<heights.Length-1;i++){Color color=heights[i]<62?body:white;
                Poly(vh,color,PinPoint(x,y,-widths[i],-heights[i],angle),PinPoint(x,y,widths[i],-heights[i],angle),PinPoint(x,y,widths[i+1],-heights[i+1],angle),PinPoint(x,y,-widths[i+1],-heights[i+1],angle));}
            var head=PinPoint(x,y,0,-111,angle);Oval(vh,head.x,head.y,14,16,white);
            Line(vh,PinPoint(x,y,-8,-83,angle),PinPoint(x,y,8,-83,angle),4,body);
            Line(vh,PinPoint(x,y,-8,-90,angle),PinPoint(x,y,8,-90,angle),2,body);
            Color shine=new Color(1,1,1,.28f*alpha);Line(vh,PinPoint(x,y,-11,-22,angle),PinPoint(x,y,-8,-47,angle),5,shine);
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();Box(vh,0,0,600,1000,"F4F0E4");Oval(vh,300,529,246,214,Art.Hex("E4EBE1"));Oval(vh,525,110,145,145,Art.Hex("E8ECDD"));Oval(vh,25,900,180,150,Art.Hex("E9E5D5"));
            // Festival bunting frames the scene instead of an empty loading screen.
            Vector2 prev=new Vector2(-20,63);for(int i=0;i<=12;i++){float x=i*50,y=60+44*Mathf.Sin(x/600*Mathf.PI);var p=new Vector2(x,y);Line(vh,prev,p,2,Art.Hex("B9BCA4"));prev=p;if(i>0&&i<12){string c=i%3==0?"DCA49D":i%3==1?"B6C9BA":"C7BDDA";Poly(vh,Art.Hex(c),new Vector2(x-17,y+3),new Vector2(x+17,y+3),new Vector2(x,y+36));}}
            Oval(vh,300,669,218,25,new Color(.26f,.36f,.30f,.09f));
            Poly(vh,Art.Hex("667E77"),new Vector2(107,523),new Vector2(493,523),new Vector2(533,662),new Vector2(67,662));
            Poly(vh,Art.Hex("D8C79B"),new Vector2(107,510),new Vector2(493,510),new Vector2(533,648),new Vector2(67,648));
            for(int i=0;i<8;i++){float x=114+i*52;Line(vh,new Vector2(x,516),new Vector2(x+(x-300)*.15f,641),1,new Color(.54f,.43f,.23f,.15f));}
            Line(vh,new Vector2(108,516),new Vector2(73,643),4,Art.Hex("B4A477"));Line(vh,new Vector2(492,516),new Vector2(527,643),4,Art.Hex("B4A477"));
            float t=reduced?.28f:Progress;float hit=Mathf.SmoothStep(0,1,Mathf.Clamp01((t-.49f)/.35f));
            Oval(vh,180,554,26,8,new Color(0,0,0,.08f));Pin(vh,180,550,"9680CB");
            Oval(vh,420,555,26,8,new Color(0,0,0,.08f));Pin(vh,420,552,"4DB3C2");
            Pin(vh,300-hit*22,555-hit*30,"DF7891",-hit*55,1-hit*.5f);Pin(vh,352+hit*28,565-hit*18,"DF7891",hit*62,1-hit*.5f);
            float bx=Mathf.Lerp(115,294,Mathf.SmoothStep(0,1,Mathf.Clamp01(t/.53f))),by=Mathf.Lerp(611,577,Mathf.Clamp01(t/.53f));
            Oval(vh,bx+2,by+30,34,9,new Color(.22f,.31f,.27f,.13f));Oval(vh,bx,by,33,33,Art.Hex("C35F7E"));Oval(vh,bx-3,by-4,29,28,Art.Hex("E6819D"));Oval(vh,bx-12,by-14,9,6,new Color(1,1,1,.24f));
            float roll=t*3;for(int i=0;i<3;i++){float a=roll+i*2.094f;Oval(vh,bx+Mathf.Cos(a)*10,by+Mathf.Sin(a)*10,3.1f,3.8f,Art.Hex("3E4B62"));}
            Star(vh,124,433,12,Art.Hex("C8B580"));Star(vh,472,423,8,Art.Hex("AFBFAE"));if(hit>0){for(int i=0;i<6;i++){float a=i*Mathf.PI/3;Star(vh,325+Mathf.Cos(a)*(30+hit*47),494+Mathf.Sin(a)*(24+hit*27),6*(1-hit*.45f),Art.Hex(i%2==0?"D3B567":"DF7891"));}}
            // Three color beats communicate that the next board is being prepared.
            for(int i=0;i<3;i++){bool ready=reduced||t>(i+1)*.19f;Oval(vh,276+i*24,742,ready?5:3,ready?5:3,Art.Hex(ready?new[]{"DF7891","4DB3C2","9680CB"}[i]:"D8DED1"));}
        }
    }
}
