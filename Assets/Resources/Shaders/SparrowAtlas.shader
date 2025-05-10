Shader "Custom/SparrowAtlas"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _SubTexture ("SubTexture (X, Y, Width, Height)", Vector) = (0, 0, -1, -1)
        _SubTextureFrame ("SubTexture (FrameX, FrameY, FrameWidth, FrameHeight)", Vector) = (0, 0, -1, -1)
        _PixelsPerUnit ("Pixels Per Unit", Float) = 100
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
            float4 _SubTextureFrame;
            float _PixelsPerUnit;

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
                float2 uv = v.uv;
                int4 subTex = int4(_SubTexture);
                int4 subTexFrame = int4(_SubTextureFrame);
                
                float x = subTex.x;
                float y = subTex.y;
                float width = subTex.z;
                float height = subTex.w;

                float frameX = subTexFrame.x;
                float frameY = subTexFrame.y;
                float frameWidth = subTexFrame.z;
                float frameHeight = subTexFrame.w;

                if (width <= 0)
                {
                    width = _MainTex_TexelSize.z;
                }

                if (height <= 0)
                {
                    height = _MainTex_TexelSize.w;
                }

                if (frameWidth <= 0)
                {
                    frameWidth = width;
                }

                if (frameHeight <= 0)
                {
                    frameHeight = height;
                }

                v.vertex.x -= (frameX / frameWidth * _MainTex_TexelSize.z) / _PixelsPerUnit;
                v.vertex.y += (frameY / frameHeight * _MainTex_TexelSize.w) / _PixelsPerUnit;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
      
                // Calculate the new UV coordinates for the SubTexture original frame region
                float4 newUv = float4(0, 0, 1, 1);
                newUv.x = x / _MainTex_TexelSize.z; // x
                newUv.y = y / _MainTex_TexelSize.w; // y
                newUv.z = frameWidth / _MainTex_TexelSize.z; // width
                newUv.w = frameHeight / _MainTex_TexelSize.w; // height
                
                // Invert the Y coordinate to match Unity's UV system
                float fixedY = -(newUv.y - (1.0f - newUv.w));
                uv = uv * float2(newUv.z, newUv.w) + float2(newUv.x, fixedY);

                o.uv = uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                int4 subTex = int4(_SubTexture);
                float x = subTex.x;
                float y = subTex.y;
                float width = subTex.z;
                float height = subTex.w;

                float uvClipWidth = width / _MainTex_TexelSize.z;
                float uvClipHeight = 1 - height / _MainTex_TexelSize.w;

                clip(1.0f - i.uv.x);
                clip(1.0f - i.uv.y);
                clip(i.uv.y - (uvClipHeight - y / _MainTex_TexelSize.w));
                clip(uvClipWidth + x / _MainTex_TexelSize.z - i.uv.x);
                return tex2D(_MainTex, i.uv) * i.color;
            }
            ENDCG
        }
    }
}
