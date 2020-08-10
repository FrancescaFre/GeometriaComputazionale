﻿﻿Shader "Project/RaymarchShaderSimple"
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "DistanceFunctions.cginc"

             #define SCENE_SIZE 10

            sampler2D _MainTex;

            uniform sampler2D _CameraDepthTexture;
            uniform float4x4 _CamFrustum, _CamToWorld;

            //--- INPUT DATA BLOCKS

            int _ArrayLength = 0;
            uniform float _Array[SCENE_SIZE];

            uniform float _shapes[SCENE_SIZE];
            uniform float _ops[SCENE_SIZE];
            uniform float _sel[SCENE_SIZE];
            uniform float _auto[SCENE_SIZE];
            uniform float _morph[SCENE_SIZE];
            uniform float _size[SCENE_SIZE];
            uniform float3 _positions[SCENE_SIZE];
            uniform float4 _rotations[SCENE_SIZE];
            uniform float4 _colors[SCENE_SIZE];

            uniform int _scene_size;
            uniform int plane = 0;


            //--- INPUT DATA RENDERING
            uniform float _accuracy;
            uniform float _maxDistance;
            uniform float _boxround;
            uniform float _smooth1;
            uniform float _smooth2;
            uniform float _LightInt;
            uniform float _ShadowIntensity;
            uniform float _penumbra;

            uniform int _maxIterations;

            uniform float2 _shadowDistance;

            uniform float3 _LightDir;

            uniform float3 _modInterval;

            uniform fixed4 _mainColor, _LightColor;

            uniform float4 _sphere1, _sphere2, _box1;

            //--- DATA STRUCTUREs
            struct block {
                float shape;
                float op;
                float sel;
                float auto_;
                float morph;
                float size;
                float3 position;
                float4 rotation;
                float4 color;
            };

            static block scene[SCENE_SIZE];

            struct RM {
                Hit hit;
                float travel;
            };
            //--

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

            float2x2 Rotation(float a) {
                float s = sin(a);
                float c = cos(a);
                return float2x2(c, -s, s, c);
            }

            //-------------------------------------------- Project

            Hit ShapeDistance(float3 raypos, block b)
            {
                if (b.shape == 0)
                    return df_Sphere(raypos, b.position, b.size);
                if (b.shape == 1)
                    return df_Box(raypos, b.position, b.size);
                if (b.shape == 2)
                    return df_Torus(raypos, b.position, b.size);

                Hit hit;
                return hit;
            }

            float3 GetColor(float3 raypos) {
                float3 color = float3 (0.0, 0.0, 0.0);
                if (plane == 1)
                    color = max(0.0, 1.0 - df_Plane(raypos).dist) * float3(0.0, 0.4, 0.0) * 1.0;

                for (int i = 0; i < SCENE_SIZE; i++) {
                    if (scene[i].op != 1.0)
                        color += max(0.0, 1.0 - ShapeDistance(raypos, scene[i]).dist) * scene[i].color * 1.0;
                }
                return color;
            }

            //provare a metterlo dentro i for di distance field
            float GetBorder(float3 raypos) {
                float border = 0.0;
                for (int i = 0; i < SCENE_SIZE; i++) {
                    Hit shape = ShapeDistance(raypos, scene[i]);
                    if (shape.dist < 0.3)
                        border += scene[i].sel;
                }
                return clamp(border, 0.0, 1.0);
            }


            Hit distanceField(float3 p)
            {
                Hit result;

                result.dist = 1e20;
                if (plane == 1)
                    result = df_Plane(p);

                //union
                for (int i = 0; i < SCENE_SIZE; i++) {
                    if (scene[i].op == 0.0) {
                        block b = scene[i];
                        Hit shape = ShapeDistance(p, b);
                        result.dist = SmoothUnion(result.dist, shape.dist, _smooth1);
                    }
                }

                //sub
                for (int j = 0; j < SCENE_SIZE; j++) {
                    if (scene[j].op == 1) {
                        Hit shape = ShapeDistance(p, scene[j]);
                        result.dist = SmoothSubtraction(result.dist, shape.dist, _smooth2);
                    }
                }

                //intersection
                for (int k = 0; k < SCENE_SIZE; k++) {
                    if (scene[k].op == 2) {
                        Hit shape = ShapeDistance(p, scene[k]);
                        result.dist = SmoothIntersection(result.dist, shape.dist, _smooth1);
                    }
                }

                result.color = GetColor(p);
                result.selected = GetBorder(p);
                return result;

            }
         

            //---- SHADING
            float3 getNormal(float3 p)
            {
                const float2 offset = float2(0.001, 0.0);
                float3 n = float3(
                    distanceField(p + offset.xyy).dist - distanceField(p - offset.xyy).dist,
                    distanceField(p + offset.yxy).dist - distanceField(p - offset.yxy).dist,
                    distanceField(p + offset.yyx).dist - distanceField(p - offset.yyx).dist
                    );
                return normalize(n);
            }

            float3 softShadow(float3 ro, float3 rd, float mint, float maxt, float k)
            {
                float result = 1.0;
                for (float t = mint; t < maxt; )
                {
                    float h = distanceField(ro + rd * t).dist;
                    if (h < 0.001)
                        return 0.0;
                    result = min(result, k * h / t);
                    t += h;
                }
                return result;
            }

            float3 Rendering(float3 p, float3 cameraPosition, Hit target) {
                float3 result;
                float3 color = target.color;
                float3 n = getNormal(p);

                float3 light = (_LightColor * dot(-_LightDir, n) * 0.5 + 0.5) * _LightInt;

                float shadow = softShadow(p, -_LightDir, _shadowDistance.x, _shadowDistance.y, _penumbra) * 0.5 + 0.5; //shadow intensity
                shadow = max(0.0, pow(shadow, _ShadowIntensity));
                result = color * light * shadow;
                return result;
            }

            float3 Shading(float3 p, float3 n)
            {
                float3 result;
                float3 color = _mainColor.rgb;

                float3 light = (_LightColor * dot(-_LightDir, n) * 0.5 + 0.5) * _LightInt;

                float shadow = softShadow(p, -_LightDir, _shadowDistance.x, _shadowDistance.y, _penumbra) * 0.5 + 0.5; //shadow intensity
                shadow = max(0.0, pow(shadow, _ShadowIntensity));
                result = color * light * shadow;
                return result;
            }


            //---- RAYMARCHING
            RM Raymarching(float3 ro, float3 rd, float depth)
            {
                const int max_iteration = _maxIterations;
                float t = 0; //distance travelled along the ray dir

                Hit hit;

                for (int i = 0; i < max_iteration; i++)
                {
                    float3 p = ro + rd * t; //position
                    hit = distanceField(p); //check in distancefield
                    t += hit.dist;

                    if (t > _maxDistance || t >= depth || hit.dist < _accuracy)
                        break;
                }

                RM result;
                result.travel = t;
                result.hit = hit;
                return result;
            }



            fixed4 frag(v2f i) : SV_Target
            {
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
                depth *= length(i.ray);
                fixed3 col = tex2D(_MainTex, i.uv);
                float3 rayDirection = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;
                   
                
                //update data
                [unroll] ///array reference cannot be used as an l-value; not natively addressable, forcing loop to unroll
                for (int i = 0; i < 1; i++)
                {

                    // warning: 
                    //array reference cannot be used as an l-value; not natively addressable, forcing loop to unroll 
                    scene[i].shape = _shapes[i];
                    scene[i].op = _ops[i];
                    scene[i].sel = _sel[i];
                    scene[i].auto_ = _auto[i];
                    scene[i].morph = _morph[i];
                    scene[i].size = _size[i];
                    scene[i].position = _positions[i];
                    scene[i].rotation = _rotations[i];
                    scene[i].color = _colors[i];   
                }
               
                //to do: mettere un campo in rm per indicare il result.w = 0, in modo che appaia lo sfondo
                //fixed4 result = raymarching(rayOrigin, rayDirection, depth);
                RM raymarch = Raymarching(rayOrigin, rayDirection, depth);

                float a = raymarch.travel / 35.0; 
                float4 color = float4(a, a, a, 1.0);

                return color;

                if (raymarch.hit.dist < _accuracy)
                {
                    float3 hit_point = rayOrigin + rayDirection * raymarch.travel;
                    color = float4(Rendering(hit_point, rayOrigin, raymarch.hit),1);
                }

                return fixed4(col * (1.0 - color.w) + color.xyz * color.w, 1.0);
            }
            ENDCG
        }
    }
}