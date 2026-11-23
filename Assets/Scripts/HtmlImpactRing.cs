using UnityEngine;
namespace PinPop {
 public sealed class HtmlImpactRing:MonoBehaviour {
  Material material;float age,duration;Color color;
  public void Build(Color tint,float seconds){color=tint;duration=seconds;material=new Material(Shader.Find("Sprites/Default"));Art.LandingRing(transform,"Impact ring",.286f,.314f,material,0);}
  void Update(){age+=Time.unscaledDeltaTime;float p=age/.38f;transform.localScale=Vector3.one*(1+p*2.5f);material.color=new Color(color.r,color.g,color.b,Mathf.Max(0,(duration-age)/.38f*.4f));if(age>=duration)Destroy(gameObject);}
  void OnDestroy(){if(material!=null)Destroy(material);}
 }
}
