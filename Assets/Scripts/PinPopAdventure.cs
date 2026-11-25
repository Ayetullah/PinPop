using System;
using System.Collections.Generic;
using UnityEngine;
namespace PinPop {
 [Serializable] public class AdventureSave {public int version=1,chests,lessons;public int[] stock={3,2,2,2};}
 public static class AdventureRules {
  public const int Chapters=40,LevelsPerRegion=20,MapPageSize=5;
  public static int RegionCount=>(Chapters+LevelsPerRegion-1)/LevelsPerRegion;
  public static int MapPageCount=>(Chapters+MapPageSize-1)/MapPageSize;
  public static int RegionForPage(int page)=>page*MapPageSize/LevelsPerRegion;
  public static int RegionSize(int region)=>Mathf.Min(LevelsPerRegion,Chapters-region*LevelsPerRegion);
  public static bool CanVisitRegion(int region,int highest)=>region>=0&&region<RegionCount&&highest>=region*LevelsPerRegion;
  public static int FocusForRegion(int region,int highest)=>Mathf.Clamp(ClampLevel(highest),region*LevelsPerRegion,region*LevelsPerRegion+RegionSize(region)-1);
  public static int ClampLevel(int level)=>Mathf.Clamp(level,0,Chapters-1);
  public static bool FinalLevel(int level)=>level>=Chapters-1;
  public static bool RegionChest(int page)=>(page+1)*MapPageSize%LevelsPerRegion==0||(page+1)*MapPageSize>=Chapters;
  public static readonly string[] Regions={"LUNAPARK KAPISI","DÖNME DOLAP","OYUN ÇADIRI","YILDIZ FESTİVALİ"};
  public static readonly string[] Boosters={"Renk seç","Gökkuşağı","Tepsi cebi","+3 hamle"};
  public static readonly string[] Symbols={"●","✦","▣","+3"};
  public static readonly int[] Prices={20,35,25,30};
  public static bool Fixed(int level)=>level>=2&&(level%4==2||level>=12&&level%4==0);
  public static bool Ice(int level)=>level>=4&&(level%4==0||level>=12&&level%4==2);
  public static bool Band(int level)=>level>=6&&(level%4==2||level>=12&&level%4==1);
  public static bool Moving(int level)=>level>=8&&(level%4==0||level>=12&&level%4==3);
  public static string Preview(int level){var a=new List<string>();if(Fixed(level))a.Add("Sabit engel");if(Ice(level))a.Add("Buzlu duba");if(Band(level))a.Add("Bağlı üçlü");if(Moving(level))a.Add("Hareketli bariyer");if(level>=2&&level<=10&&level%2==0)a.Insert(0,GameLocalization.Format("{0} açılır",PinPopPalette.Names[PinPopLayouts.ColorCount(level)-1]));return a.Count==0?"Aynı renkleri üçle":string.Join(" · ",a.ConvertAll(GameLocalization.Text));}
 }
 public sealed class PinPopAdventure:MonoBehaviour {
  public const string SaveKey="PinPop.Adventure.v1";
  public AdventureSave Data {get;private set;}=new AdventureSave();
  public int PocketColor {get;private set;}=-1;
  public bool RainbowArmed {get;private set;}public bool ShotRainbow {get;private set;}
  public int LastChest {get;private set;}public int LastChestCoins {get;private set;}
  PinPopGame game;Transform world;readonly List<PinTarget> band=new List<PinTarget>();Rigidbody moving;float phase;
  public void Build(PinPopGame owner){game=owner;Reload();game.MatchTray.Changed+=ReturnPocket;}
  public void Reload(){try{Data=JsonUtility.FromJson<AdventureSave>(PlayerPrefs.GetString(SaveKey,""))??new AdventureSave();}catch{Data=new AdventureSave();}if(Data.stock==null||Data.stock.Length!=4)Data.stock=new[]{3,2,2,2};for(int i=0;i<4;i++)Data.stock[i]=Mathf.Clamp(Data.stock[i],0,999);Data.chests&=(1<<AdventureRules.MapPageCount)-1;Save();}
  public void Save(){PlayerPrefs.SetString(SaveKey,JsonUtility.ToJson(Data));PlayerPrefs.Save();}
  public void ResetLevel(){PocketColor=-1;RainbowArmed=ShotRainbow=false;LastChest=LastChestCoins=0;band.Clear();moving=null;phase=0;if(world!=null){world.gameObject.SetActive(false);Destroy(world.gameObject);}world=new GameObject("Adventure obstacles").transform;world.SetParent(transform,false);
   if(AdventureRules.Fixed(game.Level)){Barrier("Left bumper",new Vector3(-1.75f,.38f,-3.55f),new Vector3(1.15f,.76f,.42f),false);Barrier("Right bumper",new Vector3(1.75f,.38f,-3.55f),new Vector3(1.15f,.76f,.42f),false);}
   if(AdventureRules.Moving(game.Level))moving=Barrier("Moving festival gate",new Vector3(0,.42f,-4.6f),new Vector3(1.1f,.84f,.35f),true);
   if(AdventureRules.Ice(game.Level))for(int c=0;c<Mathf.Min(4,PinPopLayouts.ColorCount(game.Level));c++){var p=game.Pins.Find(x=>x.ColorId==c);if(p!=null)p.Protect(true,false);}
   if(AdventureRules.Band(game.Level)){var group=game.Pins.FindAll(x=>x.ColorId==PinPopLayouts.QueueColor(game.Level,1));for(int i=0;i<Mathf.Min(3,group.Count);i++){group[i].Protect(false,true);band.Add(group[i]);}}
  }
  Rigidbody Barrier(string name,Vector3 at,Vector3 size,bool move){var obj=Art.Shape(name,PrimitiveType.Cube,at,size,Art.Mat(Art.Hex(move?"BA8AD2":"46758B")),world,true);var col=obj.GetComponent<Collider>();if(col==null)col=obj.AddComponent<BoxCollider>();col.sharedMaterial=BowlingBall.ContactMaterial;var stripe=Art.Shape("Cream bumper stripe",PrimitiveType.Cube,Vector3.zero,new Vector3(.92f,.16f,1.025f),Art.Mat(Art.Hex("F8E7B9")),obj.transform);stripe.transform.localPosition=new Vector3(0,.15f,0);if(!move)return null;var body=obj.AddComponent<Rigidbody>();body.isKinematic=true;return body;}
  public bool IsObstacle(Collider collider)=>collider!=null&&world!=null&&collider.transform.IsChildOf(world);
  void FixedUpdate(){if(game==null||game.InMenu||game.Paused||game.Transitioning||game.Finished)return;if(moving!=null){phase+=Time.fixedDeltaTime;moving.MovePosition(new Vector3(Mathf.Sin(phase*.85f)*2.35f,.42f,-4.6f));}}
  void LateUpdate(){if(world!=null)world.gameObject.SetActive(!game.InMenu);}
  public void BeginShot(){ShotRainbow=RainbowArmed;RainbowArmed=false;}
  public void BreakBand(){foreach(var p in band)if(p!=null&&!p.Cleared)p.ReleaseBand();game.Hud.Toast("BAĞ KOPTU · SONRAKİ ATIŞTA SERBEST",2);}
  bool Available=>!game.InMenu&&!game.Transitioning&&!game.Busy&&!game.IsPulling&&!game.Paused&&!game.Finished;
  public bool Buy(int id){if(id<0||id>3||game.Busy||game.Transitioning||game.Finished||Data.stock[id]>=999||!game.SpendCoins(AdventureRules.Prices[id]))return false;Data.stock[id]++;Save();return true;}
  public bool Use(int id,int color=-1,int itemId=-1){if(id<0||id>3||!Available||Data.stock[id]<=0)return false;
   if(id==0){if(color<0||color>=PinPopLayouts.ColorCount(game.Level)||!game.Pins.Exists(p=>p!=null&&!p.Cleared&&p.ColorId==color))return false;game.ChangeReadyColor(color);}
   if(id==1){if(RainbowArmed)return false;RainbowArmed=true;game.ShowRainbowReady();}
   if(id==2){if(PocketColor>=0)return false;var item=game.MatchTray.Items.Find(x=>x.Id==itemId&&!x.Matched&&x.Arrived);if(item==null)return false;PocketColor=item.Color;game.MatchTray.Extract(item);}
   if(id==3)game.AddBoosterMoves();
   Data.stock[id]--;Save();game.Hud.Toast(GameLocalization.Format("{0} kullanıldı",AdventureRules.Boosters[id]),1.5f);return true;
  }
  void ReturnPocket(){if(PocketColor<0)return;int color=PocketColor;if(game.MatchTray.Items.FindAll(x=>x.Color==color&&!x.Matched).Count<2)return;PocketColor=-1;game.MatchTray.Collect(color,new Vector2(.5f,.18f));game.Hud.Toast("CEPTEKİ DUBA ÜÇLÜYÜ TAMAMLADI",2);}
  public bool ClaimChest(int region){if(region<0||region>=AdventureRules.MapPageCount||(Data.chests&(1<<region))!=0||game.HighestLevel<(region+1)*AdventureRules.MapPageSize)return false;Data.chests|=1<<region;LastChest=region+1;LastChestCoins=new[]{25,40,60,100}[region%4];Data.stock[region%4]=Mathf.Min(999,Data.stock[region%4]+1);Save();game.GrantCoins(LastChestCoins);return true;}
  public void Complete(){if((game.Level+1)%AdventureRules.MapPageSize==0&&game.Level<AdventureRules.Chapters)ClaimChest(game.Level/AdventureRules.MapPageSize);}
  public string Lesson(){int bit=-1;string text="";int c=PinPopLayouts.ColorCount(game.Level);if(c>3&&((Data.lessons>>(c-4))&1)==0){bit=c-4;text=GameLocalization.Format("{0} AÇILDI",Art.Names[c-1])+"\n"+GameLocalization.Format("{0} dubalarını aynı renk topla eşleştir.\nToplanan üç aynı figür birleşip silinir.",CharacterName(c-1));}
   else if(AdventureRules.Fixed(game.Level)&&(Data.lessons&(1<<5))==0){bit=5;text="SEKME ENGELLERİ\nTopu ön kenarda sağa–sola kaydır. Engellerin yanından açı bularak at.";}
   else if(AdventureRules.Ice(game.Level)&&(Data.lessons&(1<<6))==0){bit=6;text="BUZLU DUBA\nAynı renkte bir top çarptığında buz kırılır.\nDuba sonraki atışta hareket edebilir.";}
   else if(AdventureRules.Band(game.Level)&&(Data.lessons&(1<<7))==0){bit=7;text="BAĞLI ÜÇLÜ\nAynı renkte bir top bağı koparır.\nÜç duba sonraki atışta serbest kalır.";}
   else if(AdventureRules.Moving(game.Level)&&(Data.lessons&(1<<8))==0){bit=8;text="HAREKETLİ BARİYER\nGeçit açılana kadar bekle, ardından atışını bırak.";}
   if(bit>=0){Data.lessons|=1<<bit;Save();}return text;
  }
  public static string CharacterName(int c)=>new[]{"Tavşan","Penguen","Kedi","Aslan","Ayı","Kurbağa","Fil","Tilki"}[c];
 }
}
