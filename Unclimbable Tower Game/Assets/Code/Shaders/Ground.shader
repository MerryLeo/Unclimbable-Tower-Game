// Simple shader that displays a color and a transparent texture in world coordinates

Shader "Unlit/GroundShader"
{
    Properties 
    {
        _Color ("BaseColor", Color) = (1,1,1,1)
        _Opacity ("Opacity", Range(0, 1)) = 0.5
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 2

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _MainTex_ST;

        struct Input 
        {
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _Opacity;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o) 
        {
            // Albedo from a color and a transparent texture
            fixed4 c = _Color + _Opacity * _Color * tex2D (_MainTex, float2(_MainTex_ST.z + _MainTex_ST.x * IN.worldPos.x, _MainTex_ST.w + _MainTex_ST.y * IN.worldPos.z));
            
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
