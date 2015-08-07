Shader "Custom/Vertex Color/Unlit VC Breezy" {
	Properties {
		_WindPower("Wind Power", Float) = 0.02
		_WindSpeed("Wind Speed", Float) = 64.0
		_WindDirectionX("Wind Direction X", Float) = 1.0
		_WindDirectionZ("Wind Direction Z", Float) = 1.0
		_SwayVariation("Sway Variation", Range(0, 1)) = 0.05
		_Turbulence("Turbulence", Range(0, 1)) = 0.1
		_TurbulenceFrequency("Turbulence Frequency", Float) = 1.0
		_DeformVariation("Deform Variation", Range(0, 1)) = 0.3
		_PlantHeight("Plant Height", Float) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float random(float3 p) {
				float dt = dot(p, float3(12.9898, 78.233, 45.5432));
				float phase = dt % 3.14159;
				return frac(43758.5453 * sin(phase));
			}
			
			half noise(float t) {
				half x0 = floor(t);
				half x1 = x0 + 1.0;
				half v0 = frac(31718.927 * sin(0.014686 * x0) + x0);
				half v1 = frac(31718.927 * sin(0.014686 * x1) + x1);
				return 2 * (v0 * (1 - frac(t)) + v1 * frac(t)) - 1 * sin(t);
			}
			
			CBUFFER_START(WindVariables)
				float _WindPower;
				float _WindSpeed;
			CBUFFER_END
			
			CBUFFER_START(WindVector)
				float _WindDirectionX;
				float _WindDirectionZ;
			CBUFFER_END
			
			float _SwayVariation;
			float _Turbulence;
			float _TurbulenceFrequency;
			
			float _DeformVariation;
			float _PlantHeight;
			
			struct VertexInput {
				float4 position : POSITION;
				float4 color : COLOR;
			};
			
			struct FragmentInput {
				float4 position : SV_POSITION;
				float4 color : COLOR;
			};
			
			FragmentInput vert(VertexInput i) {
				// Time is offset by a random amount, seeded by the object position 
				// on the xz-plane (taken out of the model matrix's translation column)
				// so that different objects don't sway in perfect unison
				// given the same _Time value.
				float3 object_position = float3(_Object2World[0][3], _Object2World[1][3], _Object2World[2][3]);
				float sway = _SwayVariation * random(object_position);
				
				float turbulence = _Turbulence * noise(_TurbulenceFrequency * _Time);
				float time = _Time + sway + turbulence;
				
				float wind_strength = _WindPower * sin(time * _WindSpeed);
				float3 wind_direction = float3(_WindDirectionX, 0, _WindDirectionZ);
				float3 wind_offset = wind_strength * wind_direction;
				
				// The given wind direction is in world space, so the transformation
				// must be applied to the vertex in world space so all vertices and
				// objects appear to have the wind blowing from the same direction.
				float3 world_position = mul(_Object2World, i.position).xyz;
				
				// Vertices are deformed to make foliage look less solid/rigid
				// so leaves or blades sort-of bend in the wind.
				float deformation = _DeformVariation * random(world_position);
				float3 deformation_offset = wind_offset * deformation;
				
				// The wind's effect is based on vertex y-position in object space
				// such that at y=0 (where the plant's roots are) there is no
				// wind effect, and at y=_PlantHeight, there is maximum wind effect.
				float height_factor = i.position.y / _PlantHeight;
				
				// combine everything and offset the vertex's world position
				world_position += lerp(0, wind_offset + deformation_offset, height_factor);
				
				FragmentInput result;
				result.position = mul(UNITY_MATRIX_VP, float4(world_position, i.position.w));
				result.color = i.color;
				return result;
            }

            fixed4 frag(FragmentInput i) : SV_Target {
                return i.color;
            }
			ENDCG
		}
	}
	Fallback off
}
