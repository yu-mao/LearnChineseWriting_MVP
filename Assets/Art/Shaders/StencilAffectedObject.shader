Shader "Custom/StencilAffectedObject"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _StencilRef ("Stencil Ref", Int) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            Stencil
            {
                Ref [_StencilRef]
                Comp Equal
                Pass Keep
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Transparency;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);

                color.a *= _Transparency;

                return lerp(color, float4(0, 0, 0, 0), 1 - color.a);
            }
            ENDCG
        }
    }
    Fallback "Transparent/Diffuse"
}
