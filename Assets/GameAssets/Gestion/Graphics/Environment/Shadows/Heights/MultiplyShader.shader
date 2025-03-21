Shader "Custom/MultiplyShader"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        // Make sure this renders as transparent
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // Set the blend mode so that:
        // final.rgb = src.rgb * dest.rgb
        // (Note: areas with zero alpha will show no effect)
        Blend DstColor Zero

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
            float4 _Color;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture and apply tint
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;
                // Return the color (the alpha channel is preserved)
                return texColor;
            }
            ENDCG
        }
    }
}
