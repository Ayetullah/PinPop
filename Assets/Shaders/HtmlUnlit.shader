Shader "PinPop/HTML Unlit" { Properties{_Color("Color",Vector)=(1,1,1,1) _MainTex("Texture",2D)="white"{} _Cull("Cull",Float)=2}
SubShader {Tags{"Queue"="Transparent" "RenderType"="Transparent"} Cull [_Cull] ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
Pass{CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
sampler2D _MainTex;float4 _MainTex_ST;float4 _Color;struct v2f{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;};v2f vert(appdata_base v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);return o;}fixed4 frag(v2f i):SV_Target{return tex2D(_MainTex,i.uv)*_Color;}
ENDCG}}}
