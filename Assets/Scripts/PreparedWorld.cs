using UnityEngine;

namespace PinPop {
 // References in the imported prefab avoid parsing JSON and rebuilding meshes on a phone.
 public sealed class PreparedWorld : MonoBehaviour {
  public Transform Table, Cart, Wheel;
  public Transform[] Cabins;
  public Material Plaque;
 }
}
