using UnityEngine;
using UnityEngine.Rendering;
namespace PinPop {
 // The aiming endpoint carries power; the trajectory keeps the ball's matching color.
 public sealed class AimPowerRing:MonoBehaviour {
  public const int Segments=96;
  public float Power {get;private set;}
  public Color Tint {get;private set;}
  public bool Maximum=>Power>=.995f;
  Mesh arc;MeshRenderer arcRenderer;Material arcMaterial,haloMaterial,trackMaterial;Transform halo;
  readonly Vector3[] vertices=new Vector3[(Segments+1)*2];float drawnPower=-1;
  void Awake(){
   arc=new Mesh{name="Power ring arc"};arc.MarkDynamic();var indices=new int[Segments*6];for(int i=0;i<Segments;i++){int n=i*2,k=i*6;indices[k]=n;indices[k+1]=n+1;indices[k+2]=n+2;indices[k+3]=n+1;indices[k+4]=n+3;indices[k+5]=n+2;}arc.vertices=vertices;arc.triangles=indices;
   gameObject.AddComponent<MeshFilter>().sharedMesh=arc;arcRenderer=gameObject.AddComponent<MeshRenderer>();arcMaterial=new Material(Resources.Load<Shader>("AimPowerOverlay"));arcMaterial.renderQueue=4002;arcRenderer.sharedMaterial=arcMaterial;arcRenderer.shadowCastingMode=ShadowCastingMode.Off;arcRenderer.receiveShadows=false;
   trackMaterial=new Material(Resources.Load<Shader>("AimPowerOverlay")){color=new Color(.10f,.20f,.26f,.92f),renderQueue=4000};Art.LandingRing(transform,"Power ring track",.64f,.80f,trackMaterial,-.003f);
   haloMaterial=new Material(Resources.Load<Shader>("AimPowerOverlay")){renderQueue=4001};Art.LandingRing(transform,"Maximum power glow",.80f,.96f,haloMaterial,-.001f);halo=transform.Find("Maximum power glow");SetProgress(0,true);
  }
  public void SetProgress(float power,bool reducedMotion){
   Power=Mathf.Clamp01(power);Tint=Power<.5f?Color.Lerp(Art.Hex("43CEC8"),Art.Hex("FFD15C"),Power*2):Color.Lerp(Art.Hex("FFD15C"),Art.Hex("F27672"),(Power-.5f)*2);arcMaterial.color=Tint;arcRenderer.enabled=Power>.001f;
   if(!Mathf.Approximately(drawnPower,Power)){drawnPower=Power;for(int i=0;i<=Segments;i++){float angle=Mathf.PI/2-Mathf.PI*2*Power*i/Segments;var direction=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));vertices[i*2]=direction*.56f;vertices[i*2+1]=direction*.80f;}arc.vertices=vertices;arc.RecalculateBounds();}
   float pulse=reducedMotion?0:Mathf.Sin(Time.unscaledTime*4);halo.gameObject.SetActive(Maximum);haloMaterial.color=new Color(Tint.r,Tint.g,Tint.b,.10f+pulse*.035f);halo.localScale=Vector3.one*(1+pulse*.025f);
  }
  void OnDestroy(){if(arc!=null)Destroy(arc);if(arcMaterial!=null)Destroy(arcMaterial);if(haloMaterial!=null)Destroy(haloMaterial);if(trackMaterial!=null)Destroy(trackMaterial);}
 }
}
