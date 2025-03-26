Shader "Custom/MinimapShader_BottomOnly"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" }
        Cull Off // ��� ������
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
                o.pos = UnityObjectToClipPos(v.vertex); // ������Ʈ�� ȭ�� ��ǥ�� ��ȯ
                o.normal = UnityObjectToWorldNormal(v.normal); // ���� ��ǥ���� ��� ����
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ���� �Ʒ��鸸 ���̵���
                if (i.normal.y > -0.1) discard;  // y�� �������� ���� ���� �ɷ����� �Ʒ��鸸 ������
                return _Color;
            }
            ENDCG
        }
    }
}