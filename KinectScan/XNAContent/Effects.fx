#define Pi 3.14159
#define MaxDepth 2046
uniform extern float DepthHStep = 1. / 640;
uniform extern float DepthVStep = 1. / 480;
uniform extern float DepthHBigStep = 1. / 80;
uniform extern float DepthVBigStep = 1. / 60;
uniform extern float4x4 SimpleTransform;
uniform extern float4x4 DepthInverseIntrinsics;
uniform extern float4x4 DepthToColor;
uniform extern float NaN;
uniform extern texture MainTexture;
uniform extern texture VideoTexture;
uniform extern texture DepthTexture;
uniform extern texture VideoCorrectionTexture;
uniform extern texture DepthCorrectionTexture;	
uniform extern texture DepthNormalTexture;	
uniform extern texture ScaleTexture;
uniform extern float DepthA = -0.0233458;
uniform extern float DepthB = 354.988;
uniform extern float DepthC = 1093.09;
uniform extern float MinColoringDepth = 0;
uniform extern float2 VideoCorrectionTextureSize, DepthCorrectionTextureSize, ViewportSize;
uniform extern float TriangleRemoveLimit = 0.05;
uniform extern float2 ModelScale = float2(1,1);
uniform extern float2 Move = float2(0,0);
uniform extern float2 StereoFocus = float2(0,0);
uniform extern float Scale = 1;
uniform extern float4 DepthToIR, IR640To1280, Color640To1280;

void SpriteVertexShader(inout float4 position : POSITION0,
		  				inout float4 color    : COLOR0,
						inout float2 texCoord : TEXCOORD0)
{
	position.xy /= ViewportSize;
	position.xy *= float2(2, -2);
	position.xy -= float2(1, -1);
}

sampler MainSampler : register(s4)
{
	Texture = <MainTexture>;	
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	AddressU = Clamp;
	AddressV = Clamp;
};

sampler VideoSampler : register(s0)
{
	Texture = <VideoTexture>;	
};

sampler DepthSampler : register(s1)
{
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	Texture = <DepthTexture>;	
};

sampler DepthNormalSampler : register(s4)
{
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	Texture = <DepthNormalTexture>;	
};

sampler VideoCorrectionSampler : register(s2)
{
	Texture = <VideoCorrectionTexture>;	
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
};

sampler DepthCorrectionSampler : register(s3)
{
	magfilter = POINT; minfilter = POINT; mipfilter=POINT;
	Texture = <DepthCorrectionTexture>;	
};

sampler ScaleSampler : register(s5)
{
	magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR;
	Texture = <ScaleTexture>;	
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexPositionTexture
{
    float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

uniform extern float2 TargetSize = float2(640, 480);
VertexPositionTexture MiniPlaneVertexShader(VertexPositionTexture i)
{
	VertexPositionTexture o = i;
	o.Position.xy *= 2.;
	o.Position.xy -= float2(0.5/TargetSize.x, -0.5/TargetSize.y);
	return o;
}

struct VertexPositionTextureColor
{
    float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float4 Color : COLOR0;
};

struct VertexPositionTextureDepth
{
    float4 Position : POSITION0;
	float Depth : NORMAL1;
	float2 TextureCoordinate : TEXCOORD0;	
};

struct VertexPosition2
{
    float4 Position : POSITION0;
	float3 RealPosition : POSITION1;
	float RawDepth: NORMAL0;
};

struct VertexPositionColor
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
};

VertexPositionColor PositionColorVertexShader(VertexPositionColor input)
{
	VertexPositionColor output;
	output.Position=mul(float4(input.Position.x,input.Position.y,input.Position.z,1),SimpleTransform);
	output.Color=input.Color;
    return output;
}

float4 PositionColorPixelShader(VertexPositionColor input) : COLOR
{
	return input.Color;
}

technique PositionColor
{
    pass P0
    {
        VertexShader = compile vs_3_0 PositionColorVertexShader();
        PixelShader = compile ps_3_0 PositionColorPixelShader();
    }
}

uniform float3 DepthWeight= float3(3840, 240, 15) / 2047;
uniform float3 Depth2047Weight= float3(3840, 240, 15);

float ToDepth(float x)
{
	return DepthA - DepthB / (x - DepthC);
}

float4 IRShader(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(VideoSampler, texCoord);
	float i=(color.r*3840+color.g*240+color.b*15)/1023;
	color.r=i;
	color.g=i;
	color.b=i;
	color.a=1;
	return color;
}

technique IR
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 IRShader();
	}
}

