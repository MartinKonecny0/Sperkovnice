Shader "Custom/SpriteExpand"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CellSize ("Cell size", float) = 1
        _NumOfCells ("Number of cells", float) = 10

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
            float4 _MainTex_TexelSize;
            int _NumOfCells;

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
                fixed4 col1 = tex2D(_MainTex, i.uv);
                float period = 1.0 / _NumOfCells; // 0.1
                float halfPeriod = period / 2;
                int2 cellIndex;
                cellIndex.x = i.uv.x / period;
                cellIndex.y = i.uv.y / period;

                
            
           
                float4 centerColor = tex2D(_MainTex, period * cellIndex + halfPeriod);
                return centerColor;
                

         
                //return lerp(col1, col2, 0.5);
            }
            ENDCG
        }
    }
}