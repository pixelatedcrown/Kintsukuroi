
Shader "Custom/Vertex Color/Oberonscourt/Landscape Shadows" 
{
	    Properties 
	{
		        
		_Normal_influence("Influence of the normals", Float) = 1
		_base_height("height of gradient", Float) = 0.2
		_height_blur("Height blurring value", Float) = 0.7
		_top_color("_top_color", Color) = (1,1,1,1)
		_bottom_color("_bottom_color", Color) = (0,0,0,1)
		_RimPower("how much rim", Range(-1,8) ) = 1.02
		_top_rim_offset("Top rim height offset ", Float) = 0
		_top_rim_blur("Top rim blurring", Float) = 0
		_Rim_Color("Top rim Color", Color) = (1,1,1,1)
		_Rimpower("_Rimpower", Float) = 1
		    }
	 
	    SubShader 
	{
		        Tags 
		{"Queue" = "Geometry" "RenderType" = "Opaque" "LightMode" = "Always" "LightMode" = "ForwardBase""IgnoreProjector"="True"
			}
		     Cull Back
		Lighting Off
		ZWrite On
		ZTest LEqual
		CGPROGRAM
		#pragma surface surf BlinnPhongEditor  exclude_path:prepass noambient nolightmap noforwardadd approxview vertex:vert
		#pragma target 2.0
		fixed _base_height;
		fixed _height_blur;
		fixed4 _top_color;
		fixed4 _bottom_color;
		fixed _RimPower;
		fixed _top_rim_offset;
		fixed _top_rim_blur;
		fixed4 _Rim_Color;
		fixed _Rimpower;
		struct EditorSurfaceOutput 
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed3 Gloss;
			fixed Specular;
			fixed Alpha;
			fixed4 Custom;
			};
		inline fixed4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, fixed4 light)
		{
			fixed3 spec = light.a * s.Gloss;
			fixed4 c;
			c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
			c.a = s.Alpha;
			return c;
			}
		inline fixed4 LightingBlinnPhongEditor (EditorSurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			fixed3 h = normalize (lightDir + viewDir);
			fixed diff = max (0, dot ( lightDir, s.Normal ));
			fixed nh = max (0, dot (s.Normal, h));
			fixed spec = pow (nh, s.Specular*128.0);
			fixed4 res;
			res.rgb = _LightColor0.rgb * diff;
			res.w = spec * Luminance (_LightColor0.rgb);
			res *= atten * 2.0;
			return LightingBlinnPhongEditor_PrePass( s, res );
			}
		struct Input 
		{
			fixed4 fullMeshUV1;
			fixed3 viewDir;
			fixed4 color : COLOR;
			};
		void vert (inout appdata_full v, out Input o) 
		{
		UNITY_INITIALIZE_OUTPUT(Input,o);
			fixed4 VertexOutputMaster0_0_NoInput = fixed4(0,0,0,0);
			fixed4 VertexOutputMaster0_1_NoInput = fixed4(0,0,0,0);
			fixed4 VertexOutputMaster0_2_NoInput = fixed4(0,0,0,0);
			fixed4 VertexOutputMaster0_3_NoInput = fixed4(0,0,0,0);
			o.fullMeshUV1 = v.texcoord;
			}
		void surf (Input IN, inout EditorSurfaceOutput o) 
		{
			o.Normal = fixed3(0.0,0.0,1.0);
			o.Alpha = 1.0;
			o.Albedo = 0.0;
			o.Emission = 0.0;
			o.Gloss = 0.0;
			o.Specular = 0.0;
			o.Custom = 0.0;
			fixed4 Split0=(IN.fullMeshUV1);
			fixed4 Multiply3=fixed4( Split0.y, Split0.y, Split0.y, Split0.y) * _height_blur.xxxx;
			fixed4 Add1=Multiply3 + _base_height.xxxx;
			fixed4 Clamp0=clamp(Add1,fixed4( 0.0, 0.0, 0.0, 0.0 ),fixed4( 1.0, 1.0, 1.0, 1.0 ));
			fixed4 Lerp0=lerp(_bottom_color,_top_color,Clamp0);
			fixed4 Multiply0=fixed4( Split0.y, Split0.y, Split0.y, Split0.y) * _top_rim_blur.xxxx;
			fixed4 Add2=Multiply0 + _top_rim_offset.xxxx;
			fixed4 Fresnel0_1_NoInput = fixed4(0,0,1,1);
			fixed4 Fresnel0=(1.0 - dot( normalize( fixed4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
			fixed4 Multiply1=Add2 * Fresnel0;
			fixed4 Clamp1=clamp(Multiply1,fixed4( 0.0, 0.0, 0.0, 0.0 ),fixed4( 1.0, 1.0, 1.0, 1.0 ));
			fixed4 Lerp1=lerp(Lerp0,_Rim_Color,Clamp1);
			fixed4 Multiply2=Lerp1 * IN.color;
			o.Emission = Multiply2;
			o.Normal = normalize(o.Normal);
			}
		ENDCG
		        Pass 
		{
			Blend DstColor Zero 
			Fog
			{ Mode Linear Color (1.0,1.0,1.0,1.0) Range 60.0,80.0}
			            CGPROGRAM
			                #pragma vertex vert
			                #pragma fragment frag
			                #pragma multi_compile_fwdbase
			                #pragma fragmentoption ARB_precision_hint_fastest
			                
			                #include "UnityCG.cginc"
			                #include "AutoLight.cginc"
			 
			                
			                
			 
			                
			          
			                struct appdata 
			{
				                    fixed4 vertex   :   POSITION;
				                    fixed4 color    :   COLOR;
				                };
			 
			                struct v2f
			                
			{
				                    fixed4  pos     :   SV_POSITION;
				                    fixed4  color   :   TEXCOORD0;
				                    LIGHTING_COORDS(1, 2) 
				                }; 
			 
			                v2f vert (appdata v)
			                
			{
				                    v2f o;
				                    o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
				                    o.color = 1;
				                    TRANSFER_VERTEX_TO_FRAGMENT(o) 
				                    return o;
				                }
			 
			                fixed4 frag(v2f i) : COLOR
			                
			{
				                    
				                    fixed atten = LIGHT_ATTENUATION(i); 
				                    
				                    
				                    
				                    fixed4 c = i.color;
				                    c.rgb *= atten;
				                    return c;
				                }
			            ENDCG
			        }
		    }
	    FallBack "VertexLit"
	}

