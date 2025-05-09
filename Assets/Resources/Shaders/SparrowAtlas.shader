Shader "Custom/SparrowAtlas"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _SubTexture ("SubTexture (X, Y, Width, Height)", Vector) = (0, 0, -1, -1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "PreviewType"="Sprite"
            "CanUseSpriteAtlas"="True"
        }

        LOD 100
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            float4 _SubTexture;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Apply side trims
                float2 uv = v.uv;
                int4 subTex = int4(_SubTexture);
                
                float x = subTex.x;
                float y = subTex.y;
                float width = subTex.z;
                float height = subTex.w;

                if (width < 0)
                {
                    width = _MainTex_TexelSize.z;
                }

                if (height < 0)
                {
                    height = _MainTex_TexelSize.w;
                }

                float4 newUv = float4(0, 0, 1, 1);
                newUv.x = x / _MainTex_TexelSize.z;
                newUv.y = y / _MainTex_TexelSize.w;
                newUv.z = width / _MainTex_TexelSize.z;
                newUv.w = height / _MainTex_TexelSize.w;
                
                
                // Apply offset and scale
                float fixedY = -(newUv.y - (1.0f - newUv.w));
                uv = uv * float2(newUv.z, newUv.w) + float2(newUv.x, fixedY);

                o.uv = uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * i.color;
            }
            ENDCG
        }
    }
}
