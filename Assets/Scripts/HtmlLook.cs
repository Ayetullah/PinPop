using UnityEngine;
namespace PinPop {
 [RequireComponent(typeof(Camera))] public sealed class HtmlLook : MonoBehaviour {
  Material effect; public bool Home;public float ModalBlur;
  void Awake(){GetComponent<Camera>().allowHDR=true;GetComponent<Camera>().depthTextureMode|=DepthTextureMode.Depth;effect=new Material(Shader.Find("Hidden/PinPop/HtmlLook"));}
  void OnDestroy(){if(effect!=null)Destroy(effect);}
  void OnRenderImage(RenderTexture source,RenderTexture target){effect.SetFloat("_ModalBlur",ModalBlur);effect.SetFloat("_Focus",Home?17:28);effect.SetFloat("_Exposure",.96f);Graphics.Blit(source,target,effect);}
 }
}
