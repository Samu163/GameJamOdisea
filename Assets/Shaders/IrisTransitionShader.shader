Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color (Black)", Color) = (0,0,0,1)
        // Este valor controla el tamaño del agujero (0 = cerrado, 1.5 = totalmente abierto)
        _HoleRadius ("Hole Radius", Range(0, 1.5)) = 1.5 
        // Suavizado del borde del círculo para que no se vea pixelado
        _Smoothness ("Edge Smoothness", Range(0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            float _HoleRadius;
            float _Smoothness;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata_t v) {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target {
                // Calculamos el centro de la imagen UV (normalmente 0.5, 0.5)
                float2 center = float2(0.5, 0.5);
                
                // Calculamos la distancia del píxel actual al centro.
                // Ajustamos por el aspecto para que el círculo no se vea ovalado en pantallas anchas.
                float2 uv = IN.texcoord - center;
                float aspect = _ScreenParams.x / _ScreenParams.y;
                uv.x *= aspect;
                float dist = length(uv);

                // Usamos smoothstep para crear el círculo transparente basado en la distancia y el radio
                // Si la distancia es menor que el radio, alpha será 0 (transparente).
                float alphaValue = smoothstep(_HoleRadius, _HoleRadius + _Smoothness, dist);
                
                // El color final es negro, con la transparencia calculada
                return fixed4(0, 0, 0, alphaValue);
            }
            ENDCG
        }
    }
}
