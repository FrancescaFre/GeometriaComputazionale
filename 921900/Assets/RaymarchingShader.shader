Shader "Project/RaymarchingShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #pragma exclude_renderers d3d11 gles

            #include "UnityCG.cginc"
            #include "DistanceFunctions.cginc"

            sampler2D _MainTex;
            uniform sampler2D _CameraDepthTexture;
            uniform float4x4 _CamFrustum, _CamToWorld;
            uniform float  _maxDistance, _boxround, _smooth1, _smooth2, _LightInt, _ShadowIntensity, _penumbra, _accuracy;
            uniform float4 _sphere1, _sphere2, _box1;
            uniform float3 _LightDir, _LightColor;
            uniform fixed4 _mainColor;
            uniform float3 _modInterval;
            uniform float2 _shadowDistance;
            uniform int _maxIterations;

            uniform float4[10] _shape_and_op_and_select;
            uniform float4[10] _size_auto_morph;
            uniform float4[10] _position;
            uniform float4[10] _rotation;
            uniform float4[10] _color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ray : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                half index = v.vertex.z;
                v.vertex.z = 0;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                o.ray = _CamFrustum[(int)index].xyz;

                o.ray /= abs(o.ray.z);

                o.ray = mul(_CamToWorld, o.ray);

                return o;
            }
            float2x2 Rot(float a) {
                float s = sin(a);
                float c = cos(a);
                return float2x2(c, -s, s, c);
            }

            float BoxSphere(float3 p) {
                float3 rp = p;
                rp.xy = mul(Rot(90.0), rp.xy);

                float s1 = sdSphere((p - _sphere1.xyz), _sphere1.w);
                //float b1 = sdRoundBox(p - _box1.xyz, _box1.www, _boxround);
                float b1 = sdBox(rp - _box1.xyz, _box1.www);
                float combine1 = opSS(s1, b1, _smooth1);

                float s2 = sdSphere((p - _sphere2.xyz), _sphere2.w);
                float combine2 = opIS(s2, combine1, _smooth2);

                return combine2;
            }

            float distanceField(float3 p)
            {
                if (_modInterval.x != 0 && _modInterval.y != 0 && _modInterval.z != 0) {
                    float modX = pMod1(p.x, _modInterval.x);
                    float modY = pMod1(p.y, _modInterval.y);
                    float modZ = pMod1(p.z, _modInterval.z);
                }
                float ground = sdPlane(p, float4(0, 1, 0, 0));
                float boxsphere = BoxSphere(p);
                return opU(boxsphere, ground);
            }

            float3 getNormal(float3 p)
            {
                const float2 offset = float2(0.001, 0.0);
                float3 n = float3(
                    distanceField(p + offset.xyy) - distanceField(p - offset.xyy),
                    distanceField(p + offset.yxy) - distanceField(p - offset.yxy),
                    distanceField(p + offset.yyx) - distanceField(p - offset.yyx)
                    );
                return normalize(n);
            }

            float3 hardShadow(float3 ro, float3 rd, float mint, float maxt)
            {
                for (float t = mint; t < maxt; )
                {
                    float h = distanceField(ro + rd * t);
                    if (h < 0.0001)
                        return 0.0;
                    t += h;
                }
                return 1.0;
            }

            float3 softShadow(float3 ro, float3 rd, float mint, float maxt, float k)
            {
                float result = 1.0;
                for (float t = mint; t < maxt; )
                {
                    float h = distanceField(ro + rd * t);
                    if (h < 0.001)
                        return 0.0;
                    result = min(result, k * h / t);
                    t += h;
                }
                return result;
            }

            uniform float _AOStep, _AOIntensity;
            uniform int _AOIterations;
            float3 AmbientOcclusion(float3 p, float3 n)
            {
                float step = _AOStep;
                float ao = 0.0;
                float dist;
                for (int i = 1; i <= _AOIterations; i++)
                {
                    dist = step * i;
                    ao += max(0.0, (dist - distanceField(p + n * dist)) / dist);
                }
                return (1.0 - ao * _AOIntensity);
            }

            float3 Shading(float3 p, float3 n)
            {
                float3 result;
                float3 color = _mainColor.rgb;

                float3 light = (_LightColor * dot(-_LightDir, n) * 0.5 + 0.5) * _LightInt;

                float shadow = softShadow(p, -_LightDir, _shadowDistance.x, _shadowDistance.y, _penumbra) * 0.5 + 0.5; //shadow intensity
                shadow = max(0.0, pow(shadow, _ShadowIntensity));

                float AO = AmbientOcclusion(p, n);

                result = color * light * shadow * AO;
                return result;
            }

            fixed4 raymarching(float3 ro, float3 rd, float depth)
            {
                fixed4 result = fixed4(1, 1, 1, 1);
                const int max_iteration = _maxIterations;
                float t = 0; //distance travelled along the ray dir

                for (int i = 0; i < max_iteration; i++)
                {
                    if (t > _maxDistance || t >= depth)
                    {
                        //env
                        result = fixed4(rd, 0);
                        break;
                    }
                    float3 p = ro + rd * t; //position
                    //check in distancefield
                    float d = distanceField(p);
                    if (d < _accuracy)
                    {
                        //shading
                        float3 n = getNormal(p);
                        float3 s = Shading(p, n);

                        result = fixed4(s, 1);
                        break;
                    }
                    t += d;
                }

                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
                depth *= length(i.ray);


                fixed3 col = tex2D(_MainTex, i.uv);
                float3 rayDirection = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;
                fixed4 result = raymarching(rayOrigin, rayDirection, depth);

                return fixed4(col * (1.0 - result.w) + result.xyz * result.w, 1.0);

            }
            ENDCG
        }
    }
}
