using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PinPop {
 public sealed class LanguageDropdown:Dropdown {
  public RectTransform Arrow;
  public Graphic FocusRing;
  public bool IsExpanded=>openList!=null;
  RectTransform listRect;Vector2 listRest;float openedAt;bool listRestCaptured;
  GameObject openList;PinPopGame game;
  protected override GameObject CreateDropdownList(GameObject template){openList=base.CreateDropdownList(template);listRect=openList.GetComponent<RectTransform>();listRestCaptured=false;openedAt=Time.unscaledTime;
   var hit=CrispUI.Rect("LANGUAGE_LIST_CLOSE",openList.transform,204,-64,61,61);var touch=hit.gameObject.AddComponent<Image>();touch.color=Color.clear;touch.raycastTarget=true;
   var face=CrispUI.Panel("Language close circle",hit,16,16,29,29,Art.Hex("E3CBA3"),15);CrispUI.Icon("Language close cross",face.transform,7,7,15,15,CrispIcon.Kind.Close,Art.Hex("745E4A"));var button=hit.gameObject.AddComponent<Button>();button.targetGraphic=face;button.onClick.AddListener(Hide);hit.gameObject.AddComponent<HtmlButtonFeedback>().Artwork=face.rectTransform;
   return openList;}
  protected override GameObject CreateBlocker(Canvas rootCanvas){var blocker=base.CreateBlocker(rootCanvas);blocker.GetComponent<Button>().onClick.RemoveAllListeners();return blocker;}
  public override void OnCancel(BaseEventData eventData){}
  sealed class CrossOnlyItem:DropdownItem {public override void OnCancel(BaseEventData eventData){}}
  void LateUpdate(){
   if(game==null)game=FindFirstObjectByType<PinPopGame>();bool reduced=game!=null&&game.ReducedMotion;
   // Dropdown uses unscaled fade time. Honor the game's motion preference too.
   alphaFadeSpeed=reduced?0:.12f;
   if(Arrow!=null){float target=IsExpanded?180:0;float angle=reduced?target:Mathf.MoveTowardsAngle(Arrow.localEulerAngles.z,target,Time.unscaledDeltaTime*1200);Arrow.localRotation=Quaternion.Euler(0,0,angle);}
   if(FocusRing!=null)FocusRing.color=Art.Hex(IsExpanded?"B68B3C":"D9B970");
   if(listRect!=null){if(!listRestCaptured){listRest=listRect.anchoredPosition;listRestCaptured=true;}float t=reduced?1:Mathf.Clamp01((Time.unscaledTime-openedAt)/.16f);listRect.anchoredPosition=listRest+Vector2.up*(1-t)*(1-t)*5;}
  }
  protected override void OnDisable(){base.OnDisable();listRect=null;if(Arrow!=null)Arrow.localRotation=Quaternion.identity;if(FocusRing!=null)FocusRing.color=Art.Hex("D9B970");}

  protected override DropdownItem CreateItem(DropdownItem template){var original=base.CreateItem(template);original.enabled=false;var item=original.gameObject.AddComponent<CrossOnlyItem>();item.text=original.text;item.image=original.image;item.rectTransform=original.rectTransform;item.toggle=original.toggle;item.gameObject.AddComponent<LanguageChoiceCommit>().Label=item.text;var selection=item.toggle.graphic.gameObject.AddComponent<CanvasGroup>();selection.alpha=item.toggle.isOn?1:0;item.toggle.onValueChanged.AddListener(on=>selection.alpha=on?1:0);return item;}
 }
 // Unity's Dropdown does not send onValueChanged when the current row is chosen.
 // A deliberate tap on that row must still turn the initial automatic choice
 // into a saved language preference.
 sealed class LanguageChoiceCommit:MonoBehaviour,IPointerClickHandler,ISubmitHandler {
  public Text Label;
  void Commit(){if(Label==null)return;foreach(var choice in GameLocalization.SupportedLanguages)if(choice.nativeName==Label.text){GameLocalization.SetPreference(choice.code);return;}}
  public void OnPointerClick(PointerEventData data){if(data.button==PointerEventData.InputButton.Left)Commit();}
  public void OnSubmit(BaseEventData data){Commit();}
 }
}
