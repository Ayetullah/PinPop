Shader "PinPop/Aim Power Overlay" {
 Properties { _Color("Color",Color)=(1,1,1,1) }
 SubShader {
  Tags { "Queue"="Overlay" "RenderType"="Transparent" }
  Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
  Pass { CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   fixed4 _Color;
   struct v2f { float4 position:SV_POSITION; };
   v2f vert(appdata_base v){v2f o;o.position=UnityObjectToClipPos(v.vertex);return o;}
   fixed4 frag(v2f i):SV_Target{return _Color;}
  ENDCG }
 }
}
