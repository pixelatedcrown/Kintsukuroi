Shader "Custom/Vertex Color/Unlit VC with Transparent Pulse" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _Color2 ("Color 2", Color) = (1,1,1,1)
    _ColorMod ("Color Mod", Color) = (1,1,1,1)
    _Speed("Speed", Float) = 1
  }
  SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Lighting Off Fog { Mode Off }
    ColorMask RGB
    Blend SrcAlpha OneMinusSrcAlpha

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		fixed4 _Color;
        fixed4 _Color2;
        fixed4 _ColorMod;
        float _Speed;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
            float t = sin(_Time.y*_Speed);
			o.color = v.color*lerp(_Color, _Color2, t*t)*_ColorMod;
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR {
			return i.color;
		}

		ENDCG
	}
  }
}
