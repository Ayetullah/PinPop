using System.Collections.Concurrent;
using Unity.Collections;
using UnityEngine;
namespace PinPop {
 // Match HTML's pair-specific material table; callbacks can run on physics worker threads.
 public sealed class HtmlPhysicsContacts:MonoBehaviour {
  public enum Kind{Floor=1,Pin=2,Ball=3,Grass=4}
  static readonly ConcurrentDictionary<int,Kind> kinds=new ConcurrentDictionary<int,Kind>();
  void OnEnable(){Physics.ContactModifyEvent+=Modify;Physics.ContactModifyEventCCD+=Modify;}
  void OnDisable(){Physics.ContactModifyEvent-=Modify;Physics.ContactModifyEventCCD-=Modify;kinds.Clear();}
  public static void Register(Collider collider,Kind kind){collider.hasModifiableContacts=true;int id=collider.GetInstanceID();kinds[id]=kind;collider.gameObject.AddComponent<HtmlContactLifetime>().Id=id;}
  internal static void Remove(int id){kinds.TryRemove(id,out _);}
  // Separate sliding pins from rolling balls; benchmark overrides last only for its process.
  public static float PinFloorFriction=.14f;
  public static Vector2 Material(Kind a,Kind b){bool pinFloor=(a==Kind.Pin&&b==Kind.Floor)||(a==Kind.Floor&&b==Kind.Pin);bool grass=a==Kind.Grass||b==Kind.Grass;return new Vector2(grass?.72f:pinFloor?PinFloorFriction:.45f,grass?.02f:.05f);}
  static void Modify(PhysicsScene scene,NativeArray<ModifiableContactPair> pairs){foreach(var pair in pairs){if(!kinds.TryGetValue(pair.colliderInstanceID,out var a)||!kinds.TryGetValue(pair.otherColliderInstanceID,out var b))continue;var material=Material(a,b);for(int i=0;i<pair.contactCount;i++){pair.SetDynamicFriction(i,material.x);pair.SetStaticFriction(i,material.x);pair.SetBounciness(i,material.y);}}}
 }
 public sealed class HtmlContactLifetime:MonoBehaviour{public int Id;void OnDestroy(){HtmlPhysicsContacts.Remove(Id);}}
}
