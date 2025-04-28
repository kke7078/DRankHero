Shader "Custom/WorldSpaceBrick_SmoothDarken"
{
    Properties
    {
        _MainTex ("Brick Texture", 2D) = "white" {}
        _Tiling ("Tiling", Float) = 1
        _HeightStart ("Start Height", Float) = 5
        _HeightEnd ("End Height", Float) = -10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _Tiling;
            float _HeightStart;
            float _HeightEnd;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = world.xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 월드 좌표 기반 UV
                float2 uv = i.worldPos.xy * _Tiling;
                fixed4 texColor = tex2D(_MainTex, uv);

                // 부드러운 그라데이션 적용
                float t = smoothstep(_HeightStart, _HeightEnd, i.worldPos.y);
                texColor.rgb = lerp(texColor.rgb, float3(0, 0, 0), t);

                return texColor;
            }
            ENDCG
        }
    }
}