Shader "Custom/Vertex Color/Unlit VC with Alpha Controlled Breeze" {
        Properties {
        _Color ("Color", Color) = (1.0,1.0,1.0,1.0)
        _WindSpeed("Wind Speed", Float) = 30
        _WindScale("Wind Scale", Range(0, 1)) = 0.1
        _WindRoughness("Wind Roughness", Float) = 30
        }
        SubShader {
                Tags { "RenderType"="Opaque" }
                Lighting Off
       
                Pass
                {
                        CGPROGRAM
                        #include "UnityCG.cginc"
                        #pragma vertex vert
                        #pragma fragment frag
 
                        // User defined variables
                        uniform fixed4 _Color;
                        uniform half _WindSpeed;
                        uniform fixed _WindScale;
                        uniform half _WindRoughness;
                       
                        // Base input structs
                        struct vertexOutput
                        {
                                fixed4 color : COLOR;                           // Vertex color
                                fixed4 pos : SV_POSITION;
                        };
           
                        // Vertex function
                        vertexOutput vert(appdata_full v)
                        {
                                vertexOutput o;
                                fixed3 worldPos = mul (_Object2World, v.vertex).xyz;
                               
                                // Vertex displacement. Influence based on vertex alpha
                                half2 wind = (0,0);
                                wind.x = sin(worldPos.x * _WindRoughness + _Time * _WindSpeed) * _WindScale;
                                wind.y = cos(worldPos.z * _WindRoughness + _Time * _WindSpeed) * _WindScale;
                                v.vertex.x += wind.x * v.color.a;
                                v.vertex.z += wind.y * v.color.a;
                               
                                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                                o.color = v.color;      // Vertex color
                                return o;
                        }
                       
                        // Fragment (pixel) function
                        fixed4 frag(vertexOutput i) : COLOR
                        {
                                // Return vertex color only (ignores alpha)
                                return fixed4(_Color.rgb * i.color.rgb, 1);
                        }
                       
                        ENDCG
                }
        }
       
        // Fallback
        Fallback Off
}