Shader "Custom/Vertex Color/Unlit VC with Set Directional and Shadow Cast"
{
        Properties
        {
                _LightColor ("Light Color", Color) = (1, 1, 1, 1)
                _DimColor ("Dim Color", Color) = (0, 0, 0, 1)
                _ColorDir ("LightDirection", Vector) = (0, 0, 0, 0)
                _Lerp ("Lerp Value", Range(0, 1)) = 0.5
        }
        SubShader
        {
            Tags { "RenderType"="Opaque" }
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
 
                float4 _LightColor;
                float4 _DimColor;
                float4 _ColorDir;
                float _Lerp;
 
                struct Input
                {
                        float3 worldNormal;
                        float3 color:COLOR;
                };
 
                void surf (Input IN, inout SurfaceOutput o)
                {
                        float4 litColor = lerp(_DimColor, _LightColor, (dot(IN.worldNormal, _ColorDir.xyz) * 0.5)+0.5);
                        o.Albedo = lerp(IN.color, litColor.rgb, _Lerp);
                        o.Alpha = 1;
                }
                ENDCG
        }
        Fallback off
}