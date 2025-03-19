Shader "CasualGame/BcManyknives/SpriteMesh-FillAndDissolve"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FillColor ("FillColor", Color) = (1,1,1,1)
        _FillPhase ("FillPhase", Range(0, 1)) = 0
        _NoiseMap("Noise Map", 2D) = "white" {}
        _DissolveThreshold("Dissolve Threshold", Range(0.0, 1.0)) = 0.0
        _LineWidth("Dissolve Line Width", Range(0.0, 0.5)) = 0.1
        _DissolveColor("Dissolve First Color", Color) = (0, 0, 1, 1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
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
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

            float4 _FillColor;
            float _FillPhase;
            sampler2D  _NoiseMap;
            float _DissolveThreshold;
            float _LineWidth;
            float4 _DissolveColor;

            #ifdef UNITY_INSTANCING_ENABLED

                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                // SpriteRenderer.Color while Non-Batched/Instanced.
                UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                // this could be smaller but that's how bit each entry is regardless of type
                UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

                #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

            #endif // instancing

            CBUFFER_START(UnityPerDrawSprite)
                #ifndef UNITY_INSTANCING_ENABLED
                    fixed2 _Flip;
                #endif
            CBUFFER_END

            // Material Color.
            fixed4 _Color;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;

                #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);

                return color;
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                //采样noise,与阈值比较进行clip
                half noise = tex2D(_NoiseMap, IN.texcoord).r;
                clip(noise - _DissolveThreshold);
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                c.rgb *= c.a;
                fixed3  finalColor = lerp(c.rgb , (_FillColor.rgb * c.a), _FillPhase); 
                //_LineWidth控制消融颜色作用的范围。与光照颜色进行lerp
                fixed t = 1 - smoothstep(0.0, _LineWidth, noise - _DissolveThreshold);
                finalColor = lerp(finalColor.rgb, _DissolveColor, t * step(0.001, _DissolveThreshold));
                return  fixed4(finalColor, c.a);
            }
            ENDCG
        }
    }
}
