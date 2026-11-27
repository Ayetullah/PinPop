using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PinPop {
 // Closed spherical wedges replace the whole ball: solid fragments, not a dust dissolve.
 // Cosmetic motion never adds colliders or impulses to the game world.
 public sealed class BallDespawnFX:MonoBehaviour {
  public const float Duration=1.1f,FadeStart=.72f;
  public const int FragmentCount=20;
  public float Age {get;private set;}
  public int BounceCount {get;private set;}
  public Transform[] Fragments {get;private set;}
  struct Piece {public Mesh mesh;public Vector3 pivot;}
  static Piece[] geometry;
  Vector3[] velocities,spin;bool fromTable;
  public void Build(Transform ballVisual,Color tint,bool rainbow){
   if(geometry==null)geometry=BuildGeometry();
   Vector3 origin=ballVisual!=null?ballVisual.position:transform.position;Quaternion rotation=ballVisual!=null?ballVisual.rotation:transform.rotation;
   Material shell=ballVisual!=null?ballVisual.GetComponent<Renderer>().sharedMaterial:Art.Mat(tint,.6f);
   if(ballVisual!=null)ballVisual.gameObject.SetActive(false);
   fromTable=PinPopGame.OnTable(origin)&&origin.y>-.1f;
   Fragments=new Transform[FragmentCount];velocities=new Vector3[FragmentCount];spin=new Vector3[FragmentCount];
   for(int i=0;i<FragmentCount;i++){
    var piece=geometry[i];var obj=new GameObject("Ball fragment "+(i+1));obj.layer=gameObject.layer;obj.transform.SetParent(transform,false);Fragments[i]=obj.transform;
    obj.transform.SetPositionAndRotation(origin+rotation*piece.pivot*BowlingBall.Radius,rotation);obj.transform.localScale=Vector3.one*BowlingBall.Radius;
    obj.AddComponent<MeshFilter>().sharedMesh=piece.mesh;var renderer=obj.AddComponent<MeshRenderer>();Color color=rainbow?Art.Colors[i%Art.Colors.Length]:tint;
    renderer.sharedMaterials=new[]{rainbow?Art.Mat(color,.6f):shell,Art.Mat(Color.Lerp(color,Art.Hex("49384D"),.32f),.18f)};
    renderer.shadowCastingMode=ShadowCastingMode.On;renderer.receiveShadows=true;
    Vector3 radial=rotation*piece.pivot;Vector3 flat=new Vector3(radial.x,0,radial.z).normalized;
    // Fixed variation keeps tests/replays consistent without consuming gameplay random state.
    float variation=Mathf.Repeat(i*.618034f,1);
    velocities[i]=flat*Mathf.Lerp(1.1f,1.8f,variation)+Vector3.up*Mathf.Lerp(1.35f,2.25f,1-variation);
    spin[i]=new Vector3(121+i*17,237-i*11,89+i*23);
   }
  }
  void Update(){
   float dt=Mathf.Min(Time.unscaledDeltaTime,.05f);Age+=Time.unscaledDeltaTime;
   if(Fragments!=null)for(int i=0;i<Fragments.Length;i++){
    var part=Fragments[i];if(part==null)continue;
    velocities[i]+=Vector3.down*(7.5f*dt);Vector3 next=part.position+velocities[i]*dt;
    float floor=fromTable&&PinPopGame.OnTable(next)?0:PinPopGame.GrassHeight;floor+=.085f;
    if(next.y<floor&&velocities[i].y<0){next.y=floor;velocities[i]=new Vector3(velocities[i].x*.64f,-velocities[i].y*.3f,velocities[i].z*.64f);spin[i]*=.55f;BounceCount++;}
    part.position=next;part.Rotate(spin[i]*dt,Space.Self);
    // Keep chunks solid and legible through the breakup; only clear debris at the end.
    part.localScale=Vector3.one*(BowlingBall.Radius*(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(FadeStart,Duration,Age))));
   }
   if(Age>=Duration)Destroy(gameObject);
  }
  static Piece[] BuildGeometry(){
   float t=(1+Mathf.Sqrt(5))*.5f;
   var v=new[]{new Vector3(-1,t,0),new Vector3(1,t,0),new Vector3(-1,-t,0),new Vector3(1,-t,0),new Vector3(0,-1,t),new Vector3(0,1,t),new Vector3(0,-1,-t),new Vector3(0,1,-t),new Vector3(t,0,-1),new Vector3(t,0,1),new Vector3(-t,0,-1),new Vector3(-t,0,1)};
   for(int i=0;i<v.Length;i++)v[i]=v[i].normalized;
   int[] faces={0,11,5,0,5,1,0,1,7,0,7,10,0,10,11,1,5,9,5,11,4,11,10,2,10,7,6,7,1,8,3,9,4,3,4,2,3,2,6,3,6,8,3,8,9,4,9,5,2,4,11,6,2,10,8,6,7,9,8,1};
   var pieces=new Piece[FragmentCount];
   for(int face=0;face<FragmentCount;face++)pieces[face]=Wedge(v[faces[face*3]],v[faces[face*3+1]],v[faces[face*3+2]],face);
   return pieces;
  }
  static Piece Wedge(Vector3 a,Vector3 b,Vector3 c,int index){
   const int n=4;Vector3 pivot=(a+b+c).normalized*.72f;
   var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var outer=new List<int>();var inner=new List<int>();
   System.Func<int,int,Vector3> point=(i,j)=>(a+(b-a)*(i/(float)n)+(c-a)*(j/(float)n)).normalized;
   System.Action<Vector3,Vector3,Vector3,bool> triangle=(x,y,z,shell)=>{
    Vector3 normal=Vector3.Cross(y-x,z-x).normalized;Vector3 expected=shell?(x+y+z).normalized:((x+y+z)/3-pivot);
    if(Vector3.Dot(normal,expected)<0){var swap=y;y=z;z=swap;normal=-normal;}
    int start=vertices.Count;foreach(var p in new[]{x,y,z}){vertices.Add(p-pivot);normals.Add(shell?p:normal);uv.Add(new Vector2(.5f+Mathf.Atan2(p.x,p.z)/(2*Mathf.PI),.5f+Mathf.Asin(Mathf.Clamp(p.y,-1,1))/Mathf.PI));}
    var indices=shell?outer:inner;indices.Add(start);indices.Add(start+1);indices.Add(start+2);
   };
   for(int i=0;i<n;i++)for(int j=0;j<n-i;j++){
    triangle(point(i,j),point(i+1,j),point(i,j+1),true);
    if(i+j<n-1)triangle(point(i+1,j),point(i+1,j+1),point(i,j+1),true);
   }
   Vector3[] corners={a,b,c};for(int edge=0;edge<3;edge++)for(int k=0;k<n;k++)triangle(Vector3.zero,Vector3.Lerp(corners[edge],corners[(edge+1)%3],k/(float)n).normalized,Vector3.Lerp(corners[edge],corners[(edge+1)%3],(k+1)/(float)n).normalized,false);
   var mesh=new Mesh{name="Cached fractured sphere wedge "+index,subMeshCount=2};mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(outer,0);mesh.SetTriangles(inner,1);mesh.RecalculateBounds();
   return new Piece{mesh=mesh,pivot=pivot};
  }
 }
}
