Shader "Custom/RingShader" {
    Properties {
        _RingColor ("Ring Color", Color) = (1,1,1,1)
        _RingAmp ("Ring Amplitude", Range(1.0, 5.0)) = 1.0
        _RingThreshold ("Ring Threshold", Range(0.0, 1.0)) = 0.5
        _RingIntensity ("Ring Intensity", Float) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color, _RingColor;
        float _RingThreshold, _RingAmp, _RingIntensity;

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float Remap(float x, float from1, float to1, float from2, float to2) {
            return from2 + (x - from1) * (to2 - from2) / (to1 - from1);
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
            float dotValue = saturate(dot(IN.viewDir, localPos));
            float invDotValue = 1 - dotValue;
            float4 c = (invDotValue < _RingThreshold) ? _Color : _Color + _RingIntensity * _RingColor * float4(_RingAmp * Remap(invDotValue, _RingThreshold, 1, 0, 1).xxx, 1);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
