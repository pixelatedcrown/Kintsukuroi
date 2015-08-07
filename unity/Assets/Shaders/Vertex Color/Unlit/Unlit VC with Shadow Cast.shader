Shader "Custom/Vertex Color/Unlit VC with Shadow Cast" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Cull Off
		
		CGPROGRAM
		#pragma surface surf Flat addshadow noambient novertexlights noforwardadd

		half4 LightingFlat(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
		
		fixed4 _Color;

		struct Input {
			float4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = _Color;
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = c.a * IN.color.a;
		}
		ENDCG
	}
	Fallback off
}