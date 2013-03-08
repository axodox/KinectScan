//Definitions------------------------------------------------------------------
#define GaussCoeffCount 9
#define Pi 3.14159

//Types------------------------------------------------------------------------
struct VertexPositionDepth
{
    float4 Position : POSITION0;
	float Depth : NORMAL1;	
};

struct VertexPositionColor
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexPositionTexture
{
    float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexPositionTextureDepth
{
    float4 Position : POSITION0;
	float Depth : POSITION1;
	float2 TextureCoordinate : TEXCOORD0;	
};

struct VertexPositionSegment
{
    float3 Position : POSITION0;
	float Segment : TEXCOORD0;	
};

struct VertexPositionSegmentDepth
{
    float4 Position : POSITION0;
	float Segment : TEXCOORD0;	
	float Depth : NORMAL0;
};

struct VertexPositionWorld
{
    float4 Position : POSITION0;
	float4 WorldPosition : TEXCOORD0;
};

//Globals----------------------------------------------------------------------
uniform extern float NaN;

uniform extern bool ClipOn = true;
uniform extern float ClipRadiusMax = 0.3;
uniform extern float ClipRadiusMin = 0;
uniform extern float ClipYMin = 0;

uniform extern float DepthAvgCountMin = 2;
uniform extern float DepthAvgLimit = 0.1;
uniform extern float DepthConstA = -0.0233458;
uniform extern float DepthConstB = 354.988;
uniform extern float DepthConstC = 1093.09;
uniform extern float2 DepthCorrectionTextureSize;
uniform extern float4x4 DepthInverseIntrinsics;
uniform extern float DepthStepH = 1. / 640;
uniform extern float DepthStepV = 1. / 480;
uniform extern float DepthLimitZ = 1.1;

uniform extern float ProjXMin;
uniform extern float ProjYMin;
uniform extern float ProjWidthMax;
uniform extern float ProjHeightMax;

uniform extern float4x4 FusionTransform;
uniform extern float4x4 LinesTransform;
uniform extern float4x4 ReprojectionTransform;
uniform extern float4x4 SpaceTransform;
uniform extern float4x4 ModelTransform;

uniform extern float4 ShadingColor;
uniform extern float SegmentLength;
uniform extern float SideSelector = 1;
uniform extern float4 SplitPlaneVector = float4(1, 0, 0, 0);
uniform extern float2 TargetSize = float2(640, 480);
uniform extern float TriangleRemoveLimit = 0.05;
uniform extern float3 CorePos;

uniform extern float GaussWeightsH[GaussCoeffCount], GaussWeightsV[GaussCoeffCount];
uniform extern float GaussPosH[GaussCoeffCount], GaussPosV[GaussCoeffCount];

uniform float DepthDiffMax = 0.01;

//Textures---------------------------------------------------------------------
uniform extern texture DepthCorrectionTexture;	
uniform extern texture DepthTexture;
uniform extern texture DepthTextureA, DepthTextureB;
uniform extern texture MainTexture;
//Samplers---------------------------------------------------------------------
sampler DepthCorrectionSampler = sampler_state
{
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	Texture = <DepthCorrectionTexture>;	
};

sampler DepthSampler = sampler_state
{
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	Texture = <DepthTexture>;	
};

sampler DepthSamplerA = sampler_state
{
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	Texture = <DepthTextureA>;	
};

sampler DepthSamplerB = sampler_state
{
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	Texture = <DepthTextureB>;	
};

sampler MainSampler = sampler_state
{
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	Texture = <MainTexture>;	
};
//Functions--------------------------------------------------------------------
float ToDepth(float x)
{
	return DepthConstA - DepthConstB / (x - DepthConstC);
}

float3 PosOf(float x, float y)
{
	float4 s = tex2Dlod(DepthSampler, float4(x, y, 0, 0));
	if(s.y == 0 || s.x == 0)
		return float4(NaN,NaN,NaN,NaN);
	else
		return mul(DepthInverseIntrinsics, float4(640 * x * s.x, 480 * y * s.x, s.x, 1)).xyz;
}

float3 AbsPosOf(float x, float y)
{
	float s = abs(tex2Dlod(DepthSampler, float4(x, y, 0, 0)).x);
	if(s == 0)
		return float4(NaN,NaN,NaN,NaN);
	else
		return mul(DepthInverseIntrinsics, float4(640 * x * s.x, 480 * y * s.x, s.x, 1)).xyz;
}

//Vertex shaders---------------------------------------------------------------
VertexPositionColor SimpleVertexShader(VertexPositionColor i)
{
	VertexPositionColor o;
	float4 pos =  mul(ModelTransform, i.Position);
	pos.x = (pos.x / pos.z / 640 * 2 - 1);
	pos.y = (-pos.y / pos.z / 480 * 2 + 1);
	pos.z = 1;	
	o.Position = pos;
	o.Color = i.Color;
	return o;
}

VertexPositionTexture MiniPlaneVertexShader(VertexPositionTexture i)
{
	VertexPositionTexture o = i;
	o.Position.xy *= 2.;
	o.Position.xy -= float2(0.5/TargetSize.x, -0.5/TargetSize.y);
	return o;
}

VertexPositionDepth PerspectiveReprojectionVertexShader(VertexPositionTexture input)
{
	VertexPositionDepth output;
	output.Position = NaN;
	output.Depth = 0;
	float3 posx = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y);
	float3 posu = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y + DepthStepV);
	float3 posd = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y - DepthStepV);
	float3 posl = PosOf(input.TextureCoordinate.x - DepthStepH, input.TextureCoordinate.y);
	float3 posr = PosOf(input.TextureCoordinate.x + DepthStepH, input.TextureCoordinate.y);
	if(pow(length(posu - posd) / posx.z, 2) + pow(length(posr - posl) / posx.z, 2) > TriangleRemoveLimit) return output;

	float2 uv = input.TextureCoordinate;
	float4 source = tex2Dlod(DepthSampler, float4(uv.x, uv.y, 0, 0));
	float depth = source.x;
	float4 startpos, pos, rpos;
	startpos = float4(640 * uv.x * depth, 480 * uv.y * depth, depth, 1);
	rpos = mul(SpaceTransform, startpos);
	float pl = length(rpos.xz);
	if (source.y == 0 || depth == 0 || ((pl > ClipRadiusMax || pl < ClipRadiusMin || rpos.y > ClipYMin) && ClipOn))
	{
		return output;
	}
	else
	{
		pos = mul(ReprojectionTransform, startpos);
	    pos.x = (pos.x / pos.z / 640 * 2 - 1) * DepthLimitZ;
		pos.y = (-pos.y / pos.z / 480 * 2 + 1) * DepthLimitZ;
		pos.w = DepthLimitZ;

		output.Position = pos;
		output.Depth = pos.z;
		return output;
	}
	
}

