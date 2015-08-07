Shader "Custom/Vertex Color/Unlit VC with Obj Directional"
{
        Properties
        {
       		_Color ("Main Color", Color) = (1,1,1,1)
        	_FakeLightSource ("Fake Light Source", Vector) = (0, 0, 0, 0)
        }
        
        SubShader
        {
        	
        Tags { "RenderType"="Opaque" "IgnoreProjector"="True" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Cull Off
               
            CGPROGRAM #pragma surface surf Flat addshadow noambient novertexlights noforwardadd

			half4 LightingFlat(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
			
        	float4 _Color;
       		float4 _DimColor;
        	float _Intensity;
        	float4 _FakeLightSource;

        	struct Input
        	{
        		float4 color : COLOR;
                float3 worldNormal;
        	};

        	void surf (Input IN, inout SurfaceOutput o)
        	{
                float4 litColor = lerp(_DimColor, _Color, (dot(IN.worldNormal, _FakeLightSource.xyz) * 0.5)+0.5);
                o.Albedo = lerp(IN.color, litColor.rgb, _Intensity);
                fixed4 c = _Color;
                o.Alpha = c.a * IN.color.a;
        	}
        	ENDCG
        }
        
        FallBack off
}