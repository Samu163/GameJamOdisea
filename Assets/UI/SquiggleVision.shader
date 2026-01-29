Shader "Custom/SquiggleVision"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Squiggle Settings)]
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Scale ("Noise Scale", Vector) = (1,1,0,0)
        _Strength ("Strength", Float) = 1.0
        _FPS ("FPS", Float) = 6.0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True" 
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float2 noise_uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            
            sampler2D _NoiseTex;
            float4 _NoiseTex_TexelSize;
            float2 _Scale;
            float _Strength;
            float _FPS;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord;
                OUT.color = IN.color * _Color;

                float3 worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
                float3 objectPos = float3(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3]);

                float2 vertex_offset = worldPos.xy - objectPos.xy;
                float2 textureSize = float2(_NoiseTex_TexelSize.z, _NoiseTex_TexelSize.w);
                float2 safeScale = max(_Scale, 0.001); 
                
                OUT.noise_uv = vertex_offset / (textureSize * safeScale);

                return OUT;
            }

            static const float2 offset_multiplier = float2(3.14159, 2.71828);

            fixed4 frag(v2f IN) : SV_Target
            {
                float time_stepped = floor(_Time.y * _FPS);
                float2 noise_offset = time_stepped * offset_multiplier;

                float noise_sample = tex2D(_NoiseTex, IN.noise_uv + noise_offset).r * 4.0 * 3.14159;
                float2 direction = float2(cos(noise_sample), sin(noise_sample));
                float2 squiggle_uv = IN.uv + direction * _Strength * 0.005;

                fixed4 c = tex2D(_MainTex, squiggle_uv) * IN.color;
                c.rgb *= c.a;
                
                return c;
            }
            ENDCG
        }
    }
}