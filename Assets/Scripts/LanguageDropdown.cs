using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PinPop {
 public sealed class LanguageDropdown:Dropdown {
  public RectTransform Arrow;
  GameObject openList;PinPopGame game;
  protected override GameObject CreateDropdownList(GameObject template){openList=base.CreateDropdownList(template);return openList;}
  void LateUpdate(){if(Arrow==null)return;if(game==null)game=FindFirstObjectByType<PinPopGame>();float target=openList!=null?180:0;float angle=game!=null&&game.ReducedMotion?target:Mathf.MoveTowardsAngle(Arrow.localEulerAngles.z,target,Time.unscaledDeltaTime*1200);Arrow.localRotation=Quaternion.Euler(0,0,angle);}
  protected override void OnDisable(){base.OnDisable();if(Arrow!=null)Arrow.localRotation=Quaternion.identity;}

  protected override DropdownItem CreateItem(DropdownItem template){var item=base.CreateItem(template);item.gameObject.AddComponent<LanguageChoiceCommit>().Label=item.text;var selection=item.toggle.graphic.gameObject.AddComponent<CanvasGroup>();selection.alpha=item.toggle.isOn?1:0;item.toggle.onValueChanged.AddListener(on=>selection.alpha=on?1:0);return item;}
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
