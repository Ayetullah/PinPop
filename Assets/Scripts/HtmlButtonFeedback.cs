using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace PinPop {
 // The hit target stays fixed; only the visible artwork responds to a press.
 public sealed class HtmlButtonFeedback:MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler {
  public RectTransform Artwork;Vector2 rest;Vector3 restScale;bool pressed,initialized;float response;static PinPopGame game;
  void Remember(){if(Artwork==null)return;rest=Artwork.anchoredPosition;restScale=Artwork.localScale;initialized=true;if(game==null)game=FindFirstObjectByType<PinPopGame>();}
  public void OnPointerDown(PointerEventData data){if(!GetComponent<Button>().IsInteractable())return;if(!initialized||response<=.001f)Remember();pressed=true;}
  public void OnPointerUp(PointerEventData data){pressed=false;}
  public void OnPointerExit(PointerEventData data){pressed=false;}
  void Update(){if(!initialized||Artwork==null)return;if(!GetComponent<Button>().IsInteractable())pressed=false;bool reduced=game!=null&&game.ReducedMotion;float target=pressed&&!reduced?1:0;response=Mathf.MoveTowards(response,target,Time.unscaledDeltaTime/(pressed?.08f:.12f));float eased=response*response*(3-2*response);Artwork.localScale=restScale*(1-.035f*eased);Artwork.anchoredPosition=rest+Vector2.down*(2*eased);}
  void OnDisable(){pressed=false;response=0;if(initialized&&Artwork!=null){Artwork.anchoredPosition=rest;Artwork.localScale=restScale;}initialized=false;}
 }
}
