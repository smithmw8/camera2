// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Custom/lsdbuzz" {
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
           
           	float nSin(float x) {
				return sin(x)*0.5+0.5;
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
				float2 u = i.uv;
				float t = _Time.w;
				u += vec2(sin(t*0.4), sin(t*0.6))*0.03;

				u.x = (u.x - 0.5)*(1.0+sin(t*0.4)*0.15) + 0.5;
				u.y = (u.y - 0.5)*(1.0+sin(t*0.1)*0.15) + 0.5;

				u.x += sin(u.y*3.0+t)*0.02;
				u.y += cos(u.x*4.5+t)*0.02;

				float w = sin(t*0.5)*0.2;

				u -= float2(0.5,0.5);
                float dist = length(u);
                float radius = 0.5f * sin(t * 0.5);
                
                float2 tc = i.uv;
                if (dist < radius)
                {
                	tc -= float2(0.5,0.5);
                    float percent = 1.0 - ((radius - dist) / radius) * 0.8;
                    percent = percent * percent;
                    
                    tc = tc * percent;
                    tc += float2(0.5,0.5);
                }
                u = tc;

				float vibration = nSin(t*0.8)*nSin(t*0.2);
				vec2 vibU = u;
				vibU.x += vibration*sin(t*99.0)*vibration*0.03;
				vibU.y += vibration*sin(t*24.0)*vibration*0.03;

				vec4 c = texture2D(_MainTex, u);

				float r1 = 0.5*sin(t*4.0)+0.6;
				float g1 = 0.1*sin(t*2.0)+0.8;
				float b1 = 0.2*sin(_Time.w*3.51)+0.2;
				float t1 = 0.5*sin(_Time.w)+1.0;
				float rc1 = 0.5*sin(_Time.w*4.0)+0.6;
				float gc1 = 0.1*sin(_Time.w*2.0)+0.8;
				float bc1 = 0.2*sin(_Time.w*3.51)+0.2;
				if (r1*c.r + g1*c.g + b1*c.b > t1) {
					c.r *= rc1;
				    c.g *= gc1;
				    c.b *= bc1;
				}

				float r2 = 0.3*sin(t*4.0)-0.3;
				float g2 = 0.2*sin(t*2.0)+0.8;
				float b2 = 0.1*sin(t*3.51)+0.2;
				float t2 = 0.3*sin(t*0.9)+0.9;
				float rc2 = 0.1*sin(_Time.w*2.0)+0.6;
				float gc2 = 0.2*sin(_Time.w*1.0)+0.8;
				float bc2 = 0.4*sin(_Time.w*0.51)+0.2;
				if (r2*c.r + g2*c.g + b2*c.b < t1) {
					c.r *= rc2;
				    c.g *= gc2;
				    c.b *= bc2;
				}

				vec4 vibSamp = texture2D(_MainTex, vibU);
				c += vibSamp*vibration;
                return c;
            }
              
            ENDCG
        }
    }
    FallBack "Diffuse"
}
