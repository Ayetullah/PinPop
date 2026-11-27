using UnityEngine;
using UnityEngine.UI;
namespace PinPop {
 // Keep labels/icons in the same disabled state as their decorative surface.
 public sealed class PopupControlState:MonoBehaviour {
  Selectable control;CanvasGroup artwork;bool last;bool fresh;
  public void Bind(Selectable owner,RectTransform visual){control=owner;artwork=visual.GetComponent<CanvasGroup>()??visual.gameObject.AddComponent<CanvasGroup>();Apply();}
  void LateUpdate(){Apply();}
  void Apply(){if(control==null||artwork==null)return;bool enabled=control.IsInteractable();if(fresh&&enabled==last)return;fresh=true;last=enabled;artwork.alpha=enabled?1:.6f;}
 }
}
