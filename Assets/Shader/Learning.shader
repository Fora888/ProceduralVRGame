Shader "Learning" {
    Properties
    {
        _Color ("Color", Color) = (.34, .85, .92, 1) // color
    }
    SubShader{

        Tags { "RenderType" = "Opaque" "LightMode" = "UniversalForward"}
        
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            struct appdata {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 pos2 : TEXCOORD2;
                float3 normal : TEXCOORD3;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.position.xyz);
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normal);
                o.pos = vertexInput.positionCS;
                o.pos2 = v.position.xyz;
                o.normal = vertexNormalInput.normalWS;
                return o;
            }

            float4 _Color;

            float4 frag(v2f i) : SV_Target
            {
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - i.pos.xyz);
                BRDFData brdfData;
                InitializeBRDFData(_Color.xyz, 0, 0, 0, 1, brdfData);
                half3 bakedGI = SampleSH(i.normal);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(i.pos2);
                Light mainLight = GetMainLight(GetShadowCoord(vertexInput));
                half3 col = GlobalIllumination(brdfData, bakedGI, 1, i.normal, viewDirectionWS);
                col += LightingPhysicallyBased(brdfData, mainLight, i.normal, viewDirectionWS);
                return half4(col, 1);
            }
            ENDHLSL
        }
        
        Pass {
            Cull Off
            HLSLPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #pragma geometry geo
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            struct appdata {
                float4 position : POSITION;
            };
            
            struct v2g {
                float4 pos : SV_POSITION;
            };
            
            struct g2f {
                float4 pos : SV_POSITION;
            };

            v2g vert(appdata v)
            {
                v2g o;
                o.pos = v.position;
                return o;
            }
            
            [maxvertexcount(3)]
            void geo(triangle v2g input[3] : SV_POSITION, inout TriangleStream<g2f> triStream)
            {
                g2f o;

                o.pos = GetVertexPositionInputs(input[1].pos + float4(0.5, 0, 0, 1)).positionCS;
                triStream.Append(o);

                o.pos = GetVertexPositionInputs(input[1].pos + float4(-0.5, 0, 0, 1)).positionCS;
                triStream.Append(o);

                o.pos = GetVertexPositionInputs(input[1].pos + float4(0, 1, 0, 1)).positionCS;
                triStream.Append(o);

                triStream.RestartStrip();

                for (int i = 0; i < 3; i++) {
                    o.pos = GetVertexPositionInputs(input[i].pos).positionCS;
                    triStream.Append(o);
                }

                triStream.RestartStrip();
            }
            

            float4 _Color;

            float4 frag(g2f i) : SV_Target
            {
                float4 col = _Color;
                return col;
            }
            
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
    }
}