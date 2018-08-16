// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Hangout" {
	 Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
          
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
               
            #define vec2 float2
			#define vec3 float3
			#define vec4 float4
			#define mix lerp  
			#define texture2D tex2D  
            sampler2D _MainTex;
            
            float4 _MainTex_ST;
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            bool insideRectangle(in vec2 uv, in vec2 location, in vec2 size) {
			    return uv.x >= location.x && 
			        uv.x <= location.x + size.x && 
			        uv.y >= location.y && 
			        uv.y <= location.y + size.y ;
			}

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

			    vec2 start_incr = vec2(0.125,0.125);
			    vec2 size_incr = vec2(0.85,0.85);
			    vec4 fragColor;
			    vec2 location = vec2(0.0,0.0);
			    vec2 size = vec2(1.0,1.0);
			    for(int i = 0; i < 7; i++) {
			        if (insideRectangle(tc, location, size)) {
			            fragColor = texture2D(_MainTex, (tc - location)/size);
			        }
			        location += start_incr / size_incr * 0.5;
			        size += size_incr - vec2(1.0,1.0);
			    }

                return fragColor;
            }
              
            ENDCG
        }
    }
    FallBack "Diffuse"
}
