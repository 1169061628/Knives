//创建人：王静静   版  权:(C) 深圳冰川网络技术有限公司

#ifndef EffectDissolveForwardPass
#define EffectDissolveForwardPass

#include "UnityCG.cginc"
#include "EffectDissolveInput.cginc"

UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

#if defined(_DISSOLVEWORLDPOS_SIMPLE) || defined(_DISSOLVEWORLDPOS_DATA) 
	float4x4 Scale (float4 scale)
	{
		return float4x4(scale.x,0.0,0.0,0.0,
						0.0,scale.y,0.0,0.0,
						0.0,0.0,scale.z,0.0,
						0.0,0.0,0.0,1.0);
	}


	float4x4 Rotation(float4 rotation)
	{
		float radX = radians(rotation.x);
		float radY = radians(rotation.y);
		float radZ = radians(rotation.z);
		
		float sinX = sin(radX);
		float cosX = cos(radX);
		float sinY = sin(radY);
		float cosY = cos(radY);
		float sinZ = sin(radZ);
		float cosZ = cos(radZ);
		
		return float4x4(cosY * cosZ, -cosY * sinZ, sinY, 0.0,
					cosX * sinZ + sinX * sinY * cosZ, cosX * cosZ - sinX * sinY * sinZ, -sinX * cosY, 0.0,
					sinX * sinZ - cosX * sinY * cosZ, sinX * cosZ + cosX * sinY * sinZ, cosX * cosY, 0.0,
					0.0, 0.0, 0.0, 1.0);
	}
#endif

VertexOutput vertForwardSimple(VertexInput v)
{
	VertexOutput o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_OUTPUT(VertexOutput, o);

	o.vertexColor = saturate(v.vertexColor);
	o.texcoord0.xy = v.texcoord0.xy;
	o.center = v.center;
	
	#if defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA) 
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.xy = v.texcoord1.xy;
		o.texcoord1.zw = v.texcoord1.zw;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.xy = v.texcoord1.xy;
		o.texcoord1.z = v.texcoord1.z;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && defined(_DISTORTION_UV_DATA)
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.xy = v.texcoord1.xy;
		o.texcoord1.z = v.texcoord1.z;
	#elif defined(_UVANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA)
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.xy = v.texcoord1.xy;
	#elif defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA)
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.xy = v.texcoord1.xy;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA)
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.xy = v.texcoord1.xy;
	#elif defined(_UVANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.x = v.texcoord1.x;
	#elif defined(_UVANIM_DATA) && defined(_DISTORTION_UV_DATA)
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.x = v.texcoord1.x;
	#elif defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) 
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.x = v.texcoord1.x;
	#elif defined(_UVMASKANIM_DATA) && defined(_DISTORTION_UV_DATA)
		o.texcoord0.zw = v.texcoord0.zw;
		o.texcoord1.x = v.texcoord1.x;
	#elif (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA) 
		o.texcoord0.zw = v.texcoord0.zw;
	#elif defined(_UVANIM_DATA) || (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) || defined(_DISTORTION_UV_DATA) || defined(_UVMASKANIM_DATA)
		#if defined(_UVANIM_DATA)
			o.texcoord0.zw = v.texcoord0.zw;
		#elif defined(_UVMASKANIM_DATA)
			o.texcoord0.zw = v.texcoord0.zw;
		#elif (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) 
			o.texcoord0.z = v.texcoord0.z;
		#elif defined(_DISTORTION_UV_DATA)  
			o.texcoord0.z = v.texcoord0.z;
		#endif
	#endif

	
	//----------------Dissolve world----------------
	#if defined(_DISSOLVEWORLDPOS_SIMPLE) || defined(_DISSOLVEWORLDPOS_DATA)
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		#if defined(_DISSOLVEWORLDPOS_DATA)
			float3 rootPos = v.center;
			float3 pos = (worldPos - rootPos )/_DissolveAmount;
		#else
			float3 rootPos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
			float3 pos = (worldPos - rootPos )/_DissolveAmount;
			pos = mul(Rotation(half4(pos,1.0)),v.vertex);
		#endif


		float posOffset = dot(_DissolveDirection, pos);
		o.worldFactor = posOffset;
		
	#endif

	
	o.vertex = UnityObjectToClipPos(v.vertex);

	return o;
}

