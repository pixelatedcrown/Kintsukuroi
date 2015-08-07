Shader "Custom/Vertex Color/Lit VC with Texture" {
	Properties {
		_Color ("Color Tint (RGB)", COLOR) = (1, 1, 1, 1)
		_Strength ("Vertex Color Strength", Range(0, 1)) = 1
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EmissionColor ("Emission Color (RGB)", COLOR) = (1, 1, 1, 1)
		_EmissionTex ("Emission (A)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		sampler2D _EmissionTex;
		float _Strength;
		float3 _Color;
		float3 _EmissionColor;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EmissionTex;
			float3 vertColors;
		};
		
		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertColors = v.color.rgb;
			o.uv_MainTex = v.texcoord;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float3 tex = tex2D (_MainTex, IN.uv_MainTex);
			float3 c = IN.vertColors.rgb * _Strength;
			o.Albedo = tex.rgb * c.rgb * _Color;
			float4 emissionTex = tex2D (_EmissionTex, IN.uv_EmissionTex);
			o.Emission = IN.vertColors.rgb * emissionTex.a * _EmissionColor;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
