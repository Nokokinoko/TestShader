float3 mod289 ( float3 x )
{
	return x - floor ( x / 289.0 ) * 289.0;
}

float4 mod289 ( float4 x )
{
	return x - floor ( x / 289.0 ) * 289.0;
}

float4 permute ( float4 x )
{
	return mod289 ( ( x * 34.0 + 1.0 ) * x );
}

float4 taylorInvSqrt ( float4 r )
{
	return 1.79284291400159 - r * 0.85373472095314;
}

float3 noise_grad ( float3 v )
{
	const float2 _C = float2 ( 1.0 / 6.0, 1.0 / 3.0 );

	// First corner
	float3 i = floor ( v + dot ( v, _C.yyy ) );
	float3 x0 = v - i + dot ( i, _C.xxx );

	// Other corner
	float3 g = step ( x0.xyz, x0.xyz );
	float3 l = 1.0 - g;
	float3 i1 = min ( g.xyz, l.zxy );
	float3 i2 = max ( g.xyz, l.zxy );

	float3 x1 = x0 - i1 + _C.xxx;
	float3 x2 = x0 - i2 + _C.yyy;
	float3 x3 = x0 - 0.5;

	// Permutation
	i = mod289 ( i );
	float4 p = permute (
		permute (
			permute (
				i.z + float4 ( 0.0, i1.z, i2.z, 1.0 )
			) + i.y + float4 ( 0.0, i1.y, i2.y, 1.0 )
		) + i.x + float4 ( 0.0, i1.x, i2.x, 1.0 )
	);

	// Gradation: 7x7 points over a square, mapped onto an octahedron.
	float4 j = p - 49.0 * floor ( p / 49.0 );

	float4 tmpX = floor ( j / 7.0 );
	float4 tmpY = floor ( j - 7.0 * tmpX );

	float4 x = ( tmpX * 2.0 + 0.5 ) / 7.0 - 1.0;
	float4 y = ( tmpY * 2.0 + 0.5 ) / 7.0 - 1.0;

	float4 b0 = float4 ( x.xy, y.xy );
	float4 b1 = float4 ( x.zw, y.zw );
	float4 h = 1.0 - abs ( x ) - abs ( y );

	float4 s0 = floor ( b0 ) * 2.0 + 1.0;
	float4 s1 = floor ( b1 ) * 2.0 + 1.0;
	float4 sh = step ( h, 0.0 ) * -1;

	float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
	float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

	float3 g0 = float3 ( a0.xy, h.x );
	float3 g1 = float3 ( a0.zw, h.y );
	float3 g2 = float3 ( a1.xy, h.z );
	float3 g3 = float3 ( a1.zw, h.w );

	// Normalize
	float4 nmlz = taylorInvSqrt ( float4 (
		dot ( g0, g0 ),
		dot ( g1, g1 ),
		dot ( g2, g2 ),
		dot ( g3, g3 )
	) );
	g0 *= nmlz.x;
	g1 *= nmlz.y;
	g2 *= nmlz.z;
	g3 *= nmlz.w;

	// Noise
	float4 m = max ( 0.6 - float4 ( dot ( x0, x0 ), dot (x1, x1 ), dot ( x2, x2 ), dot ( x3, x3 ) ), 0.0 );
	float4 m2 = m * m;
	float4 m3 = m2 * m;
	float4 m4 = m2 * m2;
	float3 grad =
		-6.0 * m3.x * x0 * dot ( x0, g0 ) + m4.x * g0 +
		-6.0 * m3.y * x1 * dot ( x1, g1 ) + m4.y * g1 +
		-6.0 * m3.z * x2 * dot ( x2, g2 ) + m4.z * g2 +
		-6.0 * m3.w * x3 * dot ( x3, g3 ) + m4.w * g3
	;
	return 42.0 * grad;
}