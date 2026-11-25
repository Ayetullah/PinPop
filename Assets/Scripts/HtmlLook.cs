using UnityEngine;
namespace PinPop {
 [RequireComponent(typeof(Camera))] public sealed class HtmlLook : MonoBehaviour {
  Material effect; public bool Home;public float ModalBlur;
  void Awake(){GetComponent<Camera>().allowHDR=true;GetComponent<Camera>().depthTextureMode|=DepthTextureMode.Depth;var shader=Shader.Find("Hidden/PinPop/HtmlLook");if(shader!=null&&shader.isSupported)effect=new Material(shader);}
  void OnPreCull(){GetComponent<Camera>().depthTextureMode=Home||ModalBlur>0?DepthTextureMode.Depth:DepthTextureMode.None;}
  void OnDestroy(){if(effect!=null)Destroy(effect);}
  void OnRenderImage(RenderTexture source,RenderTexture target){if(effect==null){Graphics.Blit(source,target);return;}effect.SetFloat("_ModalBlur",ModalBlur);effect.SetFloat("_Focus",Home?17:1000);effect.SetFloat("_Exposure",.96f);Graphics.Blit(source,target,effect);}
 }
}
