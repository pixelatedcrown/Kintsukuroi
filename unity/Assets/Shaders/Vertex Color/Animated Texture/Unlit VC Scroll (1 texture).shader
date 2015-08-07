Shader "Custom/Vertex Color/Animated Texture/Unlit VC Scroll (1 texture)" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
  }
  SubShader {
	Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}

    //ZWrite Off // on might hide behind pixels, off might miss order
    //Blend SrcAlpha OneMinusSrcAlpha
	Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB
	Lighting Off Fog { Mode Off }

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float speedX;
		float speedY;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			o.texcoord = v.texcoord;
			o.texcoord.x += speedX * _Time.y;
			o.texcoord.y += speedY * _Time.y;
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
			return col;
		}

		ENDCG
	}
  }
  Fallback "Transparent/Diffuse"
}
