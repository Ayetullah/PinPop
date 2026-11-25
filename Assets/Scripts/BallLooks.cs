using System;
using System.Collections.Generic;
using UnityEngine;
namespace PinPop {
 public static class BallLooks {
  static readonly Dictionary<Vector2Int,Material> materials=new Dictionary<Vector2Int,Material>();
  static readonly Dictionary<Vector3Int,Sprite> icons=new Dictionary<Vector3Int,Sprite>();
  public static Color Paint(int style,int color,double u,double v){
   Color basis=Art.Colors[Mathf.Clamp(color,0,PinPopPalette.Count-1)],cream=Art.Hex("FFF1D5");double blend=0;
   if(style==1){double wave=Math.Sin(u*38+Math.Sin(v*17)*2.4+Math.Sin(u*11+v*19));blend=Math.Pow(Math.Max(0,wave),10)*.68;}
   if(style==2){double stripe=Math.Abs(Math.Sin((v+.045*Math.Sin(u*Math.PI*2))*Math.PI*8));blend=stripe>.95?.85:0;}
   if(style==3){basis*=.82f;basis.a=1;double cloud=(Math.Sin(u*19+v*13)+Math.Sin(u*31-v*11))*.5;basis=Color.Lerp(basis,Art.Colors[color],(float)(.5+cloud*.35));int x=(int)(u*48),y=(int)(v*24);double hash=Math.Sin(x*12.9898+y*78.233)*43758.5453;hash-=Math.Floor(hash);double dx=u*48-x-.5,dy=v*24-y-.5;if(hash>.94&&dx*dx+dy*dy<.025)blend=.95;}
   return Color.Lerp(basis,cream,(float)blend);
  }
  public static Material Material(int style,int color){
   if(style==0)return Art.Mat(Art.Colors[color],.6f);var key=new Vector2Int(style,color);if(materials.TryGetValue(key,out var found)&&found!=null)return found;
   var texture=Resources.Load<Texture2D>("Prepared/Balls/coat-"+style+"-"+color);if(texture==null)throw new InvalidOperationException("Prepared ball coat is missing");
   var material=new Material(Shader.Find("Standard")){name="Bowling look "+style+" / "+color,color=Color.white,mainTexture=texture};material.SetFloat("_Glossiness",.7f);material.SetFloat("_Metallic",.03f);materials[key]=material;return material;
  }
  public static Sprite Icon(int style,int color,int pixelSize=0){
   var preparedKey=new Vector3Int(style,color,512);if(icons.TryGetValue(preparedKey,out var prepared)&&prepared!=null)return prepared;
   prepared=Resources.Load<Sprite>("Prepared/Balls/icon-"+style+"-"+color);if(prepared!=null){icons[preparedKey]=prepared;return prepared;}
#if !UNITY_EDITOR
   throw new InvalidOperationException("Prepared ball icon is missing");
#else
   return GenerateIcon(style,color,pixelSize);
#endif
  }
#if UNITY_EDITOR
  public static Texture2D GenerateTexture(int style,int color){var texture=new Texture2D(512,256,TextureFormat.RGBA32,false){name="Ball look "+style+" / "+color,wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear,anisoLevel=4};var pixels=new Color32[512*256];for(int y=0;y<256;y++)for(int x=0;x<512;x++)pixels[y*512+x]=Paint(style,color,x/512.0,y/256.0);texture.SetPixels32(pixels);texture.Apply();return texture;}
  public static Sprite GenerateIcon(int style,int color,int pixelSize=512){
   if(pixelSize==0)pixelSize=Mathf.CeilToInt(110*Screen.width/600f);int size=pixelSize<=128?128:pixelSize<=256?256:512;var key=new Vector3Int(style,color,size);if(icons.TryGetValue(key,out var found)&&found!=null)return found;var texture=new Texture2D(size,size,TextureFormat.RGBA32,false){name="Ball icon "+style+" / "+color,filterMode=FilterMode.Bilinear};var pixels=new Color32[size*size];
   for(int y=0;y<size;y++)for(int x=0;x<size;x++){double px=(x+.5)*128/size-.5,py=(y+.5)*128/size-.5,nx=(px-63.5)/61,ny=(py-63.5)/61,r=nx*nx+ny*ny;if(r>=1){pixels[y*size+x]=Color.clear;continue;}double z=Math.Sqrt(1-r),u=(Math.Atan2(nx,z)/Math.PI+1)*.5,v=(ny+1)*.5;Color c=Paint(style,color,u,v);float light=(float)(.53+.47*Math.Max(0,(-nx*.35+ny*.45+z*.82)));c*=light;c.a=(float)Math.Min(1,(1-r)*size*.4);double shine=Math.Exp(-((nx+.32)*(nx+.32)+(ny-.4)*(ny-.4))/.025)*.38;c=Color.Lerp(c,Color.white,(float)shine);if((px-61)*(px-61)/16.0+(py-87)*(py-87)/25.0<1||(px-79)*(px-79)/16.0+(py-79)*(py-79)/25.0<1||(px-63)*(px-63)/16.0+(py-66)*(py-66)/25.0<1)c=Art.Hex("263051");pixels[y*size+x]=c;}
   texture.SetPixels32(pixels);texture.Apply();var sprite=Sprite.Create(texture,new Rect(0,0,size,size),new Vector2(.5f,.5f),100);sprite.name="look-"+style+"-color-"+color;icons[key]=sprite;return sprite;
  }
#endif
 }
}
