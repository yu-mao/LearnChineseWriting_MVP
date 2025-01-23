Shader "Custom/TransparentBrush"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0.5)
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
                Comp Always
                Pass Replace
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
    Fallback "Transparent/Diffuse"
}
