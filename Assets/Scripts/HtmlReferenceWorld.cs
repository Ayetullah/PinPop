using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PinPop {
 public sealed class HtmlReferenceWorld : MonoBehaviour {
  [Serializable] class Data { public Geometry[] geometries;public Surface[] materials;public TextureInfo[] textures;public Node[] nodes; }
  [Serializable] class Geometry {public float[] vertices,normals,uv,colors;public int[] indices;public Group[] groups;}
  [Serializable] class Group{public int start,count,material;}
  [Serializable] class TextureInfo{public string name;public float[] repeat,offset;}
  [Serializable] class Surface{public string name;public float[] color;public float roughness,metalness;public int texture;public bool transparent,unlit,doubleSided,vertexColors,plaque;}
  [Serializable] class Node{public string name;public int parent,geometry;public int[] materials;public float[] position,rotation,scale;public bool castShadow,receiveShadow;}
  public PinPopGame Model;public Transform Table {get;private set;} public Transform Cart {get;private set;}
  Transform wheel;readonly System.Collections.Generic.List<Transform> cabins=new System.Collections.Generic.List<Transform>(); float clock;Texture2D plaqueTexture,plaqueOriginal;Color32[] plaqueBase; Material plaque; int level=-1;string plaqueLanguage;
  public void Build(){
   var asset=Resources.Load<GameObject>("Prepared/World");
   if(asset==null)throw new InvalidOperationException("Prepared world is missing. Run PinPop/Prepare production assets before building.");
   var world=Instantiate(asset,transform,false).GetComponent<PreparedWorld>();
   Table=world.Table;Cart=world.Cart;wheel=world.Wheel;cabins.AddRange(world.Cabins);plaque=new Material(world.Plaque);plaqueOriginal=(Texture2D)world.Plaque.mainTexture;
   foreach(var renderer in world.GetComponentsInChildren<MeshRenderer>(true)){var assigned=renderer.sharedMaterials;bool changed=false;for(int i=0;i<assigned.Length;i++)if(assigned[i]==world.Plaque){assigned[i]=plaque;changed=true;}if(changed)renderer.sharedMaterials=assigned;}
  }
#if UNITY_EDITOR
  public PreparedWorld BuildSource(){
   var data=JsonUtility.FromJson<Data>(Resources.Load<TextAsset>("HtmlReference/scene").text);
   var mats=new Material[data.materials.Length];
   for(int i=0;i<mats.Length;i++){var s=data.materials[i];var m=new Material(Shader.Find(s.unlit?"PinPop/HTML Unlit":s.transparent?"PinPop/HTML Transparent":"PinPop/HTML Surface"));m.name=s.name;
    m.SetVector("_Color",new Vector4(s.color[0],s.color[1],s.color[2],s.color[3]));m.SetFloat("_Glossiness",1-s.roughness);m.SetFloat("_Metallic",s.metalness);m.SetFloat("_Cull",s.doubleSided?0:2);if(m.HasProperty("_VertexColors"))m.SetFloat("_VertexColors",s.vertexColors?1:0);
    if(s.texture>=0){var t=data.textures[s.texture];m.mainTexture=Resources.Load<Texture2D>("HtmlReference/"+t.name);m.mainTexture.wrapMode=TextureWrapMode.Repeat;m.mainTexture.anisoLevel=8;m.mainTextureScale=new Vector2(t.repeat[0],t.repeat[1]);m.mainTextureOffset=new Vector2(t.offset[0],t.offset[1]);}mats[i]=m;if(s.plaque)plaque=m;
   }
   var meshes=new Mesh[data.geometries.Length];for(int i=0;i<meshes.Length;i++){var g=data.geometries[i];var mesh=new Mesh{name="HTML geometry "+i,indexFormat=IndexFormat.UInt32};var v=new Vector3[g.vertices.Length/3];var n=new Vector3[v.Length];for(int j=0;j<v.Length;j++){v[j]=new Vector3(g.vertices[j*3],g.vertices[j*3+1],-g.vertices[j*3+2]);if(g.normals.Length>0)n[j]=new Vector3(g.normals[j*3],g.normals[j*3+1],-g.normals[j*3+2]);}mesh.vertices=v;if(g.normals.Length>0)mesh.normals=n;
    if(g.uv.Length>0){var uv=new Vector2[v.Length];for(int j=0;j<uv.Length;j++)uv[j]=new Vector2(g.uv[j*2],g.uv[j*2+1]);mesh.uv=uv;}
    if(g.colors.Length>0){var colors=new Color[v.Length];for(int j=0;j<colors.Length;j++)colors[j]=new Color(g.colors[j*3],g.colors[j*3+1],g.colors[j*3+2]);mesh.colors=colors;}
    var groups=g.groups.Length==0?new[]{new Group{start=0,count=g.indices.Length,material=0}}:g.groups;mesh.subMeshCount=groups.Length;
    for(int k=0;k<groups.Length;k++){var group=groups[k];var tris=new int[group.count];for(int j=0;j<tris.Length;j+=3){tris[j]=g.indices[group.start+j];tris[j+1]=g.indices[group.start+j+2];tris[j+2]=g.indices[group.start+j+1];}mesh.SetTriangles(tris,k);}mesh.RecalculateBounds();meshes[i]=mesh;
   }
   transform.localPosition=Vector3.down*1.25f;var objects=new Transform[data.nodes.Length];
   for(int i=0;i<objects.Length;i++){var d=data.nodes[i];var o=new GameObject(d.name).transform;o.SetParent(d.parent<0?transform:objects[d.parent],false);o.localPosition=new Vector3(d.position[0],d.position[1],-d.position[2]);o.localRotation=new Quaternion(-d.rotation[0],-d.rotation[1],d.rotation[2],d.rotation[3]);o.localScale=new Vector3(d.scale[0],d.scale[1],d.scale[2]);objects[i]=o;
    // Keep the lettering in front of the cart trim (front face at z=-1.085).
    if(d.name=="Plaque")o.localPosition=new Vector3(o.localPosition.x,o.localPosition.y,-Mathf.Max(d.position[2],1.10f));
    if(d.geometry>=0){o.gameObject.AddComponent<MeshFilter>().sharedMesh=meshes[d.geometry];var renderer=o.gameObject.AddComponent<MeshRenderer>();var g=data.geometries[d.geometry];var materialIds=d.materials;if(g.groups.Length>0){var assigned=new Material[g.groups.Length];for(int k=0;k<assigned.Length;k++)assigned[k]=mats[materialIds[Mathf.Min(g.groups[k].material,materialIds.Length-1)]];renderer.sharedMaterials=assigned;}else renderer.sharedMaterial=mats[materialIds[0]];renderer.shadowCastingMode=d.castShadow?ShadowCastingMode.On:ShadowCastingMode.Off;renderer.receiveShadows=d.receiveShadow;}
    if(d.parent>=0&&data.nodes[d.parent].name=="Wheel"&&d.geometry<0)cabins.Add(o);if(d.name=="Table")Table=o;if(d.name=="Cart")Cart=o;if(d.name=="Wheel")wheel=o;
   }
   var prepared=gameObject.AddComponent<PreparedWorld>();prepared.Table=Table;prepared.Cart=Cart;prepared.Wheel=wheel;prepared.Cabins=cabins.ToArray();prepared.Plaque=plaque;return prepared;
  }
#endif
  public void SetHome(bool home){Table.gameObject.SetActive(!home);Cart.gameObject.SetActive(home);}
  public void SetLevel(int value){if(plaque==null||level==value&&plaqueLanguage==GameLocalization.LanguageCode)return;level=value;
   if(plaqueBase==null||plaqueLanguage!=GameLocalization.LanguageCode){plaqueLanguage=GameLocalization.LanguageCode;plaqueBase=(plaqueOriginal??(Texture2D)plaque.mainTexture).GetPixels32();for(int y=0;y<160;y++)for(int x=0;x<512;x++)plaqueBase[y*512+x]=new Color32(0,0,0,0);if(!GameLocalization.Turkish){var heading=Resources.Load<Texture2D>("Localization/plaque-en");if(heading==null)throw new InvalidOperationException("Prepared English plaque is missing");var strip=heading.GetPixels32();System.Array.Copy(strip,0,plaqueBase,160*512,strip.Length);}}
   if(plaqueTexture==null){plaqueTexture=new Texture2D(512,256,TextureFormat.RGBA32,false);plaqueTexture.wrapMode=TextureWrapMode.Clamp;plaque.mainTexture=plaqueTexture;}
   var pixels=(Color32[])plaqueBase.Clone();string text=(value+1).ToString();int left=Mathf.RoundToInt(256-text.Length*75.08f/2);
   foreach(char c in text){var glyph=Resources.Load<Texture2D>("HtmlReference/plaque-digit-"+c);var data=glyph.GetPixels32();for(int y=0;y<160;y++)for(int x=0;x<110;x++){int dx=left+x,dy=y+25;if(dx>=0&&dx<512&&dy<160&&data[y*110+x].a>0)pixels[dy*512+dx]=data[y*110+x];}left+=75;}
   plaqueTexture.SetPixels32(pixels);plaqueTexture.Apply();}
  void Update(){if(Model!=null&&Model.ReducedMotion){if(wheel!=null)wheel.localRotation=Quaternion.Euler(0,0,.12f*Mathf.Rad2Deg);foreach(var cabin in cabins)cabin.localRotation=Quaternion.Inverse(wheel.localRotation);if(Cart!=null)Cart.localRotation=Quaternion.identity;return;}clock+=Time.unscaledDeltaTime;if(wheel!=null)wheel.localRotation=Quaternion.Euler(0,0,clock*.035f*Mathf.Rad2Deg);foreach(var cabin in cabins)cabin.localRotation=Quaternion.Inverse(wheel.localRotation);if(Cart!=null)Cart.localRotation=Quaternion.Euler(0,-Mathf.Sin(clock*.22f)*.018f*Mathf.Rad2Deg,0);}
 }
}