VertexPositionWorld OrthographicReprojectionVertexShader(VertexPositionTexture input)
{
	VertexPositionWorld output;
	float3 posx = AbsPosOf(input.TextureCoordinate.x, input.TextureCoordinate.y);
	float3 posu = AbsPosOf(input.TextureCoordinate.x, input.TextureCoordinate.y + DepthStepV);
	float3 posd = AbsPosOf(input.TextureCoordinate.x, input.TextureCoordinate.y - DepthStepV);
	float3 posl = AbsPosOf(input.TextureCoordinate.x - DepthStepH, input.TextureCoordinate.y);
	float3 posr = AbsPosOf(input.TextureCoordinate.x + DepthStepH, input.TextureCoordinate.y);
	if(pow(length(posu - posd) / posx.z, 2) + pow(length(posr - posl) / posx.z, 2) > TriangleRemoveLimit)
	{		
		output.Position = NaN;
		output.WorldPosition = NaN;		
		return output;
	}

	float2 uv = input.TextureCoordinate;
	float4 source = tex2Dlod(DepthSampler, float4(uv.x, uv.y, 0, 0));
	float depth = abs(source.x);
	float4 startpos, pos, spos;
	if (source.y == 0 || depth == 0)
	{
		pos = NaN;
		spos = NaN;
	}
	else
	{
		startpos = float4(640 * uv.x * depth, 480 * uv.y * depth, depth, 1);
		spos = mul(SpaceTransform, startpos);
		spos.w = depth;

		pos = spos;
		pos.x = (pos.x - ProjXMin) / ProjWidthMax * 2 - 1;
		pos.y = -(pos.y - ProjYMin) / ProjHeightMax * 2 + 1;
		pos.z += DepthLimitZ / 2;
		pos.w = 1;

		spos = mul(FusionTransform, startpos);
		spos.w = pos.z;
	}
	output.Position = pos;
	output.WorldPosition = spos;
	return output;
}

