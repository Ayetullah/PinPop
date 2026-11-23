using UnityEngine;
namespace PinPop {
 // Small cached UI textures, created once. No imported texture readback.
 public static class HelpDemoArt {
  static Sprite wood,sky,shadow,ring,disc,card,plate;
  public static Sprite Wood=>wood??(wood=Make(256,128,(x,y)=>{float grain=Mathf.Sin(x*.63f+Mathf.Sin(y*.09f)*.8f)*.004f+Mathf.Sin(x*.19f)*.006f;float seam=x%43==0?.035f:0;var c=Color.Lerp(new Color(.91f,.83f,.61f),new Color(.77f,.64f,.40f),y/128f*.35f)+new Color(grain-seam,grain-seam,grain-seam,0);c.a=Corner(x,y,256,128,7);return c;}));
  public static Sprite Sky=>sky??(sky=Make(256,168,(x,y)=>{var c=Color.Lerp(new Color(.48f,.63f,.49f),new Color(.70f,.78f,.60f),y/167f);c.a=Corner(x,y,256,168,10);return c;}));
  public static Sprite Card=>card??(card=Make(256,384,(x,y)=>{var c=Color.Lerp(new Color(.15f,.26f,.34f),new Color(.24f,.39f,.46f),y/383f);c.a=Corner(x,y,256,384,14);return c;}));
  public static Sprite Plate=>plate??(plate=Make(256,384,(x,y)=>new Color(1,1,1,Corner(x,y,256,384,14))));
  public static Sprite Shadow=>shadow??(shadow=Make(64,32,(x,y)=>{float r=Mathf.Sqrt(Mathf.Pow((x-31.5f)/31,2)+Mathf.Pow((y-15.5f)/15,2));return new Color(1,1,1,Mathf.Pow(Mathf.Clamp01(1-r),1.5f));}));
  public static Sprite Ring=>ring??(ring=Make(64,64,(x,y)=>{float r=Vector2.Distance(new Vector2(x,y),Vector2.one*31.5f);return new Color(1,1,1,Mathf.Clamp01(2-Mathf.Abs(r-27)));}));
  public static Sprite Disc=>disc??(disc=Make(64,64,(x,y)=>new Color(1,1,1,Mathf.Clamp01(31.5f-Vector2.Distance(new Vector2(x,y),Vector2.one*31.5f)))));
  static float Corner(int x,int y,int w,int h,float r){float dx=Mathf.Max(r-x,Mathf.Max(0,x-(w-1-r))),dy=Mathf.Max(r-y,Mathf.Max(0,y-(h-1-r)));return Mathf.Clamp01(r+.5f-Mathf.Sqrt(dx*dx+dy*dy));}
  static Sprite Make(int w,int h,System.Func<int,int,Color> pixel){var t=new Texture2D(w,h,TextureFormat.RGBA32,false){filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};var p=new Color[w*h];for(int y=0;y<h;y++)for(int x=0;x<w;x++)p[y*w+x]=pixel(x,y);t.SetPixels(p);t.Apply();return Sprite.Create(t,new Rect(0,0,w,h),Vector2.one*.5f,100);}
 }
}
