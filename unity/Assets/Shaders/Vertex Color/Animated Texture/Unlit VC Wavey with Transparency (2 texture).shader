Shader "Custom/Vertex Color/Animated Texture/Unlit VC Wavey with Transparency (2 texture)" {
  Properties {
	_Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" {}
	_MainTex2 ("Texture2", 2D) = "white" {}
	
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
	
	speed2X ("Speed 2 X", Float) = 1
	speed2Y ("Speed 2 Y", Float) = 1
	
	amplitudeX ("amplitude X", Float) = 0.017453292
	amplitudeY ("amplitude Y", Float) = 0.017453292
	
	amplitude2X ("amplitude 2 X", Float) = 0.017453292
	amplitude2Y ("amplitude 2 Y", Float) = 0.017453292
	
	rangeX ("range X", Float) = 0.017453292
	rangeY ("range Y", Float) = 0.017453292
	
	range2X ("range 2 X", Float) = 0.017453292
	range2Y ("range 2 Y", Float) = 0.017453292
	
	colorMod1 ("ColorMod 1", Color) = (1,1,1,1)
	colorMod2 ("ColorMod 2", Color) = (1,1,1,1)
  }
  SubShader {
	Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}

    //ZWrite Off // on might hide behind pixels, off might miss order
    //Blend SrcAlpha OneMinusSrcAlpha
	Blend SrcAlpha OneMinusSrcAlpha
	//Blend SrcAlpha One
    ColorMask RGB
	Lighting Off Fog { Mode Off }

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		fixed4 _Color;
		
		sampler2D _MainTex;
		sampler2D _MainTex2;
		
		float speedX;
		float speedY;
		
		float speed2X;
		float speed2Y;
		
		float amplitudeX;
		float amplitudeY;
		
		float amplitude2X;
		float amplitude2Y;
		
		float rangeX;
		float rangeY;
		
		float range2X;
		float range2Y;
		
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
			
			o.texcoord = float2(
				v.texcoord.x + sin(v.texcoord.y*rangeY + speedX*_Time.y)*amplitudeX,
				v.texcoord.y + sin(v.texcoord.x*rangeX + speedY*_Time.y)*amplitudeY);
				
			o.texcoord2 = float2(
				v.texcoord.x + sin(v.texcoord.y*range2Y + speed2X*_Time.y)*amplitude2X,
				v.texcoord.y + sin(v.texcoord.x*range2X + speed2Y*_Time.y)*amplitude2Y);
							
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 clr = _Color * i.color;
			fixed4 clr1 = tex2D(_MainTex, i.texcoord) * colorMod1;
			fixed4 clr2 = tex2D(_MainTex2, i.texcoord2) * colorMod2;
			
			clr.rgb = lerp(clr.rgb, (clr1 * i.color).rgb, clr1.a);
			clr.rgb *= clr.a;
			
			clr.rgb = lerp(clr.rgb, (clr2 * i.color).rgb, clr2.a);
			clr.rgb *= clr.a;
			
			return clr;
		}

		ENDCG
	}
  }
  Fallback "Transparent/Diffuse"
}
