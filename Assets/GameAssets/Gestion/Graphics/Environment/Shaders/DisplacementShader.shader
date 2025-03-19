Shader "Custom/DisplacementShader"
{
    Properties
    {
        _MainTex ("Cloud Shadow Texture", 2D) = "white" {}
        _HeightMap ("Height Map", 2D) = "black" {}
        _HeightScale ("Height Scale", Float) = 0.1
        _CloudScroll ("Cloud Scroll Offset", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            sampler2D _HeightMap;
            float _HeightScale;
            float4 _CloudScroll; // Use xy for scrolling offset
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                // Transform vertex into clip space.
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Pass through the UV for the cloud sprite.
                o.uv = v.uv;
                // Get world position for static heightmap sampling.
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Scroll the cloud shadow texture.
                float2 cloudUV = i.uv + _CloudScroll.xy;
                
                // Compute UVs for the heightmap.
                // This example assumes a direct mapping from world space (xy) to the heightmap.
                // In a real project, you might want to scale/offset this based on your world dimensions.
                float2 heightUV = i.worldPos.xy;
                fixed height = tex2D(_HeightMap, heightUV).r;
                
                // Calculate an offset based on the height value.
                // For example, objects with greater height (higher value) will receive a larger offset.
                float2 offset = _HeightScale * (height - 0.5);
                
                // Offset the cloud's UV coordinates so the shadow appears shifted on taller objects.
                cloudUV += offset;
                
                // Sample the cloud shadow texture with the adjusted UVs.
                fixed4 col = tex2D(_MainTex, cloudUV);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
