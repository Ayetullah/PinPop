using System;
using System.Collections.Generic;
using UnityEngine;

namespace PinPop
{
    [Serializable] public sealed class LevelEarnings { public int level; public int bestMoves; }
    [Serializable] public sealed class EconomySave
    {
        public int version=2,coins,swaps=3,ownedMask=1,selectedBall,legacyRewardedBefore,highestLevel;
        public List<LevelEarnings> earnings=new List<LevelEarnings>();
    }
    public sealed class PinPopEconomy
    {
        public const string SaveKey="PinPop.Economy.v2";
        public const int MaxCoins=999999999,MaxSwaps=999,BaseReward=30,MoveBonus=2,SwapCost=10;
        public static readonly string[] BallNames={"Klasik","Mermer","Yörünge","Galaksi"};
        public static readonly int[] BallPrices={0,90,180,300};
        public EconomySave Data {get;private set;}
        public PinPopEconomy(EconomySave save){Data=save??new EconomySave();Normalize();}
        public static int MoveBudget(int level)=>PinPopLayouts.TargetCount(level);
        public void Normalize(){
            Data.version=2;Data.coins=Mathf.Clamp(Data.coins,0,MaxCoins);Data.swaps=Mathf.Clamp(Data.swaps,0,MaxSwaps);Data.ownedMask=Data.ownedMask>=1&&Data.ownedMask<=15?(Data.ownedMask|1):1;
            Data.legacyRewardedBefore=Mathf.Max(0,Data.legacyRewardedBefore);Data.highestLevel=Mathf.Max(Data.legacyRewardedBefore,Data.highestLevel);if(Data.selectedBall<0||Data.selectedBall>=4||!Owns(Data.selectedBall))Data.selectedBall=0;
            var clean=new List<LevelEarnings>();if(Data.earnings!=null)foreach(var row in Data.earnings){if(row==null||row.level<0||row.level<Data.legacyRewardedBefore)continue;var existing=clean.Find(x=>x.level==row.level);Data.highestLevel=Mathf.Max(Data.highestLevel,row.level==int.MaxValue?int.MaxValue:row.level+1);int best=Mathf.Clamp(row.bestMoves,0,MoveBudget(row.level));if(existing==null)clean.Add(new LevelEarnings{level=row.level,bestMoves=best});else existing.bestMoves=Mathf.Max(existing.bestMoves,best);}Data.earnings=clean;
        }
        public bool Owns(int id)=>id>=0&&id<4&&(Data.ownedMask&(1<<id))!=0;
        public bool BuyBall(int id){if(id<=0||id>=4||Owns(id)||Data.coins<BallPrices[id])return false;Data.coins-=BallPrices[id];Data.ownedMask|=1<<id;Data.selectedBall=id;return true;}
        public bool Equip(int id){if(!Owns(id))return false;Data.selectedBall=id;return true;}
        public bool BuySwap(){if(Data.coins<SwapCost||Data.swaps>=MaxSwaps)return false;Data.coins-=SwapCost;Data.swaps++;return true;}
        public bool UseSwap(){if(Data.swaps<=0)return false;Data.swaps--;return true;}
        public int Complete(int level,int moves,out int basePaid,out int bonusPaid){
            basePaid=bonusPaid=0;if(level<0)return 0;Data.highestLevel=Mathf.Max(Data.highestLevel,level==int.MaxValue?int.MaxValue:level+1);if(level<Data.legacyRewardedBefore)return 0;int best=Mathf.Clamp(moves,0,MoveBudget(level));var row=Data.earnings.Find(x=>x.level==level);int basis=0,bonus=0;
            if(row==null){basis=BaseReward;bonus=best*MoveBonus;Data.earnings.Add(new LevelEarnings{level=level,bestMoves=best});}
            else if(best>row.bestMoves){bonus=(best-row.bestMoves)*MoveBonus;row.bestMoves=best;}
            int paid=Mathf.Min(basis+bonus,MaxCoins-Data.coins);basePaid=Mathf.Min(basis,paid);bonusPaid=paid-basePaid;Data.coins+=paid;return paid;
        }
        public static PinPopEconomy Load(){
            EconomySave data=null;if(PlayerPrefs.HasKey(SaveKey))try{data=JsonUtility.FromJson<EconomySave>(PlayerPrefs.GetString(SaveKey));}catch(ArgumentException){}
            if(data==null||data.version!=2)data=new EconomySave{coins=PlayerPrefs.GetInt("Coins",0),swaps=PlayerPrefs.GetInt("SwapCredits",3),legacyRewardedBefore=Mathf.Max(0,PlayerPrefs.GetInt("Level",0)),highestLevel=Mathf.Max(0,PlayerPrefs.GetInt("Level",0))};
            data.highestLevel=Mathf.Max(data.highestLevel,PlayerPrefs.GetInt("Level",0));var economy=new PinPopEconomy(data);economy.Save();return economy;
        }
        public void Save(){PlayerPrefs.SetString(SaveKey,JsonUtility.ToJson(Data));PlayerPrefs.SetInt("Coins",Data.coins);PlayerPrefs.SetInt("SwapCredits",Data.swaps);PlayerPrefs.Save();}
        public static string Format(int value)=>value.ToString("N0",System.Globalization.CultureInfo.GetCultureInfo("tr-TR"));
        public static string Compact(int value){if(value<10000)return value.ToString();if(value<1000000)return((decimal)(value/100)/10m).ToString("0.#",System.Globalization.CultureInfo.GetCultureInfo("tr-TR"))+"K";return((decimal)(value/100000)/10m).ToString("0.#",System.Globalization.CultureInfo.GetCultureInfo("tr-TR"))+"M";}
    }
}
