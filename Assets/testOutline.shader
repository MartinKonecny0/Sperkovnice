Shader "Custom/SpriteExpand"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        [HDR] _OutlineColor("Outline Color", Color) = (0,0,0,2)
        _OutlineThickness("Outline Thickness", Float) = 1
        _Precision("Precision", Float) = 1

    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _Precision;
            float4 _MainTex_TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;

            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 texelSize = _MainTex_TexelSize * _OutlineThickness;
                float alpha = tex2D(_MainTex, i.uv).a;

                float outlineAlpha = 0;
                for (int x = -_Precision; x <= _Precision; x++)
                {
                    for (int y = -_Precision; y <= _Precision; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        float2 offset = texelSize * normalize(float2(x, y));
                        float neighborAlpha = tex2D(_MainTex, i.uv + offset).a;
                        outlineAlpha = max(outlineAlpha, neighborAlpha);
                    }
                }
                // If current pixel is transparent but neighbor is not, draw outline
                if (alpha < 0.9 && outlineAlpha > 0.1)
                {
                    return _OutlineColor;
                }

                // Regular sprite color
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}