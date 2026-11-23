using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace PinPop {
 public sealed class HtmlButtonFeedback:MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler {
  public RectTransform Artwork;Vector2 rest;bool pressed;
  void Awake(){rest=Vector2.zero;}
  public void OnPointerDown(PointerEventData data){if(!GetComponent<Button>().interactable)return;rest=Artwork.anchoredPosition;pressed=true;Artwork.anchoredPosition=rest+Vector2.down*3;}
  public void OnPointerUp(PointerEventData data){Release();}public void OnPointerExit(PointerEventData data){Release();}void OnDisable(){Release();}
  void Release(){if(pressed&&Artwork!=null)Artwork.anchoredPosition=rest;pressed=false;}
 }
}