float4 RGBShader(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(VideoSampler, texCoord);
	color.a=1;
	return color;
}

technique RGB
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 RGBShader();
	}
}

technique Simple
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 RGBShader();
	}
}

float4 HSLToRGB(float4 cl)
{
    float h=cl.x, sl=cl.y, l=cl.z;
    float v;
    float r=1.,g=1.,b=1.;
	if(h>=1)h=0.999999;
    v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
    if (v > 0.) 
    {
            float m;
            float sv;
            float fract, vsf, mid1, mid2;
            m = l + l - v;
            sv = (v - m ) / v;
            h *= 6.;
            fract = h - trunc(h);
            vsf = v * sv * fract;
            mid1 = m + vsf;
            mid2 = v - vsf;
            if(h<1.)
			{
                        r = v; 
                        g = mid1; 
                        b = m; 
            }
			else if(h<2.)
            { 
                        r = mid2; 
                        g = v; 
                        b = m; 
            }
            else if(h<3.)
			{ 
                        r = m; 
                        g = v; 
                        b = mid1; 
            }
			else if(h<4.)
            {
                        r = m; 
                        g = mid2; 
                        b = v; 
            }
			else if(h<5)
            { 
                        r = mid1; 
                        g = m; 
                        b = v; 
            }
            else
			{ 
                        r = v; 
                        g = m; 
                        b = mid2;
            }
    }
    return float4(r,g,b,cl.a);
}

uniform extern float DepthZLimit = 1.1;
uniform extern float DepthAvgCount;
uniform extern float DepthHSLColoringPeriod = 0.5;

uniform extern float4x4 ReprojectionTransform;

float3 PosOf(float x, float y)
{
	float4 s = tex2Dlod(DepthSampler, float4(x, y, 0, 0));
	if(s.y == 0 || s.x == 0)
		return float4(NaN,NaN,NaN,NaN);
	else
		return mul(DepthInverseIntrinsics, float4(640 * x * s.x, 480 * y * s.x, s.x, 1)).xyz;
}

float4 PositionOutputPixelShader(float2 uv : TEXCOORD0) : COLOR0
{
	float3 posx = PosOf(uv.x, uv.y);
	float3 posu = PosOf(uv.x, uv.y + DepthVStep);
	float3 posd = PosOf(uv.x, uv.y - DepthVStep);
	float3 posl = PosOf(uv.x - DepthHStep, uv.y);
	float3 posr = PosOf(uv.x + DepthHStep, uv.y);
	if(pow(length(posu - posd) / posx.z, 2) + pow(length(posr - posl) / posx.z, 2) > TriangleRemoveLimit)
	{			
		return float4(NaN,NaN,NaN,NaN);
	}
	return float4(posx, 1);
}

technique PositionOutputShading
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 PositionOutputPixelShader();
	}
}

float4 DepthNormalVertexShader(float2 uv : TEXCOORD0) : COLOR0
{
	float3 posu = PosOf(uv.x, uv.y + DepthVBigStep);
	float3 posd = PosOf(uv.x, uv.y - DepthVBigStep);
	float3 posl = PosOf(uv.x - DepthHBigStep, uv.y);
	float3 posr = PosOf(uv.x + DepthHBigStep, uv.y);
	float3 normal = normalize(cross(posu - posd, posr - posl));
	float i = abs(normal.z);
	return float4(i, i, i, 1.);
}

technique DepthNormalShading
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthNormalVertexShader();
	}
}


VertexPositionTextureDepth ReprojectionVertexShader(VertexPositionTexture input)
{
	VertexPositionTextureDepth output;
	output.TextureCoordinate = NaN;
	float3 posx = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y);
	float3 posu = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y + DepthVStep);
	float3 posd = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y - DepthVStep);
	float3 posl = PosOf(input.TextureCoordinate.x - DepthHStep, input.TextureCoordinate.y);
	float3 posr = PosOf(input.TextureCoordinate.x + DepthHStep, input.TextureCoordinate.y);
	if(pow(length(posu - posd) / posx.z, 2) + pow(length(posr - posl) / posx.z, 2) > TriangleRemoveLimit)
	{		
		output.Position = NaN;
		output.Depth = NaN;		
		return output;
	}

	float2 uv = input.TextureCoordinate;
	float4 source = tex2Dlod(DepthSampler, float4(uv.x, uv.y, 0, 0));
	float depth = source.x;
	float4 startpos, pos, tpos;
	float reprojectedDepth;
	if (source.y == 0 || depth == 0)
	{
		pos = NaN;
	}
	else
	{
		startpos = float4(640 * uv.x * depth, 480 * uv.y * depth, depth, 1);
		pos = mul(ReprojectionTransform, startpos);
	    pos.x = (pos.x / pos.z / 640 * 2 - 1) * DepthZLimit * ModelScale.x;
		pos.y = (-pos.y / pos.z / 480 * 2 + 1) * DepthZLimit * ModelScale.y;
		pos.xy = (pos.xy + Move) * Scale + StereoFocus;
		pos.w = DepthZLimit;
		tpos = mul(DepthToColor, startpos); 
		tpos.xyz /= tpos.z;
		tpos.x /= 640;
		tpos.y /= 480;
		output.TextureCoordinate = tpos.xy;
	}
	output.Position = pos;
	output.Position.z/=10;
	output.Depth = pos.z;
    return output;
}

float4 ReprojectionAntiDistortTexturePixelShader(VertexPositionTextureDepth input) : COLOR
{
	float2 cpos = tex2D(VideoCorrectionSampler, input.TextureCoordinate).xy / VideoCorrectionTextureSize;
	return tex2D(VideoSampler, cpos);
}

float4 ReprojectionTexturePixelShader(VertexPositionTextureDepth input) : COLOR
{
	return tex2D(VideoSampler, input.TextureCoordinate);
}

float4 ReprojectionRainbowPixelShader(VertexPositionTextureDepth input) : COLOR
{
	return HSLToRGB(float4(input.Depth % DepthHSLColoringPeriod / DepthHSLColoringPeriod, 1, 0.5, 1));
}

float4 ReprojectionRainbowShadowPixelShader(VertexPositionTextureDepth input) : COLOR
{
	float shade = tex2D(DepthNormalSampler, input.TextureCoordinate).x;
	if(isnan(shade)||shade<0||shade>1)
		return HSLToRGB(float4(input.Depth % DepthHSLColoringPeriod / DepthHSLColoringPeriod, 1, 0.5, 1));
	else
		return HSLToRGB(float4(input.Depth % DepthHSLColoringPeriod / DepthHSLColoringPeriod, 1, 0.25+shade/4, 1));
}

float4 ReprojectionZebraPixelShader(VertexPositionTextureDepth input) : COLOR
{
	float i = sin(input.Depth / DepthHSLColoringPeriod * 2 * Pi) / 2. + 0.5;
	return float4(i, i, i, 1.);
}

float4 ReprojectionScalePixelShader(VertexPositionTextureDepth input) : COLOR
{
	return tex2D(ScaleSampler, float2((input.Depth - MinColoringDepth) / (DepthZLimit - MinColoringDepth), 0));
}

float4 ReprojectionDepthOutputShader(VertexPositionTextureDepth input) : COLOR
{
	return float4(input.Depth / DepthZLimit, 0, 0, 1);
}

VertexPosition2 ReprojectionOutputVertexShader(VertexPositionTexture input)
{
	VertexPosition2 output;
	float3 posx = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y);
	float3 posu = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y + DepthVStep);
	float3 posd = PosOf(input.TextureCoordinate.x, input.TextureCoordinate.y - DepthVStep);
	float3 posl = PosOf(input.TextureCoordinate.x - DepthHStep, input.TextureCoordinate.y);
	float3 posr = PosOf(input.TextureCoordinate.x + DepthHStep, input.TextureCoordinate.y);
	if(pow(length(posu - posd) / posx.z, 2) + pow(length(posr - posl) / posx.z, 2) > TriangleRemoveLimit)
	{		
		output.Position = NaN;
		output.RealPosition = NaN;		
		output.RawDepth = NaN;
		return output;
	}

	float2 uv = input.TextureCoordinate;
	float4 source = tex2Dlod(DepthSampler, float4(uv.x, uv.y, 0, 0));
	float depth = source.x;
	float4 startpos, pos;
	float3 rpos;
	float reprojectedDepth;
	if (source.y == 0)
	{
		pos = NaN;
		rpos = NaN;
	}
	else
	{
		startpos = float4(640 * uv.x * depth, 480 * uv.y * depth, depth, 1);
		pos = mul(ReprojectionTransform, startpos);
	    pos.x = (pos.x / pos.z / 640 * 2 - 1) * DepthZLimit * ModelScale.x;
		pos.y = (-pos.y / pos.z / 480 * 2 + 1) * DepthZLimit * ModelScale.y;
		pos.w = DepthZLimit;		
		rpos = posx;
	}
    output.Position = pos;
	output.RealPosition = rpos;
	output.RawDepth = -DepthB/(tex2Dlod(DepthSampler, float4(input.TextureCoordinate.x, input.TextureCoordinate.y, 0, 0)).x - DepthA)+DepthC;
	return output;
}

float4 ReprojectionOutputPixelShader(VertexPosition2 input) : COLOR
{
	return float4(input.RealPosition, input.RawDepth);
}

uniform extern float4x4 ReprojectionModelTransform;
VertexPositionTextureDepth ReprojectionModelVertexShader(VertexPositionTexture input)
{
	VertexPositionTextureDepth output;
	output.TextureCoordinate = input.TextureCoordinate;
	float4 startpos=input.Position;
	startpos.w=1;
	float4 pos = mul(ReprojectionModelTransform, startpos);
	pos.x = (pos.x / pos.z / 640 * 2 - 1) * DepthZLimit * ModelScale.x;
	pos.y = (-pos.y / pos.z / 480 * 2 + 1) * DepthZLimit * ModelScale.y;
	pos.xy = (pos.xy + Move) * Scale;
	pos.w = DepthZLimit;
    output.Position = pos;
	output.Depth = pos.z;
    return output;
}

technique DepthReprojection
{
	pass P0
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 ReprojectionAntiDistortTexturePixelShader();
	}
	pass P1
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 ReprojectionRainbowPixelShader();
	}
	pass P2
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 ReprojectionRainbowShadowPixelShader();
	}
	pass P3
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 ReprojectionZebraPixelShader();
	}
	pass P4
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 ReprojectionScalePixelShader();
	}
}

technique DepthReprojectionOutput
{
	pass P0
	{
		VertexShader = compile vs_3_0 ReprojectionOutputVertexShader();
		PixelShader = compile ps_3_0 ReprojectionOutputPixelShader();
	}
}

technique ModelReprojection
{
	pass P0
	{
		VertexShader = compile vs_3_0 ReprojectionModelVertexShader();
		PixelShader = compile ps_3_0 ReprojectionTexturePixelShader();
	}
	pass P1
	{
		VertexShader = compile vs_3_0 ReprojectionModelVertexShader();
		PixelShader = compile ps_3_0 ReprojectionRainbowPixelShader();
	}
	pass P2
	{
		VertexShader = compile vs_3_0 ReprojectionModelVertexShader();
		PixelShader = compile ps_3_0 ReprojectionRainbowShadowPixelShader();
	}
	pass P3
	{
		VertexShader = compile vs_3_0 ReprojectionModelVertexShader();
		PixelShader = compile ps_3_0 ReprojectionZebraPixelShader();
	}
	pass P4
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 ReprojectionScalePixelShader();
	}
}

uniform extern texture DepthTextureA, DepthTextureB;
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
float4 DepthAddShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	float4 source = tex2D(DepthSamplerB, texCoord);
	float4 target = tex2D(DepthSamplerA, texCoord);
	float rawDepth = source.r*3840+source.g*240+source.b*15;
	float depth = ToDepth(rawDepth);
	if (rawDepth > 2046 || depth > DepthZLimit || rawDepth == 0)
	{
		return target;
	}
	else
	{
		return float4(depth, 1, 0, 1) + target;
	}
}

uniform extern float2 DepthScale = float2(1, 1), DepthDisp = float2(0, 0);
float4 DepthAntiDistortAndAverageShader(float2 texCoord: TEXCOORD0) : COLOR
{
	float2 cpos = (tex2D(DepthCorrectionSampler, texCoord).xy*DepthScale+DepthDisp) / DepthCorrectionTextureSize;
	if(cpos.x < 0 || cpos.x > 1 || cpos.y < 0 || cpos.y > 1)
		return float4(0, 0, 0, 1);
	else
	{
		float4 v = tex2D(DepthSamplerA, cpos);
		if(v.y < 1)
		{
			return float4(0, 0, 0, 1);
		}
		else
		{
			return float4(v.x / v.y, 1, 0, 1);
		}
	}
}

float4 DepthAverageShader(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 v = tex2D(DepthSamplerA, texCoord);
	if(v.y < 1)
	{
		return float4(0, 0, 0, 1);
	}
	else
	{
		return float4(v.x / v.y, 1, 0, 1);
	}
}

float4 DepthDisplayShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	float3 source = tex2D(DepthSampler, texCoord);
	float i = source.x % DepthHSLColoringPeriod / DepthHSLColoringPeriod;
	if(source.y == 0)
	{
		return 1;
	}
	else
	{
		return HSLToRGB(float4(i, 1, 0.5, 1));
	}
}

technique DepthAdd
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthAddShader();
	}
}

technique DepthAntiDistortAndAverage
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthAntiDistortAndAverageShader();
	}
}

technique DepthAverage
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthAverageShader();
	}
}

technique DepthDisplay
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthDisplayShader();
	}
}

#define CoeffCount 9
uniform extern float GaussWeightsH[CoeffCount], GaussPosH[CoeffCount], GaussWeightsV[CoeffCount], GaussPosV[CoeffCount];
uniform extern float NormalGaussWeightsH[CoeffCount], NormalGaussPosH[CoeffCount], NormalGaussWeightsV[CoeffCount], NormalGaussPosV[CoeffCount];
float4 DepthHGaussShader(float4 uv : TEXCOORD) : COLOR0
{
	float4 sum = float4(0, 0, 0, 0);
	float4 data;
	float2 pos;
	float gain = 0;
	data = tex2D(DepthSamplerA, uv);
	if(data.y == 0)
	{
		return float4(0, 0, 0, 1);
	}
	for(int i = 0; i < CoeffCount; i++)
	{
		pos = float2(uv.x + GaussPosH[i], uv.y);
		if(pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1)
		{
			data = tex2D(DepthSamplerA, pos);
			if(data.y != 0)
			{
				sum += data * GaussWeightsH[i];
				gain +=	GaussWeightsH[i];
			}		
		}
	}
	return sum / gain;
}

technique DepthHGauss
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthHGaussShader();
	}
}

float4 DepthVGaussShader(float4 uv : TEXCOORD) : COLOR0
{
	float4 sum = float4(0, 0, 0, 0);
	float4 data;
	float2 pos;
	float gain = 0;
	data = tex2D(DepthSamplerA, uv);
	if(data.y == 0)
	{
		return float4(0, 0, 0, 1);
	}
	for(int i = 0; i < CoeffCount; i++)
	{
		pos = float2(uv.x, uv.y + GaussPosV[i]);
		if(pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1)
		{
			data = tex2D(DepthSamplerA, pos);
			if(data.y != 0)
			{
				sum += data * GaussWeightsV[i];
				gain +=	GaussWeightsV[i];
			}		
		}
	}
	return sum / gain;
}

