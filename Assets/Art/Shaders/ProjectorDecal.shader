Shader "Projector/ProjectorDecal" {
    Properties {
        _ShadowTex ("Cookie", 2D) = "white" {}
        _FalloffTex ("FallOff", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Threshold ("White Threshold", Range(0, 1)) = 0.9
        _LightDir ("Fake Light Direction", Vector) = (0.3, 0.7, 0.5, 0)
        _AOIntensity ("Edge Darkening", Range(0, 1)) = 0.3
    }

    Subshader {
        Tags {"Queue"="Transparent"}
        Pass {
            ZWrite Off
            ZTest LEqual
            Cull Back
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct v2f {
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                float4 pos : SV_POSITION;
            };

            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;

            v2f vert (float4 vertex : POSITION) {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, vertex);
                o.worldPos = world.xyz;
                o.pos = UnityObjectToClipPos(vertex);
                o.uvShadow = mul(unity_Projector, vertex);
                o.uvFalloff = mul(unity_ProjectorClip, vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            sampler2D _ShadowTex;
            sampler2D _FalloffTex;
            float4 _Color;
            float _Threshold;
            float4 _LightDir;
            float _AOIntensity;

            fixed4 frag (v2f i) : SV_Target {
                fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
                fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));

                float luminance = dot(texS.rgb, float3(0.299, 0.587, 0.114));
                if (luminance > _Threshold) {
                    texS.a = 0;
                } else {
                    // 🔥 실제 표면의 월드 노멀 추정
                    float3 dx = ddx(i.worldPos);
                    float3 dy = ddy(i.worldPos);
                    float3 worldNormal = normalize(cross(dx, dy));

                    // 가짜 조명 적용 (월드 노멀 기반)
                    float3 lightDir = normalize(_LightDir.xyz);
                    float NdotL = saturate(dot(worldNormal, lightDir));
                    float lighting = lerp(0.7, 1.2, NdotL); // 밝기 조정

                    texS.rgb *= _Color.rgb * lighting;
                    texS.a = texF.a;

                    // 가장자리 어둡게
                    float ao = 1.0 - texF.a;
                    texS.rgb *= 1.0 - (ao * _AOIntensity);
                }

                UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(1,1,1,1));
                return texS;
            }
            ENDCG
        }
    }
}