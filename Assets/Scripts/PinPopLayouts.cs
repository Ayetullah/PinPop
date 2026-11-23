using System.Collections.Generic;
using UnityEngine;
namespace PinPop {
 public static class PinPopLayouts {
  public struct Pin {public Vector3 Position;public int Color;}
  public static int ColorCount(int level)=>Mathf.Min(PinPopPalette.Count,3+Mathf.Max(0,level)/2);
  public static int ColorOffset(int level)=>level<12?0:(level/2)%PinPopPalette.Count;
  public static int QueueColor(int level,int index)=>(index+ColorOffset(level))%ColorCount(level);
  public static int TargetCount(int level)=>level==0?12:level==1?18:ColorCount(level)*6;
  public static List<Pin> Build(int level){
   level=Mathf.Max(0,level);var result=new List<Pin>();int colors=ColorCount(level);
   for(int group=0;group<colors;group++){
    int count=level==0?(group==0?6:3):6;
    for(int i=0;i<count;i++){
     int row=i/2,col=i%2;float x,z;int color=group;
     if(level<2){x=(group-1)*2.45f+(col-.5f)*.68f+(row%2)*.12f;if(count==3&&i==2)x=(group-1)*2.45f;z=-.2f+row*.88f+(level==1?group*.28f:0);}
     else {int band=group/4,columns=Mathf.Min(4,colors-band*4);float center=(group%4-(columns-1)*.5f)*1.96f;x=center+(col-.5f)*.68f+(row%2)*.10f;z=(colors<=4?-.2f:band==0?-1.8f:1.3f)+row*.84f;if(level%2==1&&i>=3)color=(group+1)%colors;}
     if(level>=20){int stage=(level-20)/5,pattern=(level-20)%5;float center=(group%4-1.5f)*1.92f,gap=.66f+stage*.025f;int band=group/4;x=center+(col-.5f)*gap;z=(band==0?-2.15f:1.15f)+row*(.78f+stage*.035f);
      if(pattern==0){x+=(row%2)*.12f;z+=(group%2)*.16f;}
      else if(pattern==1){z+=Mathf.Abs(center)*.12f;x+=(row-1)*.07f;}
      else if(pattern==2){x+=(row%2==0?-.13f:.13f);z+=col*.15f;}
      else if(pattern==3){x=center+(col-.5f)*gap*(1+row*.12f);z+=(group%2)*.22f;}
      else {x+=(band==0?-1:1)*(row-1)*.12f;z+=(group%4%2==0?.14f:-.14f);}
     }
     color=(color+ColorOffset(level))%colors;result.Add(new Pin{Position=new Vector3(x,.02f,z),Color=color});
    }
   }
   return result;
  }
 }
}
