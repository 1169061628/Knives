//创建人：王静静   版  权:(C) 深圳冰川网络技术有限公司

#ifndef EffectDissolveInput
#define EffectDissolveInput

//----------------main----------------
half _DstBlend;
sampler2D _MainTex;
float4 _MainTex_ST;
half4 _Color;
half _Brightness;
half _InvFade;

#if defined(_UVANIM_DEFAULT) || defined(_UVANIM_DATA)
	float _SpeedU;
	float _SpeedV;
	float _RotateAngle;
	float _RotateSpeed;
#endif
	
//----------------AlphaTest----------------
#ifdef _ALPHATEST_ON
	half _Cutoff;
#endif
			
//----------------Dissolve----------------
#if defined(_DISSOLVE_SIMPLE) || defined(_DISSOLVE_EDGE) ||  defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA) 
	sampler2D _DissolveMap;
	float4 _DissolveMap_ST;
	float _DissolveSpeedU;
	float _DissolveSpeedV;
	half _DissolveLevel;
	half _DissolveHardness;
	#if defined(_DISSOLVE_EDGE) ||defined(_DISSOLVE_EDGE_DATA)
		half4 _EdgeColor;
		half _EdgeIntensity;
		half _EdgeWidth;
	#endif
#endif

#if defined(_FLOWMAP) 
	sampler2D _FlowMapTex;
#endif

#if defined(_DISSOLVEWORLDPOS_SIMPLE) || defined(_DISSOLVEWORLDPOS_DATA) 
	float4 _DissolveDirection;
	float _DissolveAmount;
	half _DissolveLevelW;
#endif

//----------------Distortion----------------
#if defined(_DISTORTION_UV)  || defined(_DISTORTION_UV_DATA) 
	half _DistortionStrengthX;
	half _DistortionStrengthY;
	float _DistortionSpeedX;
	float _DistortionSpeedY;
	sampler2D _DistortionMap;
	float4 _DistortionMap_ST;
#endif
//----------------Mask----------------
#ifdef _MASK_ON
	sampler2D _MaskMap;
	float4 _MaskMap_ST;
	#if defined(_UVMASKANIM_DEFAULT) || defined(_UVMASKANIM_DATA)
		float _MaskU;
		float _MaskV;
	#endif
#endif

//----------------Mask2----------------
#ifdef _MASK2_ON
	sampler2D _MaskMap2;
	float4 _MaskMap2_ST;
	float _Mask2U;
	float _Mask2V;
#endif

float _ClampUV;
float _AlphaChannel;

struct VertexInput
{
	float4 vertex : POSITION;
	half4 vertexColor : COLOR;
	
	#if defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA) 
		float4 texcoord0 : TEXCOORD0;
		float4 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		float4 texcoord0 : TEXCOORD0;
		float3 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && defined(_DISTORTION_UV_DATA)
		float4 texcoord0 : TEXCOORD0;
		float3 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA)
		float4 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	#elif defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA)
		float4 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA)
		float4 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		float4 texcoord0 : TEXCOORD0;
		float texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && defined(_DISTORTION_UV_DATA)  
		float4 texcoord0 : TEXCOORD0;
		float texcoord1 : TEXCOORD1;
	#elif  defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		float4 texcoord0 : TEXCOORD0;
		float texcoord1 : TEXCOORD1;
	#elif  defined(_UVMASKANIM_DATA) && defined(_DISTORTION_UV_DATA)
		float4 texcoord0 : TEXCOORD0;
		float texcoord1 : TEXCOORD1;
	#elif (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA) 
		float4 texcoord0 : TEXCOORD0;
	#elif defined(_UVANIM_DATA) || (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) || defined(_DISTORTION_UV_DATA) || defined(_UVMASKANIM_DATA)
		#if defined(_UVANIM_DATA)
			float4 texcoord0 : TEXCOORD0;
		#elif defined(_UVMASKANIM_DATA) 
			float4 texcoord0 : TEXCOORD0;
		#elif (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) 
			float3 texcoord0 : TEXCOORD0;
		#elif defined(_DISTORTION_UV_DATA) 
			float3 texcoord0 : TEXCOORD0;
		#endif
	#else
		float2 texcoord0 : TEXCOORD0;
	#endif

	float3 center : TEXCOORD2;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput
{
	float4 vertex : SV_POSITION;
	half4 vertexColor : COLOR;

	#if defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA) 
		float4 texcoord0 : TEXCOORD0;
		float4 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		float4 texcoord0 : TEXCOORD0;
		float3 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && defined(_DISTORTION_UV_DATA)
		float4 texcoord0 : TEXCOORD0;
		float3 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA)
		float4 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	#elif defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA)
		float4 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA)
		float4 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		float4 texcoord0 : TEXCOORD0;
		float texcoord1 : TEXCOORD1;
	#elif defined(_UVANIM_DATA) && defined(_DISTORTION_UV_DATA)  
		float4 texcoord0 : TEXCOORD0;
		float texcoord1 : TEXCOORD1;
	#elif  defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		float4 texcoord0 : TEXCOORD0;
		float texcoord1 : TEXCOORD1;
	#elif  defined(_UVMASKANIM_DATA) && defined(_DISTORTION_UV_DATA)
		float4 texcoord0 : TEXCOORD0;
		float texcoord1 : TEXCOORD1;
	#elif (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA) 
		float4 texcoord0 : TEXCOORD0;
	#elif defined(_UVANIM_DATA) || (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) || defined(_DISTORTION_UV_DATA) ||  defined(_UVMASKANIM_DATA)
		#if defined(_UVANIM_DATA)
			float4 texcoord0 : TEXCOORD0;
		#elif defined(_UVMASKANIM_DATA) 
			float4 texcoord0 : TEXCOORD0;
		#elif (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) 
			float3 texcoord0 : TEXCOORD0;
		#elif defined(_DISTORTION_UV_DATA) 
			float3 texcoord0 : TEXCOORD0;
		#endif
	#else
		float2 texcoord0 : TEXCOORD0;
	#endif

	float3 center : TEXCOORD2;
		
	#if defined(_DISSOLVEWORLDPOS_SIMPLE) || defined(_DISSOLVEWORLDPOS_DATA) 
		float3 worldPos : TEXCOORD3;
		float3 worldFactor : TEXCOORD4;
	#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

#endif