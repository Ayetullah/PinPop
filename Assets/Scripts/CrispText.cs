using UnityEngine;
using UnityEngine.UI;

namespace PinPop {
 // Legacy panels use reference coordinates. Rasterize their glyphs at the final
 // screen density rather than magnifying a small font atlas with the panel.
 public sealed class CrispText:Text {
  public string SourceText {get;private set;}
  public override string text {get=>base.text;set{SourceText=value;base.text=GameLocalization.Text(value);}}
  readonly UIVertex[] quad=new UIVertex[4];float previousDensity;bool atlasChanged;
  public float RasterDensity {
   get {if(canvas==null||font==null||!font.dynamic)return 1;var parent=canvas.transform.lossyScale;var own=rectTransform.lossyScale;float ratio=Mathf.Max(Mathf.Abs(own.x/Mathf.Max(.000001f,Mathf.Abs(parent.x))),Mathf.Abs(own.y/Mathf.Max(.000001f,Mathf.Abs(parent.y))));return Mathf.Clamp(Mathf.Ceil(ratio*4)/4,1,4);}
  }
  protected override void OnEnable(){base.OnEnable();Font.textureRebuilt+=AtlasRebuilt;GameLocalization.Changed+=Relocalize;atlasChanged=true;Relocalize();}
  protected override void OnDisable(){Font.textureRebuilt-=AtlasRebuilt;GameLocalization.Changed-=Relocalize;base.OnDisable();}
  void Relocalize(){if(SourceText!=null)base.text=GameLocalization.Text(SourceText);}
  void AtlasRebuilt(Font changed){if(changed==font)atlasChanged=true;}
  void LateUpdate(){float density=RasterDensity;if(atlasChanged||!Mathf.Approximately(density,previousDensity)){atlasChanged=false;previousDensity=density;cachedTextGenerator.Invalidate();SetVerticesDirty();}}
  protected override void OnPopulateMesh(VertexHelper output){
   output.Clear();if(font==null)return;float density=RasterDensity;
   m_DisableFontTextureRebuiltCallback=true;
   try {
    var settings=GetGenerationSettings(rectTransform.rect.size);settings.scaleFactor*=density;
    cachedTextGenerator.PopulateWithErrors(text,settings,gameObject);var vertices=cachedTextGenerator.verts;if(vertices.Count==0)return;
    float units=1/(pixelsPerUnit*density);Vector2 first=(Vector2)vertices[0].position*units;Vector2 offset=PixelAdjustPoint(first)-first;
    for(int i=0;i<vertices.Count;i++){int corner=i&3;quad[corner]=vertices[i];quad[corner].position*=units;quad[corner].position+=(Vector3)offset;if(corner==3)output.AddUIVertexQuad(quad);}
   } finally {m_DisableFontTextureRebuiltCallback=false;}
  }
 }
}
