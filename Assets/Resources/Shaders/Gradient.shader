Shader "Common/Gradient"
{
    Properties
    {
        _Color1 ("Top Color", Color) = (1,0,0,1)
        _Color2 ("Bottom Color", Color) = (0,0,1,1)
        _Direction ("Direction", Integer) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color1;
            fixed4 _Color2;
            int _Direction;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_Direction == 0)
                    return lerp(_Color2, _Color1, i.uv.y);
                else
                    return lerp(_Color1, _Color2, i.uv.y);
            }
            ENDCG
        }
    }
}
