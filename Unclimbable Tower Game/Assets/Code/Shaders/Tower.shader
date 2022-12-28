Shader "Custom/TowerShader"
{
    Properties
    {
        _TopTex ("Top Texture", 2D) = "white" {}
        _TopColor ("Top Color", Color) = (0.5, 0.5, 0.5, 1)
        _TopOpacity ("Opacity", Range(0.0, 1.0)) = 0.5
        _SideTex ("Side Texture", 2D) = "white" {}
        _SideColor ("Side Color", Color) = (0.5, 0.5, 0.5, 1)
        _SideOpacity ("Side Opacity", Range(0.0, 1.0)) = 0.5
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #define PI 3.14159265359

        sampler2D _TopTex, _SideTex;
        float4 _TopColor, _SideColor, _TopTex_ST, _SideTex_ST;
        float _TopOpacity, _SideOpacity;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float4 TurnToLightmap(float4 col)
        {
            return (col / 4.0).xxxx;
        }

        float Remap(float x, float from1, float to1, float from2, float to2)
        {
            return from2 + (x - from1) * (to2 - from2) / (to1 - from1);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Side Color
            float3 tangent = cross(float3(IN.worldNormal.x, 0, IN.worldNormal.z), float3(0, 1, 0));
            float dist = dot(IN.worldPos.xz, tangent.xz);
            float2 sideCoords = float2(_SideTex_ST.z + dist * _SideTex_ST.x, _SideTex_ST.w + IN.worldPos.y * _SideTex_ST.y); 
            fixed4 sideCol = _SideColor + _SideOpacity * TurnToLightmap(tex2D(_SideTex, sideCoords));

            // Top color
            half2 topCoords = half2(_TopTex_ST.z + IN.worldPos.x * _TopTex_ST.x, _TopTex_ST.w + IN.worldPos.z * _TopTex_ST.y);
            fixed4 topCol = _TopColor + _TopOpacity * TurnToLightmap(tex2D(_TopTex, topCoords));

            fixed4 c = (IN.worldNormal.y > 0.95) ? topCol : sideCol;

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
