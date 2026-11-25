using System;
using System.Collections.Generic;
using UnityEngine;

namespace PinPop {
 // Smash_Match.html: reserve triples on insertion, wait for their flights, converge,
 // then blast and compact. Overflow finishes reserved flights while physics is frozen.
 public sealed class PinPopMatchTray:MonoBehaviour {
  public const int Capacity=10;
  public const float CollectHeight=-.8f,FlightDuration=.48f,SettleDuration=.22f,ConvergeDuration=.32f,BlastDuration=.44f;
  public sealed class Item {
   public int Id,Color,Group;public bool Arrived,Matched,BlastAnnounced,Presented;public float RenderScale=1,SlotGoal;public float Age,MergeAge=-1,MergeSlot,MergeFromSlot,VisualSlot,LandAge,SlotAge,SlotFrom;public int SlotTarget;public Vector2 Source;public float Spin,Lift;
  }
  public readonly List<Item> Items=new List<Item>();
  public int LiveCount {get {int n=0;foreach(var item in Items)if(!item.Matched)n++;return n;}}
  public bool Empty=>Items.Count==0;
  public bool Pending=>Items.Exists(x=>!x.Arrived||x.Matched);
  public int MatchCount {get;private set;}
  public int Combo {get;private set;}
  public int Version {get;private set;}
  public event Action Changed;
  public event Action<Item,int> Matched;
  public event Action<int> Landed;
  sealed class Presentation {public readonly List<Item> Items=new List<Item>();public bool Visible;public int Wells=>Items[0].MergeAge>=ConvergeDuration*.4f?0:Items.Count;}
  readonly List<Presentation> presentations=new List<Presentation>(),presentationPool=new List<Presentation>();
  int presentationUsed;
  Presentation NewPresentation(){if(presentationUsed==presentationPool.Count)presentationPool.Add(new Presentation());var unit=presentationPool[presentationUsed++];presentations.Add(unit);return unit;}
  readonly Dictionary<int,Presentation> presentationGroups=new Dictionary<int,Presentation>();
  // Reserve a separate well for every figure until the triple has left the board.
  // Extra reservations wait for a visible well; their flight clocks do not run off-screen.
  void ArrangePresentation(){
   presentations.Clear();presentationGroups.Clear();presentationUsed=0;foreach(var unit in presentationPool){unit.Items.Clear();unit.Visible=false;}
   foreach(var item in Items){Presentation unit;if(item.Matched){if(!presentationGroups.TryGetValue(item.Group,out unit)){unit=NewPresentation();presentationGroups[item.Group]=unit;}}else {unit=NewPresentation();}unit.Items.Add(item);unit.Visible|=item.Presented;}
   int used=0;foreach(var unit in presentations)if(unit.Visible){if(used+unit.Wells<=Capacity)used+=unit.Wells;else unit.Visible=false;}
   foreach(var unit in presentations)if(!unit.Visible&&used+unit.Wells<=Capacity){unit.Visible=true;used+=unit.Wells;}
   int slot=0;foreach(var unit in presentations){for(int i=0;i<unit.Items.Count;i++){var item=unit.Items[i];item.Presented=unit.Visible;if(!unit.Visible||item.MergeAge>=0)continue;int target=slot+i;if(item.SlotTarget!=target||Mathf.Abs(item.SlotGoal-target)>.001f){item.SlotFrom=item.VisualSlot;item.SlotTarget=target;item.SlotGoal=target;item.SlotAge=0;}}if(unit.Visible)slot+=unit.Wells;}
  }
  PinPopGame game;int nextId,nextGroup;float clock,lastMatch=-100;
  public void Build(PinPopGame owner){game=owner;}
  public void Clear(){Version++;Items.Clear();nextId=nextGroup=MatchCount=Combo=0;clock=0;lastMatch=-100;Changed?.Invoke();}
  public void Extract(Item item){if(item==null||item.Matched)return;Items.Remove(item);Version++;Changed?.Invoke();}
  public Item Collect(int color,Vector2 source){
   int index=Items.Count;for(int i=Items.Count-1;i>=0;i--)if(Items[i].Color==color&&!Items[i].Matched){index=i+1;break;}
   var item=new Item{Id=++nextId,Color=color,Source=source,VisualSlot=Mathf.Min(index,Capacity-1),SlotGoal=Mathf.Min(index,Capacity-1),SlotTarget=Mathf.Min(index,Capacity-1),SlotFrom=Mathf.Min(index,Capacity-1),SlotAge=.22f,Spin=UnityEngine.Random.Range(-270f,270f),Lift=UnityEngine.Random.Range(70f,120f)};Items.Insert(index,item);
   var same=Items.FindAll(x=>x.Color==color&&!x.Matched);if(same.Count>=3){int group=++nextGroup;for(int i=0;i<3;i++){same[i].Matched=true;same[i].Group=group;}}
   Changed?.Invoke();return item;
  }
  public static float Ease(float t){if(t<=0)return 0;if(t>=1)return 1;float u=t;for(int i=0;i<8;i++){float q=1-u,x=3*q*q*u*.3f+3*q*u*u*.5f+u*u*u,dx=3*q*q*.3f+6*q*u*.2f+3*u*u*.5f;if(dx>.00001f)u=Mathf.Clamp01(u-(x-t)/dx);}float v=1-u;return 3*v*v*u*1.4f+3*v*u*u+u*u*u;}
  void Update(){if(game==null||game.InMenu||game.Transitioning||game.Finished||game.Paused&&!game.TrayOverflow)return;Tick(game.TrayOverflow?Time.unscaledDeltaTime:Time.deltaTime);}
  public void Tick(float dt){
   if(dt<=0)return;clock+=dt;bool changed=false;
   ArrangePresentation();
   for(int i=0;i<Items.Count;i++){var item=Items[i];
    if(!item.Presented)continue;
    item.Age+=dt;item.RenderScale=Mathf.MoveTowards(item.RenderScale,1,dt*4);
    item.SlotAge+=dt;item.VisualSlot=Mathf.Lerp(item.SlotFrom,item.SlotGoal,Ease(item.SlotAge/.22f));
    if(!item.Arrived&&item.Age>=FlightDuration){item.Arrived=true;item.LandAge=item.Age-FlightDuration;Landed?.Invoke(item.SlotTarget);changed=true;}else if(item.Arrived)item.LandAge+=dt;
   }
   foreach(var unit in presentations){var list=unit.Items;if(!list[0].Matched||list.Count!=3||!list.TrueForAll(x=>x.Arrived&&x.Presented&&(x.MergeAge>=0||x.LandAge>=SettleDuration&&x.SlotAge>=.22f)))continue;
    float mergeStep=dt;if(list[0].MergeAge<0){foreach(var entry in list)mergeStep=Mathf.Min(mergeStep,Mathf.Max(0,Mathf.Min(entry.LandAge-SettleDuration,entry.SlotAge-.22f)));list.Sort((a,b)=>Items.IndexOf(a)-Items.IndexOf(b));float middle=list[1].SlotTarget;foreach(var item in list){item.MergeAge=0;item.MergeSlot=middle;item.MergeFromSlot=item.VisualSlot;}}
    foreach(var item in list)item.MergeAge+=mergeStep;
    if(list[0].MergeAge>=ConvergeDuration&&!list[0].BlastAnnounced){foreach(var item in list)item.BlastAnnounced=true;Combo=clock-lastMatch<2.2f?Combo+1:1;lastMatch=clock;MatchCount++;Matched?.Invoke(list[1],Combo);changed=true;}
    if(list[0].MergeAge>=ConvergeDuration+BlastDuration){foreach(var item in list)Items.Remove(item);changed=true;}
   }
   if(changed)Changed?.Invoke();
  }
 }
}
