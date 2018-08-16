// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Custom-Pinch" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader {
  //      Tags { "RenderType"="Opaque" }
          Tags { 
           "Queue"="Transparent" 
         "RenderType"="Transparent" 
         }

        LOD 200
          
        Pass
        {
        Blend SrcAlpha OneMinusSrcAlpha 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
              
            sampler2D _MainTex;
              
            float4 _MainTex_ST;
            float _isFlip;
             fixed4 _Color;

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
  
            v2f vert(appdata_full v)
            {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
            }
              
            float4 frag(v2f i) : COLOR
            {      
                float2 tc = i.uv;
                float radius = 0.5f;
                
				tc -= float2(0.5,0.5);
				float theta = atan2(tc.y,tc.x);
				radius = sqrt(tc.x * tc.x + tc.y * tc.y);
				float newR = sqrt(radius) * 0.5;

				tc.x = newR * cos(theta);
				tc.y = newR * sin(theta);

				tc += float2(0.5,0.5);
               
                float3 color = tex2D(_MainTex, tc).rgb;
                  return float4(color, 1.0)* _Color;
            }
              
            ENDCG
        }
    }
    FallBack "Diffuse"
}