// Shader that renders a vertical transparent wave for the player's spawn point

Shader "Unlit/SpawnShader"
{
    Properties 
    {
        _ColorA ("BaseColor", Color) = (1,1,1,1)
        _ColorB ("EndColor", Color) = (0,0,0,1)
        _Offset ("Offset", Range(-10, 10)) = 0
        _Opacity ("Opacity", Float) = 1
        _Amplitude ("Amplitude", Range(-5, 5)) = 0
        _Frequency ("Frequency", Float) = 2
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent" 
        }

        Blend One One
        Cull Off
        ZWrite Off

        LOD 100

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _ColorA, _ColorB;
            float _Offset, _Amplitude, _Frequency, _Opacity;

            struct appdata 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f 
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            float GetWave(float t) 
            {
                return saturate(_Amplitude * sin( _Frequency * t));
            }

            v2f vert (appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            float4 frag (v2f i) : SV_Target 
            {
                float value = saturate( i.uv.y * GetWave(-_Time + i.uv.y));
                float4 color = lerp(_ColorA, _ColorB, value) * float4((1 - saturate(i.uv.y * _Offset)).xxx, 1);
                color *= (abs(i.normal.y) < 0.999); // remove the caps
                return float4(color * _Opacity);
            }
            ENDCG
        }
    }
}
