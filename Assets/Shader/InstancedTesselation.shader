Shader "Custom/InstancedTesselation"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _Tesselation("Tesselation", Range(0, 32)) = 1
    }
        SubShader
    {
        Pass
        {
            //Tags {"LightMode" = "ForwardBase"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma hull hull
            #pragma domain domain
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float random(float2 st) {
            return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct t2f
            {
                float4 pos : SV_POSITION;
                float3 posWS : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2d
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            v2d vert(appdata v)
            {
                v2d output;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, output);
                output.vertex = v.vertex;
                output.normal = v.normal;
                return output;
            }

            t2f tessVert(appdata v)
            {
                t2f output;
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_TRANSFER_INSTANCE_ID(v, output);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz + random(v.vertex.xz));
                output.pos = vertexInput.positionCS;
                output.posWS = vertexInput.positionWS;
                output.normal = v.normal;
                return output;
            }

            float _Tesselation;

            TessellationFactors patchConstantFunction(InputPatch<v2d, 3> patch)
            {
                UNITY_SETUP_INSTANCE_ID(patch[0]);
                VertexPositionInputs vertexInput1 = GetVertexPositionInputs(patch[0].vertex.xyz);
                VertexPositionInputs vertexInput2 = GetVertexPositionInputs(patch[1].vertex.xyz);
                VertexPositionInputs vertexInput3 = GetVertexPositionInputs(patch[2].vertex.xyz);
                TessellationFactors f;
                f.edge[0] = _Tesselation;
                f.edge[1] = _Tesselation;
                f.edge[2] = _Tesselation;
                f.inside = _Tesselation;
                return f;
            }

            [patchconstantfunc("patchConstantFunction")]
            [domain("tri")]
            [partitioning("integer")]
            [outputtopology("triangle_cw")]
            [outputcontrolpoints(3)]
            v2d hull(InputPatch<v2d, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            [domain("tri")]
            t2f domain(TessellationFactors factors, OutputPatch<v2d, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
            {
                v2d v;
                #define MY_DOMAIN_PROGRAM_INTERPOLATE(fieldName) v.fieldName = patch[0].fieldName * barycentricCoordinates.x + patch[1].fieldName * barycentricCoordinates.y + patch[2].fieldName * barycentricCoordinates.z;
                MY_DOMAIN_PROGRAM_INTERPOLATE(vertex)
                MY_DOMAIN_PROGRAM_INTERPOLATE(normal)
                UNITY_TRANSFER_INSTANCE_ID(patch[0], v);
                return tessVert(v);
            }


            float4 _Color;

            float4 frag(t2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - i.posWS.xyz);

                BRDFData brdfData;
                float alpha = 1;
                InitializeBRDFData(_Color.xyz, 0, 0, 0, alpha, brdfData);


                //lighting
                half3 bakedGI = SampleSH(i.normal);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(i.posWS.xyz);
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(i.posWS.xyz));
                half3 col = GlobalIllumination(brdfData, bakedGI, 1, i.normal, viewDirectionWS);
                col += LightingPhysicallyBased(brdfData, mainLight, i.normal, viewDirectionWS);

                //return float4(col,1);
                return float4(col, 1);
            }
        ENDHLSL
    }
    UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    UsePass "Universal Render Pipeline/Lit/DepthOnly"
    }
        Fallback "Diffuse"
}