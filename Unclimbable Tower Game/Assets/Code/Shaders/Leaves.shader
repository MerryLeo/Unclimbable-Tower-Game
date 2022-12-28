Shader "Custom/LeavesStandard"
{
    Properties
    {
        _Texture ("Texture", 2D) = "" {}
        _Illumination ("Self-illumination", Range(1.0, 5.0)) = 1.0
        _Opacity ("Texture Opacity", Range(0.0, 1.0)) = 0.5
        _MinHeight ("Min Height", Float) = 0
        _MaxHeight ("Max Height", Float) = 10
        _MinColor ("Min Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _MaxColor ("Max Color", Color) = (1.0, 1.0, 1.0, 1.0)
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

        sampler2D _Texture;
        float4 _Texture_ST;
        float _MinHeight, _MaxHeight, _Opacity, _Illumination;
        float4 _MinColor, _MaxColor;

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

        float InvLerp(float a, float b, float v) 
        {
            return (v - a) / (b - a);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Gradient Color
            float height = clamp(IN.worldPos.y, _MinHeight, _MaxHeight);
            float ratio = InvLerp(_MinHeight, _MaxHeight, height);
            float4 gradientCol = lerp(_MinColor, _MaxColor, ratio);

            // Texture Color
            float3 tangent = cross(float3(IN.worldNormal.x, 0, IN.worldNormal.z), float3(0, 1, 0));
            float dist = dot(IN.worldPos.xz, tangent.xz);
            float2 texCoords = float2(_Texture_ST.z + dist * _Texture_ST.x, _Texture_ST.w + IN.worldPos.y * _Texture_ST.y); 
            float4 texCol = tex2D(_Texture, texCoords);

            // Albedo comes from a texture tinted by color
            fixed4 c = (gradientCol - gradientCol * texCol * _Opacity) * _Illumination;
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
