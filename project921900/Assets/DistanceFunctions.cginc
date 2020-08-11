//Ifinite Plane
float sdPlane(float3 p, float4 n) 
{
	return dot(p, n.xyz) + n.w;
}

// Sphere
// s: radius
float sdSphere(float3 p, float s)
{
	return length(p) - s;
}

// Box
// b: size of box in x/y/z
float sdBox(float3 p, float3 b)
{
	float3 d = abs(p) - b;
	return min(max(d.x, max(d.y, d.z)), 0.0) +
		length(max(d, 0.0));
}

//RoundedBox
float sdRoundBox(in float3 p, in float3 b, in float r)
{
	float3 d = abs(p) - b;
	return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0)) - r; 
}

// BOOLEAN OPERATORS //

// Union
float opU(float d1, float d2)
{
	return min(d1, d2);
}

// Subtraction
float opS(float d1, float d2)
{
	return max(-d1, d2);
}

// Intersection
float opI(float d1, float d2)
{
	return max(d1, d2);
}

// Mod Position Axis
float pMod1 (inout float p, float size)
{
	float halfsize = size * 0.5;
	float c = floor((p+halfsize)/size);
	p = fmod(p+halfsize,size)-halfsize;
	p = fmod(-p+halfsize,size)-halfsize;
	return c;
}

// SMOOTH BOOLEAN OPERATORS

float4 opUS(float4 d1, float4 d2, float k)
{
	float h = clamp(0.5 + 0.5 * (d2.w - d1.w) / k, 0.0, 1.0);
	float3 color = lerp(d2.rgb, d1.rgb, h);
	float dist = lerp(d2.w, d1.w, h) - k * h * (1.0 - h);
	return float4(color, dist);
}

float opSS(float d1, float d2, float k)
{
	float h = clamp(0.5 - 0.5 * (d2 + d1) / k, 0.0, 1.0);
	return lerp(d2, -d1, h) + k * h * (1.0 - h);
}

float opIS(float d1, float d2, float k)
{
	float h = clamp(0.5 - 0.5 * (d2 - d1) / k, 0.0, 1.0);
	return lerp(d2, d1, h) + k * h * (1.0 - h);
}

//--------------------------------------

float SmoothUnion(float d1, float d2, float k) {
	float h = clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
	return lerp(d2, d1, h) - k * h * (1.0 - h);
}

float Union(float obj1, float obj2)
{
	return min(obj1, obj2);
}

float SmoothSubtraction(float d1, float d2, float k) {
	float h = clamp(0.5 - 0.5 * (d2 + d1) / k, 0.0, 1.0);
	return lerp(d2, -d1, h) + k * h * (1.0 - h);
}

float Subtraction(float obj1, float obj2)
{
	return max(-obj1, obj2);
}

float SmoothIntersection(float d1, float d2, float k) {
	float h = clamp(0.5 - 0.5 * (d2 - d1) / k, 0.0, 1.0);
	return lerp(d2, d1, h) + k * h * (1.0 - h);
}

float Intersection(float obj1, float obj2)
{
	return max(obj1, obj2);
}


//---------------------------
struct Hit {
	float dist;
	float3 color;
	float3 hitPos;
	int subject;
	int selected;
};

//---------------------------------------------------------Funzioni che calcolano la distanza
Hit df_Plane(float3 rayPos)
{
	Hit hit;
	hit.subject = false;
	hit.dist = 2 + rayPos.y;
	hit.color = float4(0.0, 0.0, 0.0, 0.0); 
	return hit;
}

Hit df_Sphere(float3 rayPos, float3 spherePos, float size)
{
	float d = length(rayPos - spherePos) - size;
	Hit hit;
	hit.dist = d;
	hit.subject = true;
	return hit;
}

Hit df_Box(float3 rayPos, float3 pos, float s)
{
	pos = rayPos - pos;
	float3 size = float3(s, s, s);
	float3 d = abs(pos) - size;
	float dist = length(max(d, 0.0))
		+ min(max(d.x, max(d.y, d.z)), 0.0); // remove this line for an only partially signed sdf 

	Hit hit;
	hit.subject = true;
	hit.dist = dist;
	return hit;
}

Hit df_Torus(float3 rayPos, float3 pos, float rad) //radius have 2 radius, the main shape and the radius of the border
{
	pos = rayPos - pos;
	float2 radius = float2(rad, rad * 0.3);
	float2 q = float2(length(pos.xz) - radius.x, pos.y);
	float dist = length(q) - radius.y;

	Hit hit;
	hit.subject = true;
	hit.dist = dist;
	return hit;
}