Shader "Custom/Vertex Color/Oberonscourt/Cloud Sine Sorted"
{
	Properties 
	{
_Color("_Color", Color) = (1,0,0,1)
_FrequencyX("_FrequencyX", Range(0,10) ) = 1
_FrequencyZ("_FrequencyZ", Range(0,10) ) = 1
_ScaleX("_ScaleX", Range(0,10) ) = 0.5
_ScaleZ("_ScaleZ", Range(0,10) ) = 1
_Speed("_Speed", Range(0,1) ) = 0.5

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"IgnoreProjector"="False"
"RenderType"="Transparent"

		}

		
Cull Off
ZWrite Off
ZTest LEqual
ColorMask RGBA
Blend SrcAlpha OneMinusSrcAlpha
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  exclude_path:prepass vertex:vert
#pragma target 2.0


float4 _Color;
float _FrequencyX;
float _FrequencyZ;
float _ScaleX;
float _ScaleZ;
float _Speed;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float4 color : COLOR;

			};

			void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
float4 Multiply2=_Time * _Speed.xxxx;
float4 Add3=Multiply2 + v.texcoord;
float4 Splat1=Add3.y;
float4 Multiply4=_FrequencyX.xxxx * Splat1;
float4 Sin1=sin(Multiply4);
float4 Multiply0=_ScaleX.xxxx * Sin1;
float4 Multiply1=_FrequencyZ.xxxx * Splat1;
float4 Sin0=sin(Multiply1);
float4 Multiply3=_ScaleZ.xxxx * Sin0;
float4 Assemble0=float4(Multiply0.x, float4( 0.0, 0.0, 0.0, 0.0 ).y, Multiply3.z, float4( 0.0, 0.0, 0.0, 0.0 ).w);
float4 Add2=Assemble0 + v.vertex;
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
v.vertex = Add2;


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Emission = _Color;
o.Alpha = IN.color;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}