VertexPositionWorld PolarReprojectionVertexShader(VertexPositionTexture input)
{
	VertexPositionWorld output;
	float3 posx = AbsPosOf(input.TextureCoordinate.x, input.TextureCoordinate.y);
	float3 posu = AbsPosOf(input.TextureCoordinate.x, input.TextureCoordinate.y + DepthStepV);
	float3 posd = AbsPosOf(input.TextureCoordinate.x, input.TextureCoordinate.y - DepthStepV);
	float3 posl = AbsPosOf(input.TextureCoordinate.x - DepthStepH, input.TextureCoordinate.y);
	float3 posr = AbsPosOf(input.TextureCoordinate.x + DepthStepH, input.TextureCoordinate.y);
	if(pow(length(posu - posd) / posx.z, 2) + pow(length(posr - posl) / posx.z, 2) > TriangleRemoveLimit)
	{		
		output.Position = NaN;
		output.WorldPosition = NaN;		
		return output;
	}
	
	float2 uv = input.TextureCoordinate;
	float4 source = tex2Dlod(DepthSampler, float4(uv.x, uv.y, 0, 0));
	float depth = abs(source.x);
	float4 startpos, pos, spos, rpos;
	if (source.y == 0 || depth == 0)
	{
		pos = NaN;
		spos = NaN;
	}
	else
	{
		startpos = float4(640 * uv.x * depth, 480 * uv.y * depth, depth, 1);
		rpos = pos = mul(FusionTransform, startpos);

		pos -= float4(CorePos, 0);
		spos=pos;
		
		float fi = atan2(pos.z, pos.x) / Pi;
		if(abs(fi)>0.97 || dot(SplitPlaneVector, rpos) > 0)	fi=NaN;

		pos.x = fi;
		pos.y = -(pos.y - ProjYMin) / ProjHeightMax * 2 + 1;
		pos.z = sqrt(pos.z*pos.z+pos.x*pos.x);
		pos.w = 1;

	}
	output.Position = pos;
	output.WorldPosition = spos;
	return output;
}


VertexPositionSegmentDepth ModelLinesVertexShader(VertexPositionSegment input)
{
	VertexPositionSegmentDepth output;
	float4 pos;

	pos.xyz = input.Position;
	pos.w = 1;
	pos = mul(LinesTransform, pos);
	pos.x = (pos.x - ProjXMin) / ProjWidthMax * 2 - 1;
	pos.y = -(pos.y - ProjYMin) / ProjHeightMax * 2 + 1;
	pos.z += DepthLimitZ / 2;
	output.Position = pos;
	output.Segment = input.Segment;
	output.Depth = pos.z;
	return output;
}

//Pixel shaders - Visualization------------------------------------------------
float4 SimplePixelShader(float2 uv : TEXCOORD) : COLOR0
{
	return tex2D(MainSampler, uv);
}

float4 DepthVisualizationPixelShader(float2 uv : TEXCOORD) : COLOR0
{
	float x = tex2D(MainSampler, uv).x / DepthLimitZ;
	if(x == 0)
		return 1;
	else
		return float4(0, x, 0, 1);
}

float4 SignedDepthVisualizationPixelShader(float2 uv : TEXCOORD) : COLOR0
{
	float x = tex2D(MainSampler, uv).x / DepthLimitZ;
	if(x == 0)
		return 1;
	else if(x > 0)
		return float4(x, 0, 0, 1);
	else
		return float4(0, -x, 0, 1);
}
//Pixel shaders - Processing---------------------------------------------------
float4 DepthHGaussShader(float4 uv : TEXCOORD) : COLOR0
{
	float sum = 0;
	float data;
	float2 pos;
	float gain = 0;
	data = tex2D(DepthSampler, uv).x;	
	if(data.x == 0)
	{
		return float4(0, 0, 0, 1);
	}
	float s = sign(data);
	for(int i = 0; i < GaussCoeffCount; i++)
	{
		pos = float2(uv.x + GaussPosH[i], uv.y);
		if(pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1)
		{
			data = tex2D(DepthSampler, pos).x;
			if(data.x * s > 0)
			{
				sum += data * GaussWeightsH[i];
				gain +=	GaussWeightsH[i];
			}		
		}
	}
	return float4(sum / gain, 0, 0, 1);
}

float4 DepthVGaussShader(float4 uv : TEXCOORD) : COLOR0
{
	float sum = 0;
	float data;
	float2 pos;
	float gain = 0;
	data = tex2D(DepthSampler, uv).x;	
	if(data == 0)
	{
		return float4(0, 0, 0, 1);
	}
	float s = sign(data);
	for(int i = 0; i < GaussCoeffCount; i++)
	{
		pos = float2(uv.x, uv.y + GaussPosV[i]);
		if(pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1)
		{
			data = tex2D(DepthSampler, pos).x;
			if(data * s > 0)
			{
				sum += data * GaussWeightsV[i];
				gain +=	GaussWeightsV[i];
			}		
		}
	}
	return float4(sum / gain, 0, 0, 1);
}

