// Shader that display a vertical gradient with shadows for tree leaves

Shader "Unlit/LeavesShader"
{
    Properties{
        _LowerHeightColor("Lower Height Color", Color) = (0,0,0,1)
        _HigherHeightColor("Higher Height Color", Color) = (1,1,1,1)
        _MaxHeight("Max Height", Float) = 30
        _MinHeight("Min Height", Float) = 0
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _LowerHeightColor;
            float4 _HigherHeightColor;
            float _MaxHeight;
            float _MinHeight;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;

            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldCoords : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };


            v2f vert (appdata v) {
                v2f o;
                o.normal = v.normal;
                o.worldCoords = mul (unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float InvLerp(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            fixed4 frag(v2f i) : SV_Target {
                //return float4(i.normal, 1);

                float height = clamp(i.worldCoords.y, _MinHeight, _MaxHeight);
                float ratio = InvLerp(_MinHeight, _MaxHeight, height);
                float3 col = lerp(_LowerHeightColor, _HigherHeightColor, ratio);
                return float4(col, 1);
            }
            ENDCG
        }

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            Fog { Mode Off }
            ZWrite On 
            ZTest LEqual 
            Cull Off
            Offset 1, 1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct v2f { 
				V2F_SHADOW_CASTER;
			};
	
			v2f vert( appdata_base v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}
	
			float4 frag( v2f i ) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
        }
    }
}
