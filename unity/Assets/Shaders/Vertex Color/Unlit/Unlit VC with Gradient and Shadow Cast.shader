Shader "Custom/Vertex Color/Unlit VC with Gradient and Shadow Cast" {
	  Properties {
	   	_MaxColor ("Top Color", Color) = (0, 0, 0, 0)
        _Color ("Bottom Color", Color) = (1, 1, 1, 1)
        _MinDistance ("Min Distance", Float) = 100
        _MaxDistance ("Max Distance", Float) = 1000
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
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
 
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float3 screenPos;
             float4 color : COLOR;
        };
 
        float _MaxDistance;
        float _MinDistance;
        half4 _Color;
        half4 _MaxColor;
 
        void surf (Input IN, inout SurfaceOutput o) {
           
            float dist = IN.worldPos.y;
            half weight = saturate( (dist - _MinDistance) / (_MaxDistance - _MinDistance) );
            half4 distanceColor = lerp(_Color, _MaxColor, weight);
 
            o.Albedo = IN.color.rgb * distanceColor.rgb ;
           
            o.Alpha = 1;
        }
        ENDCG
    }
   Fallback off
}