float4 FusionLinesPixelShader(VertexPositionSegmentDepth input) : COLOR
{
	return input.Segment;
}

float4 FusionLinesDisplayPixelShader(VertexPositionSegmentDepth input) : COLOR
{
	return input.Depth;
}

float4 FusionOutputShader(VertexPositionWorld input) : COLOR
{
	float4 pos = input.WorldPosition;
	if(dot(SplitPlaneVector, pos) < 0) pos.w = 2;
	else pos.w = 0.5;
	return pos;
}

float4 ColoredDepthPixelShader(VertexPositionWorld input) : COLOR
{
	float d = input.WorldPosition.w / DepthLimitZ;
	return d * ShadingColor;
}

float4 ReprojectionDepthOutputShader(VertexPositionDepth input) : COLOR
{
	return float4(input.Depth, 0, 0, 1);
}

float4 FusionDisplayShader(VertexPositionWorld input) : COLOR
{
	float d = input.WorldPosition.w / DepthLimitZ;
	if(dot(SplitPlaneVector, input.WorldPosition) < 0)	return float4(d, 0, 0, 1);
	else return float4(0, d, 0, 1);
}

float4 CalibrationPixelShader(VertexPositionWorld input) : COLOR
{
	float d = input.WorldPosition.w / DepthLimitZ;
	return float4(d, 0, 0, 1);
}
float4 DualShader(float2 uv : TEXCOORD0) : COLOR0
{
	float a = tex2D(DepthSamplerA, uv).x;
	float b = tex2D(DepthSamplerB, uv).x;
	return float4(a, b, 0, 1);
} 

float4 DepthDiffShader(float2 uv : TEXCOORD0) : COLOR0
{
	float a = tex2D(DepthSamplerA, uv).x;
	float b = tex2D(DepthSamplerB, uv).x;
	if(abs(b - a) < DepthDiffMax)
	{
		return float4(b, b, b, 1);
	}
	else
	{
		return float4(1, 0, 0, 1);
	}
} 

float4 DepthAntiDistortPixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
	float2 cpos = tex2D(DepthCorrectionSampler, texCoord).xy / DepthCorrectionTextureSize;
	if(cpos.x < 0 || cpos.x > 1 || cpos.y < 0 || cpos.y > 1)
		return 0;
	else
	{
		float4 v = tex2D(DepthSampler, cpos);
		float rawDepth = v.r * 3840 + v.g * 240 + v.b * 15;
		float depth = ToDepth(rawDepth);
		if(rawDepth > 2046 || depth > DepthLimitZ || rawDepth == 0)
			return 0;
		else
			return depth;

	}

}

float4 SumPixelShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	float source = tex2D(DepthSamplerA, texCoord).x;
	float2 acc = tex2D(DepthSamplerB, texCoord).xy;
	if(source != 0)
	{
		if(acc.y == 0)
			return float4(source, 1, 0, 1);
		else 
		{
			float avg = acc.x / acc.y;
			if(abs(source - avg) < DepthAvgLimit)
				return float4(acc.x + source, acc.y + 1, 0, 1);
			else 
			{
				if(avg > source) return float4(source, 1, 0, 1);
				else return float4(acc.x, acc.y, 0, 1);
			}
		}
	}
	else
	{
		return float4(acc.x, acc.y, 0, 1);
	}
}

float4 AvgPixelShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	float2 acc = tex2D(DepthSampler, texCoord).xy;
	if(acc.y > DepthAvgCountMin)
	{
	    float depth = float4(acc.x / acc.y, 0, 0, 1);
		float4 spos = mul(FusionTransform, float4(640 * texCoord.x * depth, 480 * texCoord.y * depth, depth, 1));
		if(dot(SplitPlaneVector, spos) > 0) return float4(depth, 0, 0, 1);
		else return float4(-depth, 0, 0, 1);
	}
	else
	{
		return 0;
	}
}

//Techniques - Visualization---------------------------------------------------
technique Simple
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 SimplePixelShader();
	}
}

technique DepthVisualization
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthVisualizationPixelShader();
	}
}

technique SignedDepthVisualization
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 SignedDepthVisualizationPixelShader();
	}
}
//Techniques - Processing------------------------------------------------------
technique DepthAntiDistort
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthAntiDistortPixelShader();
	}
}

