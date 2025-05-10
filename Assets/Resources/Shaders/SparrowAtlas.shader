Shader "Custom/SparrowAtlas"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
        [PerRendererData] _SubTexture ("SubTexture (X, Y, Width, Height)", Vector) = (0, 0, -1, -1)
        [PerRendererData] _SubTextureFrame ("SubTexture (FrameX, FrameY, FrameWidth, FrameHeight)", Vector) = (0, 0, -1, -1)
        [PerRendererData] _PixelsPerUnit ("Pixels Per Unit", Float) = 100
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
                // Warning to self, don't modify this on a batched object
                o.vertex = UnityObjectToClipPos(v.vertex);
                
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
                

                // Calculate the offset percentage of frameX and frameY from the main texture size
                float offsetPercentX = frameX / _MainTex_TexelSize.z;
                float offsetPercentY = frameY / _MainTex_TexelSize.w;
      
                // Calculate the new UV coordinates for the SubTexture original frame region
                float4 newUv = float4(0, 0, 1, 1);
                newUv.x = x / _MainTex_TexelSize.z; // x
                newUv.y = y / _MainTex_TexelSize.w; // y
                newUv.z = frameWidth / _MainTex_TexelSize.z; // width
                newUv.w = frameHeight / _MainTex_TexelSize.w; // height
                
                // Invert the Y coordinate to match Unity's UV system
                float fixedY = -(newUv.y - (1.0f - newUv.w));
                uv = uv * float2(newUv.z, newUv.w) + float2(newUv.x, fixedY);

                // Offset by frameX and frameY on the main UV
                uv.x += offsetPercentX;
                uv.y -= offsetPercentY;
                
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

                // Calculate the clip width and height
                float uvClipWidth = width / _MainTex_TexelSize.z;
                // Invert the Y coordinate to match Unity's UV system
                float uvClipHeight = 1 - height / _MainTex_TexelSize.w;

                // Clip if the UV is outside of the main texture
                clip(1.0f - i.uv.x);
                clip(1.0f - i.uv.y);
                clip(i.uv.x);
                clip(i.uv.y);
                
                // Clip if the UV is outside of the top of the subtexture
                clip(uvClipHeight - y / _MainTex_TexelSize.w + height / _MainTex_TexelSize.w - i.uv.y);

                // Clip if the UV is outside of the bottom of the subtexture
                clip(i.uv.y - (uvClipHeight - y / _MainTex_TexelSize.w));
                
                // Clip if the UV is outside of the right of the subtexture
                clip(uvClipWidth + x / _MainTex_TexelSize.z - i.uv.x);

                // Clip if the UV is outside of the left of the subtexture
                clip(i.uv.x - x / _MainTex_TexelSize.z);
                float4 finalColor = tex2D(_MainTex, i.uv) * i.color;
                return finalColor;
            }
            ENDCG
        }
    }
}
