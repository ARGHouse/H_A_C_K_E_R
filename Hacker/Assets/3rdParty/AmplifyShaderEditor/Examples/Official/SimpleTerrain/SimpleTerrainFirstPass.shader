// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SimpleTerrainFirstPass"
{
	Properties
	{
		[Header(Four Splats First Pass Terrain)]
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Normal0("Normal0", 2D) = "white" {}
		[HideInInspector]_Normal1("Normal1", 2D) = "white" {}
		[HideInInspector]_Normal2("Normal2", 2D) = "white" {}
		[HideInInspector]_Normal3("Normal3", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		_Wetness("Wetness", Float) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-100" "SplatCount"="4" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_fog
		#pragma exclude_renderers gles 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Control;
		uniform float4 _Control_ST;
		uniform sampler2D _Normal0;
		uniform float4 _Normal0_ST;
		uniform sampler2D _Normal1;
		uniform float4 _Normal1_ST;
		uniform sampler2D _Normal2;
		uniform float4 _Normal2_ST;
		uniform sampler2D _Normal3;
		uniform float4 _Normal3_ST;
		uniform float _Smoothness0;
		uniform sampler2D _Splat0;
		uniform float4 _Splat0_ST;
		uniform float _Smoothness1;
		uniform sampler2D _Splat1;
		uniform float4 _Splat1_ST;
		uniform float _Smoothness2;
		uniform sampler2D _Splat2;
		uniform float4 _Splat2_ST;
		uniform float _Smoothness3;
		uniform sampler2D _Splat3;
		uniform float4 _Splat3_ST;
		uniform float _Metallic;
		uniform float _Wetness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float localCalculateTangents16_g616_g6 = ( 0.0 );
			v.tangent.xyz = cross ( v.normal, float3( 0, 0, 1 ) );
			v.tangent.w = -1;
			float3 temp_cast_0 = (localCalculateTangents16_g616_g6).xxx;
			v.vertex.xyz += temp_cast_0;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			float4 tex2DNode5_g6 = tex2D( _Control, uv_Control );
			float dotResult20_g6 = dot( tex2DNode5_g6 , float4(1,1,1,1) );
			float SplatWeight22_g6 = dotResult20_g6;
			float4 SplatControl26_g6 = ( tex2DNode5_g6 / ( SplatWeight22_g6 + 0.001 ) );
			float4 temp_output_59_0_g6 = SplatControl26_g6;
			float2 uv_Normal0 = i.uv_texcoord * _Normal0_ST.xy + _Normal0_ST.zw;
			float2 uv_Normal1 = i.uv_texcoord * _Normal1_ST.xy + _Normal1_ST.zw;
			float2 uv_Normal2 = i.uv_texcoord * _Normal2_ST.xy + _Normal2_ST.zw;
			float2 uv_Normal3 = i.uv_texcoord * _Normal3_ST.xy + _Normal3_ST.zw;
			float4 weightedBlendVar8_g6 = temp_output_59_0_g6;
			float4 weightedBlend8_g6 = ( weightedBlendVar8_g6.x*tex2D( _Normal0, uv_Normal0 ) + weightedBlendVar8_g6.y*tex2D( _Normal1, uv_Normal1 ) + weightedBlendVar8_g6.z*tex2D( _Normal2, uv_Normal2 ) + weightedBlendVar8_g6.w*tex2D( _Normal3, uv_Normal3 ) );
			o.Normal = UnpackNormal( weightedBlend8_g6 );
			float4 appendResult33_g6 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
			float2 uv_Splat0 = i.uv_texcoord * _Splat0_ST.xy + _Splat0_ST.zw;
			float4 appendResult36_g6 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
			float2 uv_Splat1 = i.uv_texcoord * _Splat1_ST.xy + _Splat1_ST.zw;
			float4 appendResult39_g6 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
			float2 uv_Splat2 = i.uv_texcoord * _Splat2_ST.xy + _Splat2_ST.zw;
			float4 appendResult42_g6 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
			float2 uv_Splat3 = i.uv_texcoord * _Splat3_ST.xy + _Splat3_ST.zw;
			float4 weightedBlendVar9_g6 = temp_output_59_0_g6;
			float4 weightedBlend9_g6 = ( weightedBlendVar9_g6.x*( appendResult33_g6 * tex2D( _Splat0, uv_Splat0 ) ) + weightedBlendVar9_g6.y*( appendResult36_g6 * tex2D( _Splat1, uv_Splat1 ) ) + weightedBlendVar9_g6.z*( appendResult39_g6 * tex2D( _Splat2, uv_Splat2 ) ) + weightedBlendVar9_g6.w*( appendResult42_g6 * tex2D( _Splat3, uv_Splat3 ) ) );
			float4 MixDiffuse28_g6 = weightedBlend9_g6;
			o.Albedo = MixDiffuse28_g6.xyz;
			o.Metallic = _Metallic;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			o.Smoothness = saturate( pow( ( 1.0 - ase_vertex3Pos.y ) , _Wetness ) );
			o.Alpha = 1;
		}

		ENDCG
	}

	Dependency "BaseMapShader"="ASESampleShaders/SimpleTerrainBase"
	Fallback "Nature/Terrain/Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14103
332;92;1012;766;700.0611;295.1635;1.347599;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;37;-423.6948,402.6638;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;36;-126.6948,411.1436;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-105.9117,557.5325;Float;False;Property;_Wetness;Wetness;18;0;Create;0;2.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;31;-333.7468,45.5575;Float;False;Four Splats First Pass Terrain;0;;6;37452fdfb732e1443b7e39720d05b708;6;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0.0;False;58;FLOAT;0.0;False;62;FLOAT;0.0;False;6;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;19;FLOAT;17
Node;AmplifyShaderEditor.PowerNode;38;114.0559,459.6325;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;126.1189,234.9401;Float;False;Property;_Smoothness;Smoothness;19;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;134.9534,153.83;Float;False;Property;_Metallic;Metallic;20;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;41;391.3052,383.6638;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;29;92.84039,315.6637;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;657.0565,40.11808;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ASESampleShaders/SimpleTerrainFirstPass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;-100;False;Opaque;Geometry;All;True;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Nature/Terrain/Diffuse;-1;-1;-1;-1;0;1;SplatCount=4;1;multi_compile_fog;False;1;BaseMapShader=ASESampleShaders/SimpleTerrainBase;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;0;37;2
WireConnection;38;0;36;0
WireConnection;38;1;39;0
WireConnection;41;0;38;0
WireConnection;29;0;31;17
WireConnection;0;0;31;0
WireConnection;0;1;31;14
WireConnection;0;3;21;0
WireConnection;0;4;41;0
WireConnection;0;11;29;0
ASEEND*/
//CHKSM=4B46D9EF6F2ECFC4F89549FDC9873ECABCFE1E66