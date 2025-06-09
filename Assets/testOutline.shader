Shader "Custom/SpriteExpand"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ExpandAmount("Expand Amount", Float) = 10.0
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
            float _ExpandAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // Centered UV from (0.5, 0.5)
                float2 centeredUV = v.uv - float2(0.5, 0.5);

                // Expand in direction of UV
                float3 offset = float3(centeredUV * _ExpandAmount, 0);

                // Apply to vertex
                float4 worldVertex = v.vertex + float4(offset, 0);
                o.vertex = UnityObjectToClipPos(worldVertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Otherwise, fully transparent
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}