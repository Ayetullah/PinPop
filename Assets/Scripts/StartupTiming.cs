using System.Diagnostics;
namespace PinPop {
 public static class StartupTiming {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
  static readonly System.Collections.Generic.Stack<System.Collections.Generic.KeyValuePair<string,long>> work=new System.Collections.Generic.Stack<System.Collections.Generic.KeyValuePair<string,long>>();
#endif
  [Conditional("UNITY_EDITOR"),Conditional("DEVELOPMENT_BUILD")]public static void Begin(string name){
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(DevicePolishReview.Active)work.Push(new System.Collections.Generic.KeyValuePair<string,long>(name,Stopwatch.GetTimestamp()));
#endif
  }
  [Conditional("UNITY_EDITOR"),Conditional("DEVELOPMENT_BUILD")]public static void End(){
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(!DevicePolishReview.Active||work.Count==0)return;var item=work.Pop();DevicePolishReview.RecordWork(item.Key,(Stopwatch.GetTimestamp()-item.Value)*1000f/Stopwatch.Frequency);
#endif
  }
 }
}
