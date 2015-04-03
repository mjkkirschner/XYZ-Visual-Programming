 
Shader "fadeshader" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
}
SubShader {
Tags {"RenderType" = "alpha-tested"}
//Blend SrcAlpha OneMinusSrcAlpha
 

CGPROGRAM
#pragma surface surf Standard alpha vertex:vert
#include "UnityCG.cginc"
 
 uniform fixed4 _Color;
 
 struct Input {
 float4 pos : SV_POSITION;
 float3 foo;
 };

  void vert (inout appdata_full v, out Input o) {
   // Transform to camera space
   UNITY_INITIALIZE_OUTPUT(Input, o);
   o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
   o.foo = float3(o.pos.z*.00583,0.0,0.0);
  
 }
 
 void surf (Input IN, inout SurfaceOutputStandard o)
 {
 o.Albedo = _Color;
 o.Alpha = IN.foo[0];
 o.Metallic = 0.5;
 o.Smoothness = .2;
 } 
ENDCG

}
Fallback "Standard"
}