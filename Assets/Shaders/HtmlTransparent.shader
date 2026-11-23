Shader "PinPop/HTML Transparent" {
Properties { _Color("Color",Vector)=(1,1,1,1) _MainTex("Texture",2D)="white"{} _Glossiness("Smoothness",Range(0,1))=.5 _Metallic("Metal",Range(0,1))=0 _Cull("Cull",Float)=2 }
SubShader { Tags{"Queue"="Transparent" "RenderType"="Transparent"} Cull [_Cull]
CGPROGRAM
#pragma surface surf Standard alpha:fade fullforwardshadows
#pragma target 3.0
sampler2D _MainTex;fixed4 _Color;half _Glossiness,_Metallic;
struct Input{float3 worldPos;float2 uv_MainTex;};
void referenceFog(Input IN,SurfaceOutputStandard o,inout fixed4 color){float depth=-mul(UNITY_MATRIX_V,float4(IN.worldPos,1)).z;float fog=saturate((depth-38)/62);color.rgb=lerp(color.rgb,float3(.5457,.6867,.6724),fog);}
void surf(Input IN,inout SurfaceOutputStandard o){fixed4 c=tex2D(_MainTex,IN.uv_MainTex)*_Color;o.Albedo=c.rgb;o.Alpha=c.a;o.Metallic=_Metallic;o.Smoothness=_Glossiness;}
ENDCG
} FallBack "Transparent/Diffuse"
}
