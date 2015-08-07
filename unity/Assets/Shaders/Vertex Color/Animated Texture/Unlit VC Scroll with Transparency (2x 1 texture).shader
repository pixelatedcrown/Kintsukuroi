Shader "Custom/Vertex Color/Animated Texture/Unlit VC Scroll with Transparency (2x 1 texture)" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
	
	speed2X ("Speed 2 X", Float) = 1
	speed2Y ("Speed 2 Y", Float) = 1
	
	colorMod1 ("Color", Color) = (1,1,1,1)
	colorMod2 ("Color 2", Color) = (1,1,1,1)
  }
  SubShader {
	Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}

    //ZWrite Off // on might hide behind pixels, off might miss order
    //Blend SrcAlpha OneMinusSrcAlpha
	//Blend SrcAlpha OneMinusSrcAlpha
	Blend One One
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
		
		float speed2X;
		float speed2Y;
		
		fixed4 colorMod1;
		fixed4 colorMod2;

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
			float2 texcoord2 : TEXCOORD1;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			
			o.texcoord = v.texcoord;
			o.texcoord.x += speedX * _Time.y;
			o.texcoord.y += speedY * _Time.y;
			
			o.texcoord2 = v.texcoord;
			o.texcoord2.x += speed2X * _Time.y;
			o.texcoord2.y += speed2Y * _Time.y;
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 col1 = tex2D(_MainTex, i.texcoord) * colorMod1;
			fixed4 col2 = tex2D(_MainTex, i.texcoord2) * colorMod2;
			
			if(col1.a == 0) {
				return col2 * i.color;
			}
			else if(col2.a == 0) {
				return col1 * i.color;
			}
			else {
				fixed4 col = fixed4(col1.r + col2.r*col2.a, col1.g + col2.g*col2.a, col1.b + col2.b*col2.a, col1.a*col2.a);
				return col * i.color;
			}
		}

		ENDCG
	}
  }
  Fallback "Transparent/Diffuse"
}