half4 fragForwardSimple(VertexOutput IN) : SV_Target
{
	UNITY_SETUP_INSTANCE_ID(IN);
	// return half4(IN.worldFactor,1.0);

	#if defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA) || defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA) 
		float2 uvAnimData = IN.texcoord0.zw;
		float2 uvMaskAnimData = IN.texcoord1.xy;
		float dissolveData = IN.texcoord1.z;
		float distortionData = IN.texcoord1.w;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA))
		float2 uvAnimData = IN.texcoord0.zw;
		float2 uvMaskAnimData = IN.texcoord1.xy;
		float dissolveData = IN.texcoord1.z;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA) && defined(_DISTORTION_UV_DATA)
		float2 uvAnimData = IN.texcoord0.zw;
		float2 uvMaskAnimData = IN.texcoord1.xy;
		float distortionData = IN.texcoord1.z;
	#elif defined(_UVANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA)
		float2 uvAnimData = IN.texcoord0.zw;
		float dissolveData = IN.texcoord1.x;
		float distortionData = IN.texcoord1.y;
	#elif defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA)
		float2 uvMaskAnimData = IN.texcoord0.zw;
		float dissolveData = IN.texcoord1.x;
		float distortionData = IN.texcoord1.y;
	#elif defined(_UVANIM_DATA) && defined(_UVMASKANIM_DATA)
		float2 uvAnimData = IN.texcoord0.zw;
		float2 uvMaskAnimData = IN.texcoord1.xy;
	#elif defined(_UVANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA) || defined(_DISSOLVEWORLDPOS_DATA))
		float2 uvAnimData = IN.texcoord0.zw;
		float dissolveData = IN.texcoord1.x;
	#elif defined(_UVANIM_DATA) && defined(_DISTORTION_UV_DATA) 
		float2 uvAnimData = IN.texcoord0.zw;
		float distortionData = IN.texcoord1.x;
	#elif defined(_UVMASKANIM_DATA) && (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)|| defined(_DISSOLVEWORLDPOS_DATA)) 
		float2 uvMaskAnimData = IN.texcoord0.zw;
		float dissolveData = IN.texcoord1.x;
	#elif  defined(_UVMASKANIM_DATA) && defined(_DISTORTION_UV_DATA)
		float2 uvMaskAnimData = IN.texcoord0.zw;
		float distortionData = IN.texcoord1.x;
	#elif (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA) || defined(_DISSOLVEWORLDPOS_DATA)) && defined(_DISTORTION_UV_DATA) 
		float dissolveData = IN.texcoord0.z;
		float distortionData = IN.texcoord0.w;
	#elif defined(_UVANIM_DATA) || (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA) || defined(_DISSOLVEWORLDPOS_DATA)) || defined(_DISTORTION_UV_DATA) || defined(_UVMASKANIM_DATA) 
		#if defined(_UVANIM_DATA)
			float2 uvAnimData = IN.texcoord0.zw;
		#elif defined(_UVMASKANIM_DATA)
			float2 uvMaskAnimData = IN.texcoord0.zw;
		#elif (defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)) || defined(_DISSOLVEWORLDPOS_DATA)
			float dissolveData = IN.texcoord0.z;
		#elif defined(_DISTORTION_UV_DATA)  
			float distortionData = IN.texcoord0.z;
		#endif
	#endif

	//----------------Mask----------------	
	#if defined(_MASK_ON)
		float2 _MaskMapUV = TRANSFORM_TEX(IN.texcoord0.xy, _MaskMap);
	
		#if defined(_UVMASKANIM_DEFAULT)  || defined(_UVMASKANIM_DATA)
			#if defined(_UVMASKANIM_DEFAULT)
				_MaskMapUV += frac(half2(_MaskU, _MaskV) * _Time.yy);
			#else
				_MaskMapUV += uvMaskAnimData;
			#endif
		#endif
	
		half _MaskMapCol = tex2D(_MaskMap, _MaskMapUV).r;
	#endif

	//----------------Mask2----------------	
	#if defined(_MASK2_ON)
		float2 _MaskMapUV2 = TRANSFORM_TEX(IN.texcoord0.xy, _MaskMap2);
		_MaskMapUV2 += frac(half2(_Mask2U, _Mask2V) * _Time.yy);
		half _MaskMapCol2 = tex2D(_MaskMap2, _MaskMapUV2).r;
	#endif

	float2 _MainTexUV = IN.texcoord0.xy;

	 _MainTexUV = TRANSFORM_TEX(_MainTexUV, _MainTex);

 
	// ----------------Distortion----------------
	 #if defined(_DISTORTION_UV)  || defined(_DISTORTION_UV_DATA)
	 	float2 _DistortionMapUV = TRANSFORM_TEX(IN.texcoord0.xy, _DistortionMap);
	 	float2 _DistortionMapUV1 = _DistortionMapUV + frac(_Time.xz * _DistortionSpeedX * 0.1);
	 	float2 _DistortionMapUV2 = _DistortionMapUV + frac(_Time.yx * _DistortionSpeedY * 0.1);
	
	 	half offsetColor1 = tex2D(_DistortionMap, _DistortionMapUV1).r;
	 	half offsetColor2 = tex2D(_DistortionMap, _DistortionMapUV2).r;
	 	half offsetFact = (offsetColor1.r + offsetColor2.r) - 1;
	 	float2 offset = float2(offsetFact, offsetFact) * half2(_DistortionStrengthX, _DistortionStrengthY) * 0.1 * _Color.a;

		#if defined(_DISTORTION_UV_DATA)
			offset *= distortionData;
		#endif
	
	 	_MainTexUV += offset;
	 #endif
	
	//----------------UV Anima----------------
	#if defined(_UVANIM_DEFAULT)  || defined(_UVANIM_DATA)
		#if defined(_UVANIM_DEFAULT)
			_MainTexUV.xy += frac(float2(_SpeedU, _SpeedV) * _Time.yy);
		#else
			_MainTexUV.xy += uvAnimData;
		#endif
	
		_MainTexUV.xy = _MainTexUV.xy - float2(0.5, 0.5);
		// float2 rotate = float2(cos(((( _RotateAngle / 360.0 ) * UNITY_PI ) * 2.0 ) + _RotateSpeed * (_Time.y + 1000000 )),
		// 					   sin(( ( ( _RotateAngle / 360.0 ) * UNITY_PI ) * 2.0 ) + _RotateSpeed * (_Time.y + 1000000) ));
		float2 rotate = float2(cos(((( _RotateAngle / 360.0 ) * UNITY_PI ) * 2.0 ) + _RotateSpeed * fmod(_Time.y ,6000) ),
			                   sin(( ( ( _RotateAngle / 360.0 ) * UNITY_PI ) * 2.0 ) + _RotateSpeed * fmod(_Time.y  ,6000)));
		_MainTexUV.xy = float2(_MainTexUV.x * rotate.x - _MainTexUV.y * rotate.y, _MainTexUV.x * rotate.y + _MainTexUV.y * rotate.x);
		_MainTexUV.xy += float2(0.5, 0.5);

		_MainTexUV = _ClampUV * clamp(_MainTexUV.xy,0, 1) + (_MainTexUV.xy  - _ClampUV * _MainTexUV.xy);
	
	#endif

	half4 mainCol = tex2D(_MainTex, _MainTexUV);

	// half4 col = mainCol;

	mainCol = _AlphaChannel * half4(1, 1, 1, mainCol.r) + (mainCol  - _AlphaChannel * mainCol);

	half4 col = mainCol * _Color;
	col.rgb *= _Brightness;

	//----------------Dissolve 1----------------		
	#if defined(_DISSOLVE_SIMPLE) || defined(_DISSOLVE_EDGE) ||  defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA)
		
		#if defined(_DISSOLVE_SIMPLE_DATA) || defined(_DISSOLVE_EDGE_DATA) || defined(_DISSOLVEWORLDPOS_DATA)
			half dissolveLevel = dissolveData;
		#elif defined(_DISSOLVEWORLDPOS_SIMPLE)
			half dissolveLevel = _DissolveLevelW;
		#else
			half dissolveLevel = _DissolveLevel;
		#endif

		//----------------FlowMap----------------
		#if defined(_FLOWMAP)
			half2 FlowMap = tex2D( _FlowMapTex, _MainTexUV ).rg;
			_MainTexUV = lerp( _MainTexUV  , FlowMap , dissolveLevel);
			mainCol = tex2D( _MainTex, _MainTexUV );
			float2 _DissolveMapUV = TRANSFORM_TEX(IN.texcoord0.xy, _DissolveMap);
			_DissolveMapUV = _DissolveMapUV + frac(float2(_DissolveSpeedU, _DissolveSpeedV) * _Time.yy);
			_DissolveMapUV = lerp((_DissolveMapUV *0.5+0.5 ), FlowMap , dissolveLevel);
		#else
			float2 _DissolveMapUV = TRANSFORM_TEX(IN.texcoord0.xy, _DissolveMap);
			_DissolveMapUV = _DissolveMapUV + frac(float2(_DissolveSpeedU, _DissolveSpeedV) * _Time.yy);
		#endif

		//----------------Dissolve world----------------
		#if defined(_DISSOLVEWORLDPOS_SIMPLE) || defined(_DISSOLVEWORLDPOS_DATA)
			float posW = IN.worldFactor - dissolveLevel ;
			half dissove = tex2D(_DissolveMap, _DissolveMapUV).r  - posW;
		#else
			half dissove = tex2D(_DissolveMap, _DissolveMapUV).r;
	
		#endif

		float edge = clamp(dissove + 1.0 + (dissolveLevel * -2.0), 0.0, 1.0) ;
		float dissolveAlpha = smoothstep((1.0 - _DissolveHardness), _DissolveHardness, edge);

		#if defined(_FLOWMAP)
			col.rgb = mainCol.rgb * dissolveAlpha * _Color *_Brightness ;
			col.a =  dissolveAlpha * mainCol.a ;
		#else
			col.a *= dissolveAlpha;
		#endif
	#endif

	//----------------Mask----------------	
	#if defined(_MASK_ON) &&  defined(_MASK2_ON) 
		col.a *= _MaskMapCol.r * _MaskMapCol2.r;
	#elif defined(_MASK_ON) 
		col.a *= _MaskMapCol.r;
	#elif defined(_MASK2_ON)
		col.a *= _MaskMapCol2.r;
	#endif

	
	//----------------AlphaTest----------------
	#if defined(_ALPHATEST_ON)
		clip(col.a - _Cutoff);
	#endif
	
	//----------------Dissolve 2----------------		
	#if defined(_DISSOLVE_EDGE) || defined(_DISSOLVE_EDGE_DATA)
		float edgearound = clamp(dissove + 1.0 + (dissolveLevel * -2.0), 0.0, 1.0);
		edgearound = smoothstep((1.0 - _EdgeWidth), _EdgeWidth, edgearound);
		col.rgb = lerp(_EdgeColor.rgb * _EdgeIntensity, col.rgb, edgearound);
	#endif


	col.rgba *= IN.vertexColor;
	col.rgb = lerp(col.rgb, col.rgb * col.a, step(1, _DstBlend)) ;
	return col;
}

#endif