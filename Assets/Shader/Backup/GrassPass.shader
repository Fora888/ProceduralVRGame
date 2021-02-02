Shader "GrassPassBackup" {
    Properties
    {
        _ColorBase("Base Color", Color) = (.34, .85, .92, 1) // color
        _ColorTip("Tip Color", Color) = (.34, .85, .92, 1) // color
        _GrassBlades("Grass blades per triangle", Range(0, 13)) = 13
        _GrassWidth("Grass Width", Range(0, 0.5)) = 0.25
        _GrassHeight("Grass Height", Range(0, 1)) = 0.25
        _Tesselation("Tesselation", Range(0, 32)) = 1
        _Curvature("Grass Curvature", Range(0, 1)) = 0.035
        _GrassSegments("Grass Segments", Int) = 1
        _LODScaling("LOD Scaling", float) = 1
        _WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
        _WindVector("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
        _WindStrength("Wind Strength", Float) = 1
    }

    SubShader{

        Tags { "RenderType" = "Opaque" "LightMode" = "UniversalForward" }
        Pass {
            Cull Off
            HLSLPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #pragma geometry geo
            #pragma hull hull
            #pragma domain domain
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma target 4.6
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            float random(float2 st) {
            return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }
            
            // Construct a rotation matrix that rotates around the provided axis, sourced from:
            // https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
            float3x3 AngleAxis3x3(float angle, float3 axis)
            {
                float c, s;
                sincos(angle, s, c);

                float t = 1 - c;
                float x = axis.x;
                float y = axis.y;
                float z = axis.z;

                return float3x3(
                    t * x * x + c,      t * x * y - s * z,  t * x * z + s * y,
                    t * x * y + s * z,  t * y * y + c,      t * y * z - s * x,
                    t * x * z - s * y,  t * y * z + s * x,  t * z * z + c
                    );
            }

            struct appdata {
                float4 position : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2t {
                float4 position : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID

            };

            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            struct t2g {
                float4 pos : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct g2f {
                float4 pos : SV_POSITION;
                float3 color : COLOR;
                float3 posWS : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float _LODScaling;

            v2t vert(appdata v)
            {
                v2t o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.position = v.position;
                o.normal = v.normal;
                return o;
            }

            t2g vertTesselation(v2t v)
            {
                t2g o;
                UNITY_SETUP_INSTANCE_ID(v);
                //DEFAULT_UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.pos = v.position;
                o.normal = v.normal;
                return o;
            }

            float _Tesselation;

            TessellationFactors patchConstantFunction(InputPatch<v2t, 3> patch)
            {
                //
                UNITY_SETUP_INSTANCE_ID(patch[0]);
                VertexPositionInputs vertexInput1 = GetVertexPositionInputs(patch[0].position.xyz);
                VertexPositionInputs vertexInput2 = GetVertexPositionInputs(patch[1].position.xyz);
                VertexPositionInputs vertexInput3 = GetVertexPositionInputs(patch[2].position.xyz);
                float LOD = (((-distance(_WorldSpaceCameraPos, vertexInput1.positionWS) * _LODScaling + 1 ) > 0 ) + ((vertexInput1.positionWS.y > 3) + (vertexInput2.positionWS.y > 3) + (vertexInput3.positionWS.y > 3) == 3) + (vertexInput1.positionWS.y < 110)) == 3;
                TessellationFactors f;
                f.edge[0] = _Tesselation * LOD;
                f.edge[1] = _Tesselation * LOD;
                f.edge[2] = _Tesselation * LOD;
                f.inside = _Tesselation * LOD;
                return f;
            }

            [patchconstantfunc("patchConstantFunction")]
            [domain("tri")]
            [partitioning("integer")]
            [outputtopology("triangle_cw")]
            [outputcontrolpoints(3)]
            v2t hull(InputPatch<v2t, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            [domain("tri")]
            t2g domain(TessellationFactors factors, OutputPatch<appdata, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
            {
                v2t v;
                UNITY_SETUP_INSTANCE_ID(patch[0]);
                #define MY_DOMAIN_PROGRAM_INTERPOLATE(fieldName) v.fieldName = patch[0].fieldName * barycentricCoordinates.x + patch[1].fieldName * barycentricCoordinates.y + patch[2].fieldName * barycentricCoordinates.z;
                MY_DOMAIN_PROGRAM_INTERPOLATE(position)
                MY_DOMAIN_PROGRAM_INTERPOLATE(normal)
                UNITY_TRANSFER_INSTANCE_ID(patch[0], v);
                return vertTesselation(v);
            }

            
            float _GrassBlades;
            float4 _ColorBase;
            float4 _ColorTip;
            float _GrassWidth;
            float _GrassHeight;
            uint _GrassSegments;
            float _Curvature;
            sampler2D _WindDistortionMap;
            float4 _WindDistortionMap_ST;
            float2 _WindVector;
            float _WindStrength;

            [maxvertexcount(93)]
            void geo(triangle t2g input[3], inout TriangleStream<g2f> triStream)
            {
                g2f o;
                UNITY_SETUP_INSTANCE_ID(input[0]);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float3 faceNormal = normalize(input[0].normal.xyz + input[1].normal.xyz + input[2].normal.xyz);
                float LOD = -distance(_WorldSpaceCameraPos.xz, GetVertexPositionInputs(input[0].pos.xyz).positionWS.xz) * _LODScaling + 1 ;
                for (float i = 0; i < _GrassBlades * LOD; i++) {

                    float r1 = random(GetVertexPositionInputs(input[0].pos.xyz).positionWS.xz * (i + 1.0));
                    float r2 = random(GetVertexPositionInputs(input[1].pos.xyz).positionWS.xz * (i + 1.0));
                    float r3 = random(GetVertexPositionInputs(input[2].pos.xyz).positionWS.xz * (i + 1.0));
                    float r4 = r1 + r2 * 0.5;
                    float r5 = r2 + r3 * 0.5;

                    float4 midpoint = (1 - sqrt(r1)) * input[0].pos + (sqrt(r1) * (1 - r2)) * input[1].pos + (sqrt(r1) * r2) * input[2].pos;
                    float3x3 alignToNormal = AngleAxis3x3( length(float2(faceNormal.x, faceNormal.z)) * 3.1415926 * 0.5, float3(-faceNormal.z, 0, faceNormal.x));
                    float3x3 randomRotation = AngleAxis3x3(r3 * 3.1415926 * 2, float3(0, 1, 0));
                    

                    float2 uv = midpoint.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindVector * _Time.y;
                    float windSample = ((tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).x)) * _WindStrength;
                    float3 wind = float3(_WindVector.x,0 , _WindVector.y);
                    float3x3 windRotation = AngleAxis3x3(3.1415926 * windSample, wind);
                    float3 normalVector = mul(mul(float3(0,0,1), randomRotation), alignToNormal);

                    VertexPositionInputs vertexPositions;
                    for (uint i = 0; i < (uint)(_GrassSegments * LOD) + 1; i++)
                    {
                        float segmentMultiplier = i  / (float)_GrassSegments;
                        o.color = lerp(_ColorBase.xyz, _ColorTip.xyz, segmentMultiplier);
                        vertexPositions = GetVertexPositionInputs(normalVector * pow(segmentMultiplier, 3) * _Curvature * r4 + midpoint.xyz + mul(mul(float3(_GrassWidth * (1 - segmentMultiplier), _GrassHeight * segmentMultiplier, 0) * r5, randomRotation), alignToNormal));
                        o.pos = vertexPositions.positionCS;
                        o.posWS = vertexPositions.positionWS;
                        triStream.Append(o);

                        vertexPositions = GetVertexPositionInputs(normalVector * pow(segmentMultiplier, 3) * _Curvature * r4 + midpoint.xyz + mul(mul(float3(-_GrassWidth * (1 - segmentMultiplier), _GrassHeight * segmentMultiplier, 0) * r5, randomRotation), alignToNormal));
                        o.pos = vertexPositions.positionCS;
                        o.posWS = vertexPositions.positionWS;
                        triStream.Append(o);
                    }

                    //Tip
                    o.color = _ColorTip.xyz;
                    vertexPositions = GetVertexPositionInputs(normalVector * pow(1,3) * _Curvature * r4 + midpoint.xyz + mul(mul(float3(0, _GrassHeight * r5, 0), randomRotation), alignToNormal));
                    o.pos = vertexPositions.positionCS;
                    o.posWS = vertexPositions.positionWS;
                    triStream.Append(o);

                    triStream.RestartStrip();
                }
            }

            float4 frag(g2f i) : SV_Target
            {
                half3 normals = normalize(cross(ddy(i.posWS), ddx(i.posWS)));
                //BRDF Data
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - i.posWS.xyz);
                BRDFData brdfData;
                InitializeBRDFData(i.color.xyz, 0, 0, 0, 1, brdfData);


                //lighting
                half3 bakedGI = SampleSH(normals);
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(i.posWS.xyz));
                half3 col = GlobalIllumination(brdfData, bakedGI, 1, normals, viewDirectionWS);
                col += LightingPhysicallyBased(brdfData, mainLight, normals, viewDirectionWS);

                return float4(col,1);
            }

            ENDHLSL
        }
        //UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        //UsePass "Universal Render Pipeline/Lit/DepthOnly"
    }
}