technique PerspectiveReprojection
{
	pass P0
	{
		VertexShader = compile vs_3_0 PerspectiveReprojectionVertexShader();
		PixelShader = compile ps_3_0 ReprojectionDepthOutputShader();
	}
}

technique Sum
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 SumPixelShader();
	}
}

technique Avg
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 AvgPixelShader();
	}
}

technique FusionOutput
{
	pass P0
	{
		VertexShader = compile vs_3_0 OrthographicReprojectionVertexShader();
		PixelShader = compile ps_3_0 FusionOutputShader();
	}
}

technique DepthHGauss
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthHGaussShader();
	}
}

technique DepthVGauss
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthVGaussShader();
	}
}
technique FusionLines
{
	pass P0
	{
		VertexShader = compile vs_3_0 ModelLinesVertexShader();
		PixelShader = compile ps_3_0 FusionLinesPixelShader();
	}
}

technique FusionLinesDisplay
{
	pass P0
	{
		VertexShader = compile vs_3_0 ModelLinesVertexShader();
		PixelShader = compile ps_3_0 FusionLinesDisplayPixelShader();
	}
}

technique Calibration
{
	pass P0
	{
		VertexShader = compile vs_3_0 OrthographicReprojectionVertexShader();
		PixelShader = compile ps_3_0 CalibrationPixelShader();
	}
}

technique FusionDisplay
{
	pass P0
	{
		VertexShader = compile vs_3_0 OrthographicReprojectionVertexShader();
		PixelShader = compile ps_3_0 FusionDisplayShader();
	}
}

float4 PolarDisplayShader(VertexPositionWorld input) : COLOR
{
	float d = sqrt(input.WorldPosition.z*input.WorldPosition.z+input.WorldPosition.x*input.WorldPosition.x)/ClipRadiusMax ;
	//if(dot(SplitPlaneVector, input.WorldPosition) < 0)	return float4(d, 0, 0, 1);
	//else return float4(0, 0, 0, 1);
	return float4(d, 0, 0, 1);
}

float4 PolarOutputShader(VertexPositionWorld input) : COLOR
{
	float d = sqrt(input.WorldPosition.z*input.WorldPosition.z+input.WorldPosition.x*input.WorldPosition.x);
	//if(dot(SplitPlaneVector, input.WorldPosition) < 0)	return float4(d, 0, 0, 1);
	//else return float4(0, 0, 0, 1);
	return float4(d, 0, 0, 1);
}

technique PolarDisplay
{
	pass P0
	{
		VertexShader = compile vs_3_0 PolarReprojectionVertexShader();
		PixelShader = compile ps_3_0 PolarDisplayShader();
	}
}

technique PolarOutput
{
	pass P0
	{
		VertexShader = compile vs_3_0 PolarReprojectionVertexShader();
		PixelShader = compile ps_3_0 PolarOutputShader();
	}
}


float4 ColorModelPixelShader(VertexPositionColor i) : COLOR0
{
	return i.Color;
} 

technique ColorModel
{
	pass P0
	{
		VertexShader = compile vs_3_0 SimpleVertexShader();
		PixelShader = compile ps_3_0 ColorModelPixelShader();
	}
}

technique ColoredDepth
{
	pass P0
	{
		VertexShader = compile vs_3_0 OrthographicReprojectionVertexShader();
		PixelShader = compile ps_3_0 ColoredDepthPixelShader();
	}
}
technique Dual
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DualShader();
	}
}
technique DepthDiff
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthDiffShader();
	}
}


//Others-----------------------------------------------------------------------
float4 PolarAddPixelShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	float source = tex2D(DepthSamplerA, texCoord).x;
	float2 acc = tex2D(DepthSamplerB, texCoord).xy;
	if(source != 0)
	{
		return float4(acc.x + source, acc.y + 1, 0, 1);
	}
	else
	{
		return float4(acc.x, acc.y, 0, 1);
	}
}

float4 PolarAvgPixelShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	float2 acc = tex2D(DepthSampler, texCoord).xy;
	return float4(acc.x / acc.y, 0, 0, 1);
}

float4 PolarAvgDisplayPixelShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	float2 acc = tex2D(DepthSampler, texCoord).xy;
	float d = 1 - acc.x / acc.y / ClipRadiusMax;
	return float4(d, d, d, 1);
}

technique PolarAdd
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 PolarAddPixelShader();
	}
}

technique PolarAvg
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 PolarAvgPixelShader();
	}
}

technique PolarAvgDisplay
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 PolarAvgDisplayPixelShader();
	}
}