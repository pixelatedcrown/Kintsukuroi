Shader "Custom/Vertex Color/Oberonscourt/Landscape"
{
	Properties 
	{
_base_height("height of gradient", Float) = 0.2
_height_blur("Height blurring value", Float) = 0.7
_top_color("_top_color", Color) = (1,1,1,1)
_bottom_color("_bottom_color", Color) = (0,0,0,1)
_RimPower("how much rim", Range(-1,8) ) = 1.02
_top_rim_offset("Top rim height offset ", Float) = 0
_top_rim_blur("Top rim blurring", Float) = 0
_Rim_Color("Top rim Color", Color) = (1,1,1,1)
_Vertexpower("_Vertexpower", Float) = 1

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="True"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGB
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  exclude_path:prepass noambient nolightmap noforwardadd vertex:vert
#pragma target 2.0


float _base_height;
float _height_blur;
float4 _top_color;
float4 _bottom_color;
float _RimPower;
float _top_rim_offset;
float _top_rim_blur;
float4 _Rim_Color;
float _Vertexpower;

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
				float4 fullMeshUV1;
float3 viewDir;
float4 color : COLOR;

			};

			void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);

o.fullMeshUV1 = v.texcoord;

			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Split0=(IN.fullMeshUV1);
float4 Multiply3=float4( Split0.y, Split0.y, Split0.y, Split0.y) * _height_blur.xxxx;
float4 Add1=Multiply3 + _base_height.xxxx;
float4 Clamp0=clamp(Add1,float4( 0.0, 0.0, 0.0, 0.0 ),float4( 1.0, 1.0, 1.0, 1.0 ));
float4 Lerp0=lerp(_bottom_color,_top_color,Clamp0);
float4 Multiply0=float4( Split0.y, Split0.y, Split0.y, Split0.y) * _top_rim_blur.xxxx;
float4 Add2=Multiply0 + _top_rim_offset.xxxx;
float4 Fresnel0_1_NoInput = float4(0,0,1,1);
float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
float4 Multiply1=Add2 * Fresnel0;
float4 Clamp1=clamp(Multiply1,float4( 0.0, 0.0, 0.0, 0.0 ),float4( 1.0, 1.0, 1.0, 1.0 ));
float4 Lerp1=lerp(Lerp0,_Rim_Color,Clamp1);
float4 Multiply2=Lerp1 * IN.color;
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Emission = Multiply2;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}