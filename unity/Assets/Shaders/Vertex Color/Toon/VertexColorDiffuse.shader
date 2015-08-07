    Shader "Custom/Vertex Color/Toon/Vertex Colored Diffuse" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
      
    }
     
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 150
     	Cull Off
    CGPROGRAM
    #pragma surface surf Lambert vertex:vert
     
  
    fixed4 _Color;
     
    struct Input {
       
        float3 vertColor;
    };
     
    void vert (inout appdata_full v, out Input o) {
        UNITY_INITIALIZE_OUTPUT(Input, o);
        o.vertColor = v.color;
    }
     
    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 c = _Color;
        o.Albedo = c.rgb * IN.vertColor;
        o.Alpha = c.a;
    }
    ENDCG
    }
     
    Fallback "Diffuse"
    }