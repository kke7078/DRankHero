Shader "Custom/MinimapShader_BottomOnly"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _IsMinimapHide ("Is Minimap Hidden", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            fixed4 _Color;
            float _IsMinimapHide;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_IsMinimapHide > 0.5)
                {
                    // 미니맵에서 숨기기 (완전 투명)
                    return fixed4(_Color.rgb, 0);
                }

                // 아랫면만 보이도록
                if (i.normal.y > -0.1)
                    discard;

                return _Color;
            }
            ENDCG
        }
    }
}