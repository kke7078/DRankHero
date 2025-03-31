Shader "Projector/ProjectorDecal" {
    Properties {
        _ShadowTex ("Cookie", 2D) = "white" {}  // 기본값을 흰색으로 설정
        _FalloffTex ("FallOff", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Threshold ("White Threshold", Range(0, 1)) = 0.9  // 흰색을 투명하게 할 기준값
    }
    Subshader {
        Tags {"Queue"="Transparent"}
        Pass {
            ZWrite Off
            ZTest LEqual
            Cull Back
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha  // 투명도 블렌딩
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            
            struct v2f {
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
            };
            
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
            
            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uvShadow = mul (unity_Projector, vertex);
                o.uvFalloff = mul (unity_ProjectorClip, vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            
            sampler2D _ShadowTex;
            sampler2D _FalloffTex;
            fixed4 _Color;
            float _Threshold;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
                fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));

                // 흰색을 감지해서 투명화 처리
                float luminance = dot(texS.rgb, float3(0.299, 0.587, 0.114)); // 밝기 계산
                if (luminance > _Threshold) {
                    texS.a = 0; // 흰색이면 투명하게 만들기
                } else {
                    texS.rgb *= _Color.rgb;
                    texS.a = texF.a; // 페이드 효과 유지
                }

                UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(1,1,1,1));
                return texS;
            }
            ENDCG
        }
    }
}