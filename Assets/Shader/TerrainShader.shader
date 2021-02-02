Shader "Custom/TerrainShader"
{
    Properties
    {
        _SandHeight("Sand Height", Int) = 3
        _GrassHeight("Grass Height", Int) = 120
        _StoneHeight("Stone Height", Int) = 200
        _SandColor ("Sand Color", Color) = (0.8666667, 0.6901961, 0, 0)
        _GrassColor("Grass Color", Color) = (0.2862744, 0.5960785, 0.01960784, 0)
        _StoneColor("Grass Color", Color) = (0.254717, 0.254717, 0.254717, 0)
        _SnowColor("Grass Color", Color) = (1, 1, 1, 0)
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
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            float random(float2 st) {
            return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 posWS : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct v2d
            {
                float4 vertex : SV_POSITION;
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
                return output;
            }

            v2f vertt(appdata v)
            {
                v2f output;
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                DEFAULT_UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(output);
                UNITY_TRANSFER_INSTANCE_ID(v, output);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz + random(v.vertex.xz));
                output.pos = vertexInput.positionCS;
                output.posWS = vertexInput.positionWS;
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
            v2f domain(TessellationFactors factors, OutputPatch<v2d, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
            {
                v2d v;
                #define MY_DOMAIN_PROGRAM_INTERPOLATE(fieldName) v.fieldName = patch[0].fieldName * barycentricCoordinates.x + patch[1].fieldName * barycentricCoordinates.y + patch[2].fieldName * barycentricCoordinates.z;
                MY_DOMAIN_PROGRAM_INTERPOLATE(vertex)
                UNITY_TRANSFER_INSTANCE_ID(patch[0], v);
                return vertt(v);
            }
            
            float4 _SandColor;
            float4 _GrassColor;
            float4 _StoneColor;
            float4 _SnowColor;
            int _SandHeight;
            int _StoneHeight;
            int _GrassHeight;

            float4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                //Flat normals
                half3 normals = normalize(cross(ddy(i.posWS), ddx(i.posWS)));
                //Color according to height
                float4 color = (_SandColor * (i.posWS.y < _SandHeight)) +
                    (_GrassColor * (i.posWS.y > _SandHeight && i.posWS.y < _GrassHeight)) +
                    (_StoneColor * (i.posWS.y > _GrassHeight && i.posWS.y < _StoneHeight)) +
                    (_SnowColor * (i.posWS.y > _StoneHeight));

                //BRDF Dat
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - i.posWS.xyz);
                BRDFData brdfData;
                float alpha = 1;
                InitializeBRDFData(color.xyz, 0, 0, 0, alpha, brdfData);

                
                //lighting
                half3 bakedGI = SampleSH(normals);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(i.posWS.xyz);
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(i.posWS.xyz));
                half3 col = GlobalIllumination(brdfData, bakedGI, 1, normals, viewDirectionWS);
                col += LightingPhysicallyBased(brdfData, mainLight, normals, viewDirectionWS);

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