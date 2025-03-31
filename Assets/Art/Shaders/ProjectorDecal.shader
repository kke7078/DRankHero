Shader "Projector/ProjectorDecal" {
    Properties {
        _ShadowTex ("Cookie", 2D) = "white" {}  // �⺻���� ������� ����
        _FalloffTex ("FallOff", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Threshold ("White Threshold", Range(0, 1)) = 0.9  // ����� �����ϰ� �� ���ذ�
    }
    Subshader {
        Tags {"Queue"="Transparent"}
        Pass {
            ZWrite Off
            ZTest LEqual
            Cull Back
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha  // ���� ����
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

                // ����� �����ؼ� ����ȭ ó��
                float luminance = dot(texS.rgb, float3(0.299, 0.587, 0.114)); // ��� ���
                if (luminance > _Threshold) {
                    texS.a = 0; // ����̸� �����ϰ� �����
                } else {
                    texS.rgb *= _Color.rgb;
                    texS.a = texF.a; // ���̵� ȿ�� ����
                }

                UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(1,1,1,1));
                return texS;
            }
            ENDCG
        }
    }
}