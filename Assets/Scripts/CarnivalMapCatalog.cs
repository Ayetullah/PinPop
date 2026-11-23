using System;
using UnityEngine;
namespace PinPop {
 [Serializable] public class CarnivalRegion {public string id,title,subtitle,accent,ink,plate;public string[] names,icons,extendedNames;}
 [Serializable] public class CarnivalCatalog {public CarnivalRegion[] regions;}
 public static class CarnivalMapCatalog {
  static CarnivalCatalog data;public static CarnivalRegion Region(int id){if(data==null)data=JsonUtility.FromJson<CarnivalCatalog>(Resources.Load<TextAsset>("MapArt/catalog").text);return data.regions[Mathf.Clamp(id,0,3)];}
  public static string StopName(int level){var theme=Region((level/AdventureRules.MapPageSize)%4);return (level>=20?theme.extendedNames:theme.names)[level%AdventureRules.MapPageSize];}
  public static string StopIcon(int level)=>Region((level/AdventureRules.MapPageSize)%4).icons[level%AdventureRules.MapPageSize];
  public const float MapViewportHeight=820,MapCenter=410;
  public static Vector2 ScrollStop(int index)=>new Vector2(new[]{160f,410f,180f,400f}[index%4],MapCenter+index*140+(index/AdventureRules.MapPageSize)*100);
  public static float ScrollHeight(int count)=>ScrollStop(Mathf.Max(0,count-1)).y+MapCenter;
  public static readonly Vector2[] Stops={new Vector2(160,365),new Vector2(410,445),new Vector2(180,525),new Vector2(400,605),new Vector2(220,685)};
  public static string Detail(int level){int n=(AdventureRules.Fixed(level)?1:0)+(AdventureRules.Ice(level)?1:0)+(AdventureRules.Band(level)?1:0)+(AdventureRules.Moving(level)?1:0);string kind=n>1?"Karışık":AdventureRules.Ice(level)?"Buz":AdventureRules.Band(level)?"Bağ":AdventureRules.Moving(level)?"Geçit":AdventureRules.Fixed(level)?"Tampon":"Üçlü";return PinPopLayouts.ColorCount(level)+" renk · "+kind;}
 }
}
