//	Copyright 2014 Unluck Software	
//	www.chemicalbliss.com
Shader "Custom/Vertex Color/Toon/Vertex Color Toon Transparent"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
      _UnlitColor ("Diffuse Color", Color) = (0.5,0.5,0.5,1) 
      _DiffuseThreshold ("Diffuse Threshold", Range(0,1)) = 0.1 
      _OutlineColor ("Outline Color", Color) = (0,0,0,1)
      _LitOutlineThickness ("Lit Outline Thickness", Range(0,1)) = 0.1
      _UnlitOutlineThickness ("Unlit Outline Thickness", Range(0,1)) 
         = 0.4
      _SpecColor ("Specular Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Float) = 10
      _LightTweakX ("_LightTweakX", Float) = 0
      _LightTweakY ("_LightTweakY", Float) = 0
      _LightTweakZ ("_LightTweakZ", Float) = 0
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off
		ZWrite Off
		Blend OneMinusDstColor One
		Lighting Off
		//Alphatest Greater 0
		Fog { Mode Off }
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		fixed4 _Color;
		
		 uniform fixed4 _LightColor0; 
		 uniform fixed _LightTweakX;
		 uniform fixed _LightTweakY; 
		 uniform fixed _LightTweakZ; 
         uniform fixed4 _UnlitColor;
         uniform fixed _DiffuseThreshold;
         uniform fixed4 _OutlineColor;
         uniform fixed _LitOutlineThickness;
         uniform fixed _UnlitOutlineThickness;
         uniform fixed4 _SpecColor; 
         uniform fixed _Shininess;
		
		
		struct appdata //in
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float3 normal : NORMAL;
		};
		
		struct v2f	//out
		{
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float3 lightDir : TEXCOORD0;
            float3 normalDir : TEXCOORD1;

		};
		
		v2f vert (appdata v)
		{
			v2f o;
		//	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			
			
		//	float4x4 modelMatrix = _Object2World;
            float4x4 modelMatrixInverse = _World2Object; 
 
          //  o.posWorld = mul(modelMatrix, v.vertex);
            o.normalDir = normalize(float3(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz));
            o.lightDir = normalize(
			float3(_WorldSpaceCameraPos.x+_LightTweakX,_WorldSpaceCameraPos.y+_LightTweakY,_WorldSpaceCameraPos.z+_LightTweakZ) 
			//- float3(i.posWorld.xyz)
			);
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.color = v.color;
			
			return o;
		}
		
		
		half4 frag(v2f i) : COLOR
		{
			float3 normalDirection = normalize(i.normalDir).xyz;
 			fixed3 vertexColor = fixed3(i.color.rgb);
 			float3 lightDirection= i.lightDir;
            float3 viewDirection = i.lightDir;
            
            float attenuation;
			attenuation = 1.0;
			
            fixed3 fragmentColor = fixed3(_UnlitColor.rgb); 
 

            if (attenuation 
               * max(0.0, dot(normalDirection, lightDirection)) 
               >= _DiffuseThreshold)
            {
               fragmentColor = 
               //float3(_LightColor0) * 
                fixed3(_Color.rgb)
                
                ; 
            }
 
            // outline
            if (dot(viewDirection, normalDirection) 
               < lerp(_UnlitOutlineThickness, _LitOutlineThickness, 
               max(0.0, dot(normalDirection, lightDirection))))
            {
               fragmentColor = 
                 // float3(_LightColor0) * 
                  fixed3(_OutlineColor.rgb); 
            }
 
            // highest priority: highlights
            if (dot(normalDirection, lightDirection) > 0.0 
               // light source on the right side?
               && attenuation *  pow(max(0.0, dot(
               reflect(-lightDirection, normalDirection), 
               viewDirection)), _Shininess) > 0.5) 
               // more than half highlight intensity? 
            {
               fragmentColor = _SpecColor.a 
                 // * float3(_LightColor0)
                 
                  * fixed3(_SpecColor.rgb)
                  
                  + (1.0 - _SpecColor.a) 
                  
                  * fragmentColor;
            }
 
            return float4(fragmentColor*vertexColor*2, 1.0);
		}
		
		
		
		ENDCG
		}
	}
}