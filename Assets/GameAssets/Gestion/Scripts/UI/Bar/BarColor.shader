Shader "Custom/OverlayBlendingShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _OverlayTex ("Overlay Texture", 2D) = "white" {}
        _OverlayColor ("Overlay Color", Color) = (1,1,1,1)
        _OverlayStrength ("Overlay Strength", Range(0, 1)) = 1.0
        _Bar2 ("Bar2", Range(0,1)) = 0.0 
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _OverlayTex;
            float4 _OverlayColor;
            float _OverlayStrength;
            float _Bar2; 


            float4 GetInterpolatedColor(float bar2)
            {
                // Define the colors
                float4 color1 = float4(0.290f, 0.549f, 0.753f, 1.0f); // #4a8cc0
                float4 color2 = float4(0.290f, 0.753f, 0.710f, 1.0f); // #4ac0b6
                float4 color3 = float4(0.290f, 0.753f, 0.631f, 1.0f); // #4ac0a1
                float4 color4 = float4(0.322f, 0.651f, 0.353f, 1.0f); // #52a65a
                float4 color5 = float4(0.196f, 0.302f, 0.220f, 1.0f); // #324d38

                // Interpolate between the colors based on the _Bar3 value
                if (bar2 < 0.25)
                    return lerp(color1, color2, bar2 * 4.0); // Interpolates between color1 and color2
                else if (bar2 < 0.5)
                    return lerp(color2, color3, (bar2 - 0.25) * 4.0); // Interpolates between color2 and color3
                else if (bar2 < 0.75)
                    return lerp(color3, color4, (bar2 - 0.5) * 4.0); // Interpolates between color3 and color4
                else
                    return lerp(color4, color5, (bar2 - 0.75) * 4.0); // Interpolates between color4 and color5
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 OverlayBlend(float4 baseColor, float4 overlayColor, float strength)
            {
                float3 result;
                for (int i = 0; i < 3; i++) // Apply to R, G, B channels only
                {
                    if (baseColor[i] < 0.5)
                        result[i] = 2.0 * baseColor[i] * overlayColor[i];
                    else
                        result[i] = 1.0 - 2.0 * (1.0 - baseColor[i]) * (1.0 - overlayColor[i]);
                }
                result = lerp(baseColor.rgb, result, strength);
                return float4(result, baseColor.a);
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 baseColor = tex2D(_MainTex, i.uv);
                float4 overlayTexColor = tex2D(_OverlayTex, i.uv);
                float4 overlayColor = overlayTexColor * _OverlayColor;

                // Get the interpolated overlay color based on _Bar3
                float4 interpolatedColor = GetInterpolatedColor(_Bar2);

                // Blend the base color and the interpolated overlay color
                return OverlayBlend(baseColor, interpolatedColor, _OverlayStrength);
            }
            ENDCG
        }
    }
}
