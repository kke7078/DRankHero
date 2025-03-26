Shader "Custom/MinimapShader_BottomOnly"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" }
        Cull Off // 양면 렌더링
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

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // 오브젝트를 화면 좌표로 변환
                o.normal = UnityObjectToWorldNormal(v.normal); // 월드 좌표에서 노멀 벡터
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 벽의 아랫면만 보이도록
                if (i.normal.y > -0.1) discard;  // y축 방향으로 위쪽 면을 걸러내고 아랫면만 렌더링
                return _Color;
            }
            ENDCG
        }
    }
}