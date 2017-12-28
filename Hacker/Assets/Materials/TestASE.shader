// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TESTSHADER"
{
	Properties
	{
		_Mask("Mask", 2D) = "white" {}
		_specular("specular", 2D) = "white" {}
		_Float1("Float 1", Float) = 10
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Mask;
		uniform sampler2D _specular;
		uniform float4 _specular_ST;
		uniform float _Float1;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (2.0).xx;
			float2 uv_TexCoord21 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner20 = ( uv_TexCoord21 + _Time.x * temp_cast_0);
			float4 temp_output_17_0 = ( tex2D( _Mask, panner20 ) * float4(0,0.751724,1,0) );
			o.Albedo = temp_output_17_0.rgb;
			o.Emission = temp_output_17_0.rgb;
			float2 uv_specular = i.uv_texcoord * _specular_ST.xy + _specular_ST.zw;
			o.Metallic = tex2D( _specular, uv_specular ).r;
			o.Smoothness = _Float1;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
2189;507;1280;651;853.7071;191.4115;1;True;True
Node;AmplifyShaderEditor.TimeNode;22;-1012.661,-188.8604;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;23;-977.6606,-288.8604;Float;False;Constant;_Float0;Float 0;5;0;Create;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1027.06,-437.6949;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;20;-730.9342,-359.7919;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;18;-474.4542,-231.3385;Float;False;Constant;_Color0;Color 0;5;0;Create;0,0.751724,1,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-503.8679,-490.0469;Float;True;Property;_Mask;Mask;1;0;Create;None;61c0b9c0523734e0e91bc6043c72a490;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-182.2745,388.2509;Float;False;Property;_Float1;Float 1;5;0;Create;10;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-558.5974,332.428;Float;True;Property;_specular;specular;3;0;Create;None;0688235f1fee46b4581bcc1cf189cf3a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-516.5434,840.8023;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;None;cd460ee4ac5c1e746b7a734cc7cc64dd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-596.0939,122.0674;Float;True;Property;_normal;normal;4;0;Create;None;b3d940e75e1f5d24684cd93a2758e1bf;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;4.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-162.7506,-234.7107;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-618.0123,-82.9072;Float;True;Property;_Albedo;Albedo;0;0;Create;None;9fbef4b79ca3b784ba023cb1331520d5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1.058509,-1.058509;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;TESTSHADER;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;21;0
WireConnection;20;2;23;0
WireConnection;20;1;22;1
WireConnection;9;1;20;0
WireConnection;17;0;9;0
WireConnection;17;1;18;0
WireConnection;0;0;17;0
WireConnection;0;2;17;0
WireConnection;0;3;8;0
WireConnection;0;4;24;0
ASEEND*/
//CHKSM=6269AC517525F6035D1204BCEC55A01321548689