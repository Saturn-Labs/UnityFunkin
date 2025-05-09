Shader "Custom/SpriteUVSides"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _UVOffset ("UV Offset (XY) and Scale (ZW)", Vector) = (0, 0, 1, 1)
        _UVSides ("UV Sides (L, R, B, T)", Vector) = (0, 0, 0, 0)
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
            fixed4 _Color;
            float4 _UVOffset; // (offsetX, offsetY, scaleX, scaleY)
            float4 _UVSides;  // (left, right, bottom, top)

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

                float2 min = float2(_UVSides.x, _UVSides.z);           // left, bottom
                float2 max = float2(1.0 - _UVSides.y, 1.0 - _UVSides.w); // right, top

                uv = lerp(min, max, uv); // remap original UV into new range

                // Apply offset and scale
                uv = uv * _UVOffset.zw + _UVOffset.xy;

                o.uv = uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                return texColor * i.color;
            }
            ENDCG
        }
    }
}
