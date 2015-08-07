Shader "Custom/Vertex Color/Animated Texture/Unlit VC BackMoveWave" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	_Mod ("Mod", Color) = (1,1,1,1)
	
	_BackTex ("Texture Back", 2D) = "white" {}
	_BackMod ("Mod Back", Color) = (1,1,1,1)
	
	amplitudeX ("Amplitude X", Float) = 0
	amplitudeY ("Amplitude Y", Float) = 0
	
	speedX ("Speed X", Float) = 0
	speedY ("Speed Y", Float) = 0
	
	rangeX ("Range X", Float) = 0
	rangeY ("Range Y", Float) = 0
  }
  SubShader {
    Tags { "RenderType" = "Opaque" }
    Blend SrcAlpha OneMinusSrcAlpha
	Lighting Off Fog { Mode Off }
    ColorMask RGB
    
    Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _BackTex;
		fixed4 _BackMod;
		
		//strength of wave, usu. in fraction
		float amplitudeX;
		float amplitudeY;
		
		//degree per second
		float speedX; 
		float speedY; 
		
		//rev per pixel
		float rangeX;
		float rangeY;

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
			o.texcoord = float2(
			v.texcoord.x + sin(v.texcoord.y*rangeY + speedX*_Time.y)*amplitudeX,
			v.texcoord.y + sin(v.texcoord.x*rangeX + speedY*_Time.y)*amplitudeY);
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 col = tex2D(_BackTex, i.texcoord) * i.color * _BackMod;
			return col;
		}

		ENDCG
	}

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _Mod;

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
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 col = tex2D(_MainTex, i.texcoord) * i.color * _Mod;
			return col;
		}

		ENDCG
	}
  }
  Fallback "Diffuse"
}
