using System;
using System.Collections.Generic;
using UnityEngine;

namespace PinPop {
 // Smash_Match.html: reserve triples on insertion, wait for their flights, converge,
 // then blast and compact. Scaled time keeps the game's pause contract intact.
 public sealed class PinPopMatchTray:MonoBehaviour {
  public const int Capacity=10;
  public const float CollectHeight=-.8f,FlightDuration=.48f,ConvergeDuration=.32f,BlastDuration=.44f;
  public sealed class Item {
   public int Id,Color,Group;public bool Arrived,Matched,BlastAnnounced;public float Age,MergeAge=-1,MergeSlot,MergeFromSlot,VisualSlot,LandAge,SlotAge,SlotFrom;public int SlotTarget;public Vector2 Source;public float Spin,Lift;
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
  PinPopGame game;int nextId,nextGroup;float clock,lastMatch=-100;
  public void Build(PinPopGame owner){game=owner;}
  public void Clear(){Version++;Items.Clear();nextId=nextGroup=MatchCount=Combo=0;clock=0;lastMatch=-100;Changed?.Invoke();}
  public void Extract(Item item){if(item==null||item.Matched)return;Items.Remove(item);Version++;Changed?.Invoke();}
  public Item Collect(int color,Vector2 source){
   int index=Items.Count;for(int i=Items.Count-1;i>=0;i--)if(Items[i].Color==color&&!Items[i].Matched){index=i+1;break;}
   var item=new Item{Id=++nextId,Color=color,Source=source,VisualSlot=index,SlotTarget=index,SlotFrom=index,SlotAge=.22f,Spin=UnityEngine.Random.Range(-270f,270f),Lift=UnityEngine.Random.Range(70f,120f)};Items.Insert(index,item);
   var same=Items.FindAll(x=>x.Color==color&&!x.Matched);if(same.Count>=3){int group=++nextGroup;for(int i=0;i<3;i++){same[i].Matched=true;same[i].Group=group;}}
   Changed?.Invoke();return item;
  }
  public static float Ease(float t){if(t<=0)return 0;if(t>=1)return 1;float u=t;for(int i=0;i<8;i++){float q=1-u,x=3*q*q*u*.3f+3*q*u*u*.5f+u*u*u,dx=3*q*q*.3f+6*q*u*.2f+3*u*u*.5f;if(dx>.00001f)u=Mathf.Clamp01(u-(x-t)/dx);}float v=1-u;return 3*v*v*u*1.4f+3*v*u*u+u*u*u;}
  void Update(){if(game==null||game.InMenu||game.Transitioning||game.Paused||game.Finished)return;Tick(Time.deltaTime);}
  public void Tick(float dt){
   if(dt<=0)return;clock+=dt;bool changed=false;var groups=new Dictionary<int,List<Item>>();
   for(int i=0;i<Items.Count;i++){var item=Items[i];item.Age+=dt;if(item.MergeAge<0){if(item.SlotTarget!=i){item.SlotFrom=item.VisualSlot;item.SlotTarget=i;item.SlotAge=0;}item.SlotAge+=dt;item.VisualSlot=Mathf.LerpUnclamped(item.SlotFrom,item.SlotTarget,Ease(item.SlotAge/.22f));}if(!item.Arrived&&item.Age>=FlightDuration){item.Arrived=true;item.LandAge=0;Landed?.Invoke(i);changed=true;}if(item.Arrived)item.LandAge+=dt;if(item.Matched){if(!groups.TryGetValue(item.Group,out var list)){list=new List<Item>();groups[item.Group]=list;}list.Add(item);}}
   foreach(var list in groups.Values){if(list.Count!=3||!list.TrueForAll(x=>x.Arrived))continue;
    if(list[0].MergeAge<0){list.Sort((a,b)=>Items.IndexOf(a)-Items.IndexOf(b));float middle=Items.IndexOf(list[1]);foreach(var item in list){item.MergeAge=0;item.MergeSlot=middle;item.MergeFromSlot=item.VisualSlot;}}
    foreach(var item in list)item.MergeAge+=dt;
    if(list[0].MergeAge>=ConvergeDuration&&!list[0].BlastAnnounced){foreach(var item in list)item.BlastAnnounced=true;Combo=clock-lastMatch<2.2f?Combo+1:1;lastMatch=clock;MatchCount++;Matched?.Invoke(list[1],Combo);changed=true;}
    if(list[0].MergeAge>=ConvergeDuration+BlastDuration){foreach(var item in list)Items.Remove(item);changed=true;}
   }
   if(changed)Changed?.Invoke();
  }
 }
}
