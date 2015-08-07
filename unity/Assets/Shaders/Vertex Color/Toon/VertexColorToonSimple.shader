//	Copyright 2014 Unluck Software	
//	www.chemicalbliss.com
Shader "Custom/Vertex Color/Toon/Vertex Color Toon Simple"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		
      _UnlitColor ("Diffuse Color", Color) = (0.5,0.5,0.5,1) 
      _DiffuseThreshold ("Diffuse Threshold", Range(0,1)) = 0.1 
     
     
     
      
	}
	SubShader
	{
		Tags {"RenderType"="Opaque" "IgnoreProjector"="True"}
		Cull Off
		ZWrite On
		Lighting Off
		Fog { Mode Off }
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		fixed4 _Color;
		
		 uniform fixed4 _LightColor0; 
        
         uniform fixed4 _UnlitColor;
         uniform fixed _DiffuseThreshold;
        
        
        

        
		
		
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
			//float4 posWorld : TEXCOORD0;
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
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.color = v.color;
			
			return o;
		}
		
		
		half4 frag(v2f i) : COLOR
		{
			float3 normalDirection = normalize(i.normalDir).xyz;
 			fixed3 vertexColor = fixed3(i.color.rgb);
            float3 viewDirection = normalize(
			_WorldSpaceCameraPos 
			//- float3(i.posWorld.xyz)
			);
            float3 lightDirection;
            float attenuation;
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(float3(_WorldSpaceCameraPos.xyz));
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
 
           
 
            
 
            return float4(fragmentColor*vertexColor*2, 1.0);
		}
		
		
		
		ENDCG
		}
	}
	Fallback "Diffuse"
}