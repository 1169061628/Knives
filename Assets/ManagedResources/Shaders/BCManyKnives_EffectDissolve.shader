/*******************************************************************
**文件名:EffectDissolve.shader
**版权:(C)深圳冰川网络技术有限公司
**创建人:王静静
**日期:2022/08/23
**版本:1.0
**描述:特效 溶解+UV扭曲 shader
**应用:
****************************修改记录******************************
**修改人:
**日期:
**描述:
********************************************************************/ 

Shader "CasualGame/BcManyknives/Effects/Dissolve" 
{
	Properties
	{	
		
		[Enum(Off,0,Front,1,Back,2)] _Cull("Cull Mode", Float) = 2 
		[Enum(One,1,SrcAlpha,5)] _SrcBlend("Source Blend Mode", Float) = 5 
		[Enum(Zero,0,One,1,OneMinusSrcAlpha,10)] _DstBlend("Dest Blend Mode", Float) = 10  
		[HideInInspector] _BlendOp("BlendOp Mode", Float) = 0 
		
		[Toggle] _AlphaTest("Enable Cutoff", Float) = 0.0
		_Cutoff("Alpha cutoff", Range(0,1.01)) = 0.5 
		
		[Toggle(_CLAMPUV_ON)] _ClampUV("Clamp UV", Float) = 0
		
		[Toggle(_ALPHACHANNEL_R)] _AlphaChannel("Alpha Channel", Float) = 0
		
		_MainTex("Main Texture", 2D) = "white" {}      
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Brightness("Brightness", Float) = 1 
		
		[KeywordEnum(DEFAULT,DATA)]_UVAnim("Enable UVAnim", Float) = 0
		_SpeedU("UVAnim Speed U", Float) = 0 
		_SpeedV("UVAnim Speed V", Float) = 0
		_RotateAngle("UVAnim Rotate Angle", Range(0 , 360)) = 0
		_RotateSpeed("UVAnim Rotate Speed", Float) = 0
		 
//		[Toggle]_Mask("Enable Mask", Float) = 0
		_MaskMap("Mask Map (R)", 2D) = "white" {}
		[KeywordEnum(DEFAULT,DATA)]_UVMaskAnim("Enable UVMaskAnim", Float) = 0
		_MaskU("U_Mask", Float) =  0 
		_MaskV("V_Mask", Float) = 0
		
		[Toggle]_Mask2("Enable Mask 2", Float) = 0
		_MaskMap2("Mask Map 2 (R)", 2D) = "white" {}
		_Mask2U("U_Mask 2", Float) =  0 
		_Mask2V("V_Mask 2", Float) = 0

		[KeywordEnum(NONE,SIMPLE,EDGE,SIMPLE_DATA,EDGE_DATA)] _Dissolve("Dissolve Mode(不能与其它溶解同时使用)", Float) = 0
		_DissolveMap("Dissolve Map (R)", 2D) = "white" {}    
		_DissolveSpeedU("Dissolve Speed U", Float) = 0  
		_DissolveSpeedV("Dissolve Speed V", Float) = 0      
		_DissolveLevel("Dissolve Level", Range(0,1.01)) = 0 
		_DissolveHardness("Hardness", Range(0.01 , 1)) = 0.51
		
		[KeywordEnum(NONE,SIMPLE,DATA)] _DissolveWorldPos("Dissolve WorldPos(不能与其它溶解同时使用)", Float) = 0
		_DissolveLevelW("Dissolve Level W", Float) = 0
		_DissolveAmount("DissolveAmount", Range(0.1 , 2)) = 0.5
		_DissolveDirection("DissolveDirection", Vector) = (0,1,0,0)
		
		[HDR]_EdgeColor("Edge Color", Color) = (1,1,1,1)      
		_EdgeIntensity("Edge Intensity", Range(0,10)) = 1
		_EdgeWidth("Edge Width", Range(0.0,1)) = 0.6 
		
		[Toggle(_FLOWMAP)] _FlowMap("Enable FlowMap(主帖图带A通道)", Float) = 0
		_FlowMapTex("FlowMapTex", 2D) = "white" {}
		
		[KeywordEnum(NONE,UV,UV_DATA)] _Distortion("Distortion Mode", Float) = 0 
		_DistortionStrengthX("StrengthX", Float) = 0
		_DistortionStrengthY("StrengthY", Float) = 0
		_DistortionMap("Distortion Map (R)", 2D) = "white" {}      
		_DistortionSpeedX("Distortion SpeedX", Float) = 0
		_DistortionSpeedY("Distortion SpeedY", Float) = 0
		
		[Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 0
		
		[HideInInspector] _ColorMask ("Color Mask", Float) = 15

		_Stencil("Stencil ID", Float) = 0
		_StencilComp("Stencil Comparison", Float) = 8
		_StencilOp("Stencil Operation", Float) = 0
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_StencilWriteMask("Stencil Write Mask", Float) = 255
	}                
	SubShader                  
	{        
		Tags { "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" "PerformanceChecks" = "False" "Queue" = "Transparent"}   
		LOD 100  
			                                               
		BlendOp[_BlendOp]
		Blend[_SrcBlend][_DstBlend]    
		ZWrite[_ZWrite]    
		Cull[_Cull]    
		ColorMask [_ColorMask]

		
		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Pass
		{	   
			Tags{ "LightMode" = "ForwardBase" } 
			  
			CGPROGRAM 
			// #pragma target 2.0       
			
			#pragma shader_feature_local _ _ALPHATEST_ON
			
			#pragma shader_feature_local _UVANIM_DEFAULT _UVANIM_DATA

			#pragma shader_feature_local _UVMASKANIM_DEFAULT _UVMASKANIM_DATA
			   
			#pragma shader_feature_local _ _DISSOLVE_SIMPLE _DISSOLVE_EDGE _DISSOLVE_SIMPLE_DATA _DISSOLVE_EDGE_DATA 

			#pragma shader_feature_local _ _FLOWMAP
			
			#pragma shader_feature_local _ _DISSOLVEWORLDPOS_SIMPLE _DISSOLVEWORLDPOS_DATA
			 
			#pragma shader_feature_local _ _DISTORTION_UV _DISTORTION_UV_DATA

			#define _MASK_ON
			#pragma shader_feature_local _MASK2_ON 
		                                         
			#pragma vertex vertForwardSimple                  
			#pragma fragment fragForwardSimple                     
			
			#include "UnityUI.cginc"

			#include "EffectDissolveForwardPass.cginc"
			
			
			ENDCG 
		}                                    
	}
}
 