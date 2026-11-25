using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PinPop {
 public sealed class HtmlMatchTrayUI:MonoBehaviour {
  PinPopGame game;HtmlReferenceHUD hud;Font font;Sprite rounded,ring;Image panel,shadow;int version=-1;float visualClock,matchPulse,trayTop;float trayLeft=30,trayWidth=540,trayScale=1;
  readonly Sprite[] pinIcons=new Sprite[PinPopPalette.Count];
  readonly List<Sprite> surfaces=new List<Sprite>();readonly Image[] wells=new Image[10],dots=new Image[10];readonly float[] landPulse=new float[10];
  readonly Dictionary<int,RectTransform> tiles=new Dictionary<int,RectTransform>();
  Effect comboEffect;readonly List<Effect> effects=new List<Effect>();readonly int[] colorCounts=new int[PinPopPalette.Count];readonly HashSet<int> ids=new HashSet<int>();readonly List<int> remove=new List<int>();
  sealed class Effect {public RectTransform Rect;public Graphic Graphic;public float Age,Life;public Vector2 Origin,Velocity;public bool Ring;}
  public void Build(PinPopGame model,HtmlReferenceHUD view,Font typeface,Sprite shape){game=model;hud=view;font=typeface;rounded=shape;for(int color=0;color<PinPopPalette.Count;color++)pinIcons[color]=Resources.Load<Sprite>("PinIcons/pin-"+color);
   trayTop=hud.CollectionTop;var frame=Surface("557D8D","2B475A","AEC3BC",false);var well=Surface("172E40","243F50","527181",false);
   shadow=Image("Collection base shadow",30,trayTop+5,540,69,new Color(.08f,.18f,.23f,.35f));
   panel=Image("Collection tray",30,trayTop,540,69,Color.white);panel.sprite=frame;
   for(int i=0;i<10;i++){wells[i]=Image("Collection slot "+i,40+i*52.5f,trayTop+10,47.5f,47.5f,Color.white);wells[i].sprite=well;dots[i]=Image("Empty slot glint "+i,61.25f+i*52.5f,trayTop+32,5,3,Art.Hex("577583"));}
   ring=RingSprite();game.MatchTray.Matched+=OnMatched;game.MatchTray.Landed+=OnLanded;
  }

  float SlotX(float slot)=>trayLeft+(33.75f+slot*52.5f)*trayScale;
  float CenterY=>trayTop+33.75f*trayScale;
  void LayoutToTable(){
   trayTop=hud.CollectionTop;var a=hud.AppRect;var left=game.Camera.WorldToScreenPoint(new Vector3(-4.275f,0,4.8354167f));var right=game.Camera.WorldToScreenPoint(new Vector3(4.275f,0,4.8354167f));
   if(left.z<=0||right.z<=0)return;
   float min=Mathf.Clamp(left.x,a.x,a.xMax),max=Mathf.Clamp(right.x,a.x,a.xMax);trayLeft=(min-a.x)*600/a.width;trayWidth=Mathf.Max(1,(max-min)*600/a.width);trayScale=trayWidth/540;
   Place(panel.rectTransform,trayLeft,trayTop,trayWidth,69*trayScale);Place(shadow.rectTransform,trayLeft,trayTop+5*trayScale,trayWidth,69*trayScale);panel.pixelsPerUnitMultiplier=shadow.pixelsPerUnitMultiplier=600/(a.width*trayScale);
   for(int i=0;i<10;i++){Place(wells[i].rectTransform,trayLeft+(10+i*52.5f)*trayScale,trayTop+10*trayScale,47.5f*trayScale,47.5f*trayScale);wells[i].pixelsPerUnitMultiplier=600/(a.width*trayScale);Place(dots[i].rectTransform,trayLeft+(31.25f+i*52.5f)*trayScale,trayTop+32*trayScale,5*trayScale,3*trayScale);}
  }
  Vector2 Point(float x,float y){UnityEngine.Rect a=hud.AppRect;return new Vector2(a.x+x*a.width/600-CrispUI.Width/2f,a.yMax-y*a.height/1000-CrispUI.Height/2f);}
  void Place(RectTransform r,float x,float y,float w,float h){r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.sizeDelta=new Vector2(w*hud.AppRect.width/600,h*(Mathf.Approximately(w,h)?hud.AppRect.width/600:hud.AppRect.height/1000));r.anchoredPosition=Point(x+w/2,y+h/2);}
  RectTransform Rect(string name){var o=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer));o.layer=5;o.transform.SetParent(transform,false);return o.GetComponent<RectTransform>();}
  Image Image(string name,float x,float y,float w,float h,Color color){var rect=Rect(name);Place(rect,x,y,w,h);var image=rect.gameObject.AddComponent<Image>();image.sprite=rounded;image.type=UnityEngine.UI.Image.Type.Sliced;image.pixelsPerUnitMultiplier=600/hud.AppRect.width;image.color=color;image.raycastTarget=false;return image;}
  Text Label(string name,float x,float y,float w,float h,string value,int size,Color color){var rect=Rect(name);Place(rect,x,y,w,h);var text=rect.gameObject.AddComponent<CrispText>();text.font=font;text.fontSize=Mathf.Max(11,Mathf.RoundToInt(size*hud.AppRect.width/600));text.fontStyle=FontStyle.Bold;text.text=value;text.color=color;text.raycastTarget=false;text.alignment=TextAnchor.MiddleLeft;return text;}
  void LateUpdate(){if(game==null||game.MatchTray==null)return;var model=game.MatchTray;LayoutToTable();
   if(version!=model.Version){foreach(var t in tiles.Values)Destroy(t.gameObject);tiles.Clear();foreach(var e in effects)Destroy(e.Rect.gameObject);effects.Clear();comboEffect=null;version=model.Version;matchPulse=0;System.Array.Clear(landPulse,0,landPulse.Length);}
   float dt=game.Transitioning?0:game.TrayOverflow&&!game.Finished?Time.unscaledDeltaTime:game.Paused?0:Time.deltaTime;visualClock+=dt;matchPulse=Mathf.Max(0,matchPulse-dt*2.5f);
   bool danger=model.LiveCount>=8;panel.color=Color.Lerp(danger?Art.Hex("D78689"):Color.white,Art.Hex("FFF0B6"),matchPulse*.45f);
   System.Array.Clear(colorCounts,0,colorCounts.Length);foreach(var item in model.Items)if(!item.Matched)colorCounts[item.Color]++;
   for(int i=0;i<10;i++){landPulse[i]=Mathf.Max(0,landPulse[i]-dt*3);Color tint=Color.white;PinPopMatchTray.Item occupant=null;foreach(var entry in model.Items)if(entry.Presented&&entry.SlotTarget==i){occupant=entry;break;}bool occupied=occupant!=null;if(occupied&&occupant.Arrived)tint=Color.Lerp(Color.white,Art.Colors[occupant.Color],.34f+landPulse[i]*.3f);wells[i].color=tint;dots[i].enabled=!occupied;}

   ids.Clear();foreach(var item in model.Items){ids.Add(item.Id);if(!tiles.TryGetValue(item.Id,out var rect)){var image=Image("Collected color "+item.Id,0,0,47.5f,47.5f,Color.white);image.sprite=pinIcons[item.Color];image.type=UnityEngine.UI.Image.Type.Simple;image.preserveAspect=true;rect=image.rectTransform;
     var outline=image.gameObject.AddComponent<UnityEngine.UI.Outline>();outline.effectColor=Color.clear;outline.effectDistance=new Vector2(1,-1);tiles[item.Id]=rect;}
    rect.gameObject.SetActive(item.Presented);if(!item.Presented)continue;
    float target=SlotX(item.VisualSlot),x=target,y=CenterY,size=47.5f*trayScale*item.RenderScale,rotation=0,alpha=1;Vector2 squash=Vector2.one;
    if(!item.Arrived){float p=Mathf.Clamp01(item.Age/.48f),e=p<.5f?4*p*p*p:1-Mathf.Pow(-2*p+2,3)/2;float sx=item.Source.x*600,sy=(1-item.Source.y)*1000;x=Mathf.Lerp(sx,target,e);y=Mathf.Lerp(sy,y,e)-(game.ReducedMotion?0:Mathf.Sin(Mathf.PI*p)*item.Lift);size=(38+(47.5f*trayScale*item.RenderScale-38)*e)*(1+Mathf.Sin(Mathf.PI*p)*.25f);rotation=game.ReducedMotion?0:(1-e)*item.Spin;}
    else if(item.MergeAge>=0){float e=PinPopMatchTray.Ease(item.MergeAge/PinPopMatchTray.ConvergeDuration);float centerSlot=item.VisualSlot-(item.SlotGoal-item.SlotTarget);x=Mathf.LerpUnclamped(SlotX(item.MergeFromSlot),SlotX(centerSlot),e);size=47.5f*trayScale*Mathf.Lerp(item.RenderScale,1,Mathf.Clamp01(item.MergeAge/PinPopMatchTray.ConvergeDuration));if(item.MergeAge>PinPopMatchTray.ConvergeDuration){float k=Mathf.Clamp01((item.MergeAge-PinPopMatchTray.ConvergeDuration)/PinPopMatchTray.BlastDuration);float scale=k<.45f?Mathf.Lerp(1,1.35f,k/.45f):Mathf.Lerp(1.35f,0,(k-.45f)/.55f);size*=scale;alpha=1-k;}}
    if(item.Arrived&&item.LandAge<.3f&&item.MergeAge<PinPopMatchTray.ConvergeDuration&&!game.ReducedMotion){float p=item.LandAge/.3f;squash=p<.55f?Vector2.Lerp(new Vector2(1.35f,.7f),new Vector2(.9f,1.12f),p/.55f):Vector2.Lerp(new Vector2(.9f,1.12f),Vector2.one,(p-.55f)/.45f);}
    if(item.Arrived)x=Mathf.Clamp(x,trayLeft+size*squash.x/2,trayLeft+trayWidth-size*squash.x/2);
    Place(rect,x-size/2,y-size/2,size,size);rect.localRotation=Quaternion.Euler(0,0,rotation);rect.localScale=new Vector3(squash.x,squash.y,1);rect.GetComponent<Image>().color=new Color(1,1,1,alpha);
    bool pair=item.Arrived&&(item.Matched||colorCounts[item.Color]>=2);var glowColor=Color.Lerp(Art.Colors[item.Color],Color.white,.65f);glowColor.a=pair?alpha*(game.ReducedMotion?.75f:.62f+.18f*Mathf.Sin(visualClock*3)):0;rect.GetComponent<UnityEngine.UI.Outline>().effectColor=glowColor;
   }
   remove.Clear();foreach(var pair in tiles)if(!ids.Contains(pair.Key)){Destroy(pair.Value.gameObject);remove.Add(pair.Key);}foreach(int id in remove)tiles.Remove(id);
   for(int i=effects.Count-1;i>=0;i--){var e=effects[i];e.Age+=dt;float k=e.Age/e.Life;if(k>=1){if(e==comboEffect)comboEffect=null;Destroy(e.Rect.gameObject);effects.RemoveAt(i);continue;}var position=e.Origin+e.Velocity*e.Age;float size=e.Ring?10+100*k:8*(1-k);if(e.Graphic is Text){float w=Mathf.Min(360,trayWidth-8);Place(e.Rect,trayLeft+(trayWidth-w)/2,position.y-15,w,30);}else Place(e.Rect,position.x-size/2,position.y-size/2,size,size);var c=e.Graphic.color;c.a=1-k;e.Graphic.color=c;}
  }
  void OnEnable(){if(game!=null){game.MatchTray.Matched-=OnMatched;game.MatchTray.Matched+=OnMatched;game.MatchTray.Landed-=OnLanded;game.MatchTray.Landed+=OnLanded;}}
  void OnDisable(){if(game!=null){game.MatchTray.Matched-=OnMatched;game.MatchTray.Landed-=OnLanded;}}
  void OnLanded(int slot){if(slot>=0&&slot<10)landPulse[slot]=1;}
  void OnDestroy(){if(ring!=null){Destroy(ring.texture);Destroy(ring);}foreach(var sprite in surfaces){Destroy(sprite.texture);Destroy(sprite);}}
  void OnMatched(PinPopMatchTray.Item item,int combo){if(isActiveAndEnabled){matchPulse=1;Burst(item,combo);}}
  void Burst(PinPopMatchTray.Item item,int combo){Vector2 at=new Vector2(SlotX(item.MergeSlot),CenterY);Color color=Art.Colors[item.Color];
   var circle=Image("Match ring",0,0,10,10,color);circle.sprite=ring;circle.type=UnityEngine.UI.Image.Type.Simple;effects.Add(new Effect{Rect=circle.rectTransform,Graphic=circle,Origin=at,Life=.5f,Ring=true});
   if(!game.ReducedMotion)for(int i=0;i<10+combo*2;i++){float angle=Random.value*Mathf.PI*2,distance=Random.Range(30,75);var spark=Image("Match spark",0,0,8,8,color);effects.Add(new Effect{Rect=spark.rectTransform,Graphic=spark,Origin=at,Velocity=new Vector2(Mathf.Cos(angle)*distance,Mathf.Sin(angle)*distance-14)/.5f,Life=.5f});}
   string copy=combo>1?GameLocalization.Format("{0} x{1}",combo==2?"GÜZEL!":combo==3?"HARİKA!":combo==4?"SÜPER!":"MUHTEŞEM!",combo):"+3";
   if(comboEffect==null){var text=Label("Triple match",0,0,360,30,copy,19,color);text.alignment=TextAnchor.MiddleCenter;var outline=text.gameObject.AddComponent<UnityEngine.UI.Outline>();outline.effectColor=Art.Hex("173448");outline.effectDistance=new Vector2(1,-1);comboEffect=new Effect{Rect=text.rectTransform,Graphic=text,Life=1.05f,Velocity=new Vector2(0,20)};effects.Add(comboEffect);}
   ((Text)comboEffect.Graphic).text=copy;comboEffect.Graphic.color=color;comboEffect.Age=0;comboEffect.Origin=new Vector2(trayLeft+trayWidth/2,CenterY+70);

  }
  Sprite Surface(string topHex,string bottomHex,string edgeHex,bool outlineOnly){
   const int n=64;var texture=new Texture2D(n,n,TextureFormat.RGBA32,false){name="Tray polish surface",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};var pixels=new Color32[n*n];Color top=Art.Hex(topHex),bottom=Art.Hex(bottomHex),edge=Art.Hex(edgeHex);
   for(int y=0;y<n;y++)for(int x=0;x<n;x++){float qx=Mathf.Abs(x-31.5f)-19.5f,qy=Mathf.Abs(y-31.5f)-19.5f;float distance=Mathf.Sqrt(Mathf.Max(qx,0)*Mathf.Max(qx,0)+Mathf.Max(qy,0)*Mathf.Max(qy,0))+Mathf.Min(Mathf.Max(qx,qy),0)-11.5f;float alpha=Mathf.Clamp01(.5f-distance);Color c=Color.Lerp(bottom,top,y/63f);float rim=Mathf.Clamp01(1.7f+distance);c=Color.Lerp(c,edge,rim*.75f);c.a=alpha*(outlineOnly?rim:1);pixels[y*n+x]=c;}
   texture.SetPixels32(pixels);texture.Apply();var sprite=Sprite.Create(texture,new UnityEngine.Rect(0,0,n,n),Vector2.one*.5f,100,0,SpriteMeshType.FullRect,new Vector4(13,13,13,13));surfaces.Add(sprite);return sprite;
  }
  static Sprite RingSprite(){const int n=64;var t=new Texture2D(n,n,TextureFormat.RGBA32,false);var p=new Color32[n*n];for(int y=0;y<n;y++)for(int x=0;x<n;x++){float d=Mathf.Sqrt((x-31.5f)*(x-31.5f)+(y-31.5f)*(y-31.5f));p[y*n+x]=new Color32(255,255,255,(byte)(255*Mathf.Clamp01(2-Mathf.Abs(d-27))));}t.SetPixels32(p);t.Apply();return Sprite.Create(t,new UnityEngine.Rect(0,0,n,n),Vector2.one*.5f,100);}
 }
}
