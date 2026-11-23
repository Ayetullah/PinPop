Shader "Hidden/PinPop/HtmlLook" {Properties{_MainTex("Texture",2D)="white"{} _Focus("Focus",Float)=30 _Exposure("Exposure",Float)=.96}
SubShader{Cull Off ZWrite Off ZTest Always Pass{CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"
sampler2D _MainTex;float4 _MainTex_TexelSize;UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);float _Focus,_Exposure,_ModalBlur;
float3 aces(float3 c){c*=_Exposure/.6;c=float3(dot(c,float3(.59719,.35458,.04823)),dot(c,float3(.076,.90834,.01566)),dot(c,float3(.02840,.13383,.83777)));float3 a=c*(c+.0245786)-.000090537,b=c*(.983729*c+.432951)+.238081;c=a/b;return saturate(float3(dot(c,float3(1.60475,-.53108,-.07367)),dot(c,float3(-.10208,1.10813,-.00605)),dot(c,float3(-.00327,-.07276,1.07602))));}
float4 frag(v2f_img i):SV_Target{float z=LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv));float amount=max(_ModalBlur,smoothstep(_Focus+3,_Focus+22,z));float3 c=tex2D(_MainTex,i.uv).rgb,b=0;for(int x=-2;x<=2;x++)for(int y=-2;y<=2;y++){float a=x==0?6:abs(x)==1?4:1,d=y==0?6:abs(y)==1?4:1;b+=tex2D(_MainTex,i.uv+float2(x,y)*amount*1.5*_MainTex_TexelSize.xy).rgb*a*d/256;}return float4(aces(lerp(c,b,amount*.7)),1);}
ENDCG}}}
