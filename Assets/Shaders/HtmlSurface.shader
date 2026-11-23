Shader "PinPop/HTML Surface" {
Properties { _Color("Color",Vector)=(1,1,1,1) _MainTex("Texture",2D)="white"{} _Glossiness("Smoothness",Range(0,1))=.5 _Metallic("Metal",Range(0,1))=0 _VertexColors("Vertex colors",Float)=0 _Cull("Cull",Float)=2 }
SubShader { Tags{"RenderType"="Opaque"} Cull [_Cull]
CGPROGRAM
#pragma surface surf Standard fullforwardshadows vertex:vert finalcolor:referenceFog nofog
#pragma target 3.0
sampler2D _MainTex; fixed4 _Color; half _Glossiness,_Metallic; float _VertexColors;
struct Input { float facing:VFACE;float3 worldPos;float2 uv_MainTex; float4 color:COLOR; };
void vert(inout appdata_full v,out Input o){ UNITY_INITIALIZE_OUTPUT(Input,o);o.color=v.color; }
void referenceFog(Input IN,SurfaceOutputStandard o,inout fixed4 color){float depth=-mul(UNITY_MATRIX_V,float4(IN.worldPos,1)).z;float fog=saturate((depth-38)/62);color.rgb=lerp(color.rgb,float3(.5457,.6867,.6724),fog);}
void surf(Input IN,inout SurfaceOutputStandard o){fixed4 c=tex2D(_MainTex,IN.uv_MainTex)*_Color;if(_VertexColors>.5)c*=IN.color;o.Normal=float3(0,0,IN.facing>=0?1:-1);o.Albedo=c.rgb;o.Metallic=_Metallic;o.Smoothness=_Glossiness;o.Alpha=1;}
ENDCG
} FallBack "Standard"
}
