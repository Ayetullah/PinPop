using UnityEngine;
namespace PinPop {
 // A serialized edit-mode camera prevents the empty Game view before Play.
 public sealed class PinPopSplashPreview:MonoBehaviour {
  void Awake(){if(Application.isPlaying)gameObject.SetActive(false);}
 }
}