technique DepthVGauss
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DepthVGaussShader();
	}
}


float4 GraphShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	
	float c = tex2D(MainSampler, texCoord).x;

	if(1-texCoord.y > c || isnan(c))
	{
		return 0;		
	}
	else
	{
		return float4(0,0,0,0.5);
	}

}

technique Graph
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 GraphShader();
	}
}

float4 DebugShader(float2 texCoord: TEXCOORD0) : COLOR0
{
	float depth = tex2D(DepthSampler, texCoord).x;
	if(depth==0)
	return float4(1, 0, 0, 1);
	else if(depth<0.1)
	return float4(0, 1, 0, 1);
	else if(depth>1)
	return float4(1, 0, 1, 1);
	else
	return float4(depth, depth, depth, 1);
}

technique Debug
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 DebugShader();
	}
}

float4 RGBAntiDistortPixelShader(float2 uv : TEXCOORD) : COLOR
{
	float2 cpos = tex2D(VideoCorrectionSampler, uv).xy / VideoCorrectionTextureSize;
	if(cpos.x<0 || cpos.y<0 || cpos.x>1 || cpos.y>1) return 1;
	else return tex2D(VideoSampler, cpos);
}

technique RGBAntiDistort
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 RGBAntiDistortPixelShader();
	}
}

float4 IRAntiDistortPixelShader(float2 uv: TEXCOORD0) : COLOR
{
	float2 cpos = tex2D(DepthCorrectionSampler, uv).xy / DepthCorrectionTextureSize;
	float4 color = tex2D(VideoSampler, cpos);
	float i=(color.r*3840+color.g*240+color.b*15)/1023;
	color.r=i;
	color.g=i;
	color.b=i;
	color.a=1;
	return color;
}

technique IRAntiDistort
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 IRAntiDistortPixelShader();
	}
}

uniform extern float3 AnaglyphColor;

float4 ToAnaglyph(float4 c)
{
	float l = (c.r + c.g + c.b) / 3;
	return float4(AnaglyphColor * l, 1);
}

float4 AnaglyphPixelShader(float2 uv: TEXCOORD0) : COLOR
{
	return ToAnaglyph(tex2D(VideoSampler, uv));
}

technique Anaglyph
{
	pass P0
	{
		VertexShader = compile vs_3_0 MiniPlaneVertexShader();
		PixelShader = compile ps_3_0 AnaglyphPixelShader();
	}
}

float4 AnaglyphReprojectionAntiDistortTexturePixelShader(VertexPositionTextureDepth input) : COLOR
{
	return ToAnaglyph(ReprojectionAntiDistortTexturePixelShader(input));
}

float4 AnaglyphReprojectionRainbowPixelShader(VertexPositionTextureDepth input) : COLOR
{
	return ToAnaglyph(ReprojectionRainbowPixelShader(input));
}

float4 AnaglyphReprojectionRainbowShadowPixelShader(VertexPositionTextureDepth input) : COLOR
{
	return ToAnaglyph(ReprojectionRainbowShadowPixelShader(input));
}

float4 AnaglyphReprojectionZebraPixelShader(VertexPositionTextureDepth input) : COLOR
{
	return ToAnaglyph(ReprojectionZebraPixelShader(input));
}

float4 AnaglyphReprojectionScalePixelShader(VertexPositionTextureDepth input) : COLOR
{
	return ToAnaglyph(ReprojectionScalePixelShader(input));
}


technique AnaglyphDepthReprojection
{
	pass P0
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 AnaglyphReprojectionAntiDistortTexturePixelShader();
	}
	pass P1
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 AnaglyphReprojectionRainbowPixelShader();
	}
	pass P2
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 AnaglyphReprojectionRainbowShadowPixelShader();
	}
	pass P3
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 AnaglyphReprojectionZebraPixelShader();
	}
	pass P4
	{
		VertexShader = compile vs_3_0 ReprojectionVertexShader();
		PixelShader = compile ps_3_0 AnaglyphReprojectionScalePixelShader();
	}
}