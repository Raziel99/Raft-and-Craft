// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water"
{
	Properties
	{
		_RippleSpeed("RippleSpeed", Float) = 1.5
		_BlurAmount("BlurAmount", Range( 0 , 50)) = 7.967741
		_Texture("Texture", 2D) = "white" {}
		_RippleDensity("RippleDensity", Float) = 7
		_RippleSlimness("RippleSlimness", Float) = 2
		[HDR]_RippleColor("RippleColor", Color) = (0.7184479,2.75405,3.811765,1)
		_BaseColor("BaseColor", Color) = (0,0.4117409,0.911301,1)
		_CameraSize("CameraSize", Range( 1 , 1000)) = 1
		_PlaneWorldPos("PlaneWorldPos", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _BaseColor;
		uniform float _RippleDensity;
		uniform float _RippleSpeed;
		uniform float _RippleSlimness;
		uniform float4 _RippleColor;
		uniform float3 _PlaneWorldPos;
		uniform float _CameraSize;
		uniform sampler2D _Texture;
		uniform float _BlurAmount;


		float2 voronoihash149( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi149( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash149( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float time149 = ( _RippleSpeed * _Time.y );
			float3 ase_worldPos = i.worldPos;
			float4 appendResult169 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float2 coords149 = appendResult169.xy * _RippleDensity;
			float2 id149 = 0;
			float2 uv149 = 0;
			float voroi149 = voronoi149( coords149, time149, id149, uv149, 0 );
			float4 appendResult259 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 appendResult275 = (float4(_PlaneWorldPos.x , _PlaneWorldPos.z , 0.0 , 0.0));
			float4 temp_output_238_0 = ( ( ( appendResult259 - appendResult275 ) / _CameraSize ) + float4( float3(0.5,0.5,0) , 0.0 ) );
			float4 break288 = step( temp_output_238_0 , float4( 1,1,0,0 ) );
			float4 break286 = step( float4( 0,0,1,0 ) , temp_output_238_0 );
			float4 lerpResult268 = lerp( float4( 0,0,0,0 ) , temp_output_238_0 , ( break288.x * break288.y * break286.x * break286.y ));
			float4 break263 = temp_output_238_0;
			float4 appendResult264 = (float4(break263.x , break263.y , 0.0 , 0.0));
			float temp_output_173_0 = ( _BlurAmount * 0.01 );
			float4 appendResult178 = (float4(temp_output_173_0 , 0.0 , 0.0 , 0.0));
			float4 appendResult177 = (float4(0.0 , temp_output_173_0 , 0.0 , 0.0));
			float4 appendResult176 = (float4(-temp_output_173_0 , 0.0 , 0.0 , 0.0));
			float4 appendResult175 = (float4(0.0 , -temp_output_173_0 , 0.0 , 0.0));
			float4 appendResult181 = (float4(-temp_output_173_0 , -temp_output_173_0 , 0.0 , 0.0));
			float4 appendResult180 = (float4(-temp_output_173_0 , temp_output_173_0 , 0.0 , 0.0));
			float4 appendResult179 = (float4(temp_output_173_0 , -temp_output_173_0 , 0.0 , 0.0));
			float4 appendResult182 = (float4(temp_output_173_0 , temp_output_173_0 , 0.0 , 0.0));
			o.Albedo = ( ( _BaseColor + ( pow( voroi149 , _RippleSlimness ) * _RippleColor ) ) + ( step( 0.01 , lerpResult268.x ) * ( ( tex2D( _Texture, ( appendResult264 + appendResult178 ).xy ) + tex2D( _Texture, ( appendResult264 + appendResult177 ).xy ) + tex2D( _Texture, ( appendResult264 + appendResult176 ).xy ) + tex2D( _Texture, ( appendResult264 + appendResult175 ).xy ) + tex2D( _Texture, ( appendResult264 + appendResult181 ).xy ) + tex2D( _Texture, ( appendResult264 + appendResult180 ).xy ) + tex2D( _Texture, ( appendResult264 + appendResult179 ).xy ) + tex2D( _Texture, ( appendResult264 + appendResult182 ).xy ) ) / float4( float3(2,2,2) , 0.0 ) ).r ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
1920;0;1920;1019;-1971.42;4.773682;1;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;235;-4339.824,-538.637;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;216;-4342,-334;Inherit;True;Property;_PlaneWorldPos;PlaneWorldPos;8;0;Create;True;0;0;False;0;False;0,0,0;0,0,141.1153;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;275;-4060.488,-301.0042;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;259;-4043.223,-529.2255;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;236;-3760,-384;Inherit;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;215;-3898,-109;Inherit;True;Property;_CameraSize;CameraSize;7;0;Create;True;0;0;False;0;False;1;160;1;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;237;-3536.5,-338.6;Inherit;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector3Node;240;-3533.2,19.3;Inherit;False;Constant;_Vector1;Vector 1;9;0;Create;True;0;0;False;0;False;0.5,0.5,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;172;-2776.22,1282.821;Inherit;True;Property;_BlurAmount;BlurAmount;1;0;Create;True;0;0;False;0;False;7.967741;0.1;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-2725.981,1746.878;Inherit;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;238;-3304.402,-174.7;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-2472.981,1681.878;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;263;-2892.214,282.471;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NegateNode;174;-2233.745,1902.314;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;180;-1853.182,2626.297;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;182;-1867.642,1976.386;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;181;-1884.642,2193.386;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;264;-2290.049,264.5709;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;178;-1935.244,628.8779;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StepOpNode;273;-2669.267,19.08126;Inherit;True;2;0;FLOAT4;0,0,1,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;176;-1920.782,1278.788;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;177;-1919.563,932.1725;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;175;-1937.782,1495.787;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StepOpNode;267;-2597.625,-233.5572;Inherit;True;2;0;FLOAT4;1,1,1,1;False;1;FLOAT4;1,1,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;179;-1870.182,2843.297;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;167;-706.4103,-657.9703;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;252;-1413.778,1405.685;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;253;-1433.066,1834.314;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;288;-2358.528,-147.6615;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;150;-840.1774,-489.2218;Inherit;True;Property;_RippleSpeed;RippleSpeed;0;0;Create;True;0;0;False;0;False;1.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;153;-892.629,-203.8026;Inherit;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;185;-2483.147,907.3359;Inherit;True;Property;_Texture;Texture;2;0;Create;True;0;0;False;0;False;None;f0e00bebf55a4f244aed5c4703a23404;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;250;-1466.099,790.3998;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;255;-1296.771,2811.656;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;256;-1349.092,2196.371;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;254;-1271.658,2453.786;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;248;-1550.073,428.3426;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;286;-2399.383,69.98122;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;251;-1388.665,1047.815;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;193;-825.5804,2832.502;Inherit;True;Property;_TextureSample2;Texture Sample 2;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;169;-385.3105,-542.2702;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;-606.097,-413.4572;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;194;-893.1805,1484.993;Inherit;True;Property;_TextureSample4;Texture Sample 4;3;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;198;-814.8765,1835.073;Inherit;True;Property;_TextureSample5;Texture Sample 5;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;151;-534.0541,-178.8711;Inherit;True;Property;_RippleDensity;RippleDensity;3;0;Create;True;0;0;False;0;False;7;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;197;-868.0145,1137.474;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;192;-840.0413,2182.592;Inherit;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;289;-2040.674,-190.9465;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;195;-800.4144,2484.984;Inherit;True;Property;_TextureSample3;Texture Sample 3;3;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;199;-907.6414,835.0825;Inherit;True;Property;_TextureSample6;Texture Sample 6;2;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;196;-882.4754,487.5639;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;200;-101.8994,1280.335;Inherit;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;False;0;False;2,2,2;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VoronoiNode;149;-246.5971,-375.9573;Inherit;True;0;0;1;0;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;155;-291.7817,-71.03033;Inherit;True;Property;_RippleSlimness;RippleSlimness;4;0;Create;True;0;0;False;0;False;2;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;201;-343.3215,1199.135;Inherit;True;8;8;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;268;-2019.041,-463.8921;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;154;-44.78179,-223.0304;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;156;-15.78179,105.9697;Inherit;False;Property;_RippleColor;RippleColor;5;1;[HDR];Create;True;0;0;False;0;False;0.7184479,2.75405,3.811765,1;0.2085707,0.2560842,0.4056604,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;270;333.0092,586.7772;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleDivideOpNode;202;160.8099,1113.214;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;257.2183,-117.0304;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;269;755.3434,510.2897;Inherit;True;2;0;FLOAT;0.01;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;244;662.6663,1011.54;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;158;126.2182,-432.0302;Inherit;False;Property;_BaseColor;BaseColor;6;0;Create;True;0;0;False;0;False;0,0.4117409,0.911301,1;0,0.4117409,0.911301,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;266;1664.241,653.186;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;159;1254.676,-340.5776;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;212;2216.426,457.8251;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2781.853,309.1401;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;275;0;216;1
WireConnection;275;1;216;3
WireConnection;259;0;235;1
WireConnection;259;1;235;3
WireConnection;236;0;259;0
WireConnection;236;1;275;0
WireConnection;237;0;236;0
WireConnection;237;1;215;0
WireConnection;238;0;237;0
WireConnection;238;1;240;0
WireConnection;173;0;172;0
WireConnection;173;1;171;0
WireConnection;263;0;238;0
WireConnection;174;0;173;0
WireConnection;180;0;174;0
WireConnection;180;1;173;0
WireConnection;182;0;173;0
WireConnection;182;1;173;0
WireConnection;181;0;174;0
WireConnection;181;1;174;0
WireConnection;264;0;263;0
WireConnection;264;1;263;1
WireConnection;178;0;173;0
WireConnection;273;1;238;0
WireConnection;176;0;174;0
WireConnection;177;1;173;0
WireConnection;175;1;174;0
WireConnection;267;0;238;0
WireConnection;179;0;173;0
WireConnection;179;1;174;0
WireConnection;252;0;264;0
WireConnection;252;1;175;0
WireConnection;253;0;264;0
WireConnection;253;1;182;0
WireConnection;288;0;267;0
WireConnection;250;0;264;0
WireConnection;250;1;177;0
WireConnection;255;0;264;0
WireConnection;255;1;179;0
WireConnection;256;0;264;0
WireConnection;256;1;181;0
WireConnection;254;0;264;0
WireConnection;254;1;180;0
WireConnection;248;0;264;0
WireConnection;248;1;178;0
WireConnection;286;0;273;0
WireConnection;251;0;264;0
WireConnection;251;1;176;0
WireConnection;193;0;185;0
WireConnection;193;1;255;0
WireConnection;169;0;167;1
WireConnection;169;1;167;3
WireConnection;152;0;150;0
WireConnection;152;1;153;0
WireConnection;194;0;185;0
WireConnection;194;1;252;0
WireConnection;198;0;185;0
WireConnection;198;1;253;0
WireConnection;197;0;185;0
WireConnection;197;1;251;0
WireConnection;192;0;185;0
WireConnection;192;1;256;0
WireConnection;289;0;288;0
WireConnection;289;1;288;1
WireConnection;289;2;286;0
WireConnection;289;3;286;1
WireConnection;195;0;185;0
WireConnection;195;1;254;0
WireConnection;199;0;185;0
WireConnection;199;1;250;0
WireConnection;196;0;185;0
WireConnection;196;1;248;0
WireConnection;149;0;169;0
WireConnection;149;1;152;0
WireConnection;149;2;151;0
WireConnection;201;0;196;0
WireConnection;201;1;199;0
WireConnection;201;2;197;0
WireConnection;201;3;194;0
WireConnection;201;4;192;0
WireConnection;201;5;195;0
WireConnection;201;6;193;0
WireConnection;201;7;198;0
WireConnection;268;1;238;0
WireConnection;268;2;289;0
WireConnection;154;0;149;0
WireConnection;154;1;155;0
WireConnection;270;0;268;0
WireConnection;202;0;201;0
WireConnection;202;1;200;0
WireConnection;157;0;154;0
WireConnection;157;1;156;0
WireConnection;269;1;270;0
WireConnection;244;0;202;0
WireConnection;266;0;269;0
WireConnection;266;1;244;0
WireConnection;159;0;158;0
WireConnection;159;1;157;0
WireConnection;212;0;159;0
WireConnection;212;1;266;0
WireConnection;0;0;212;0
ASEEND*/
//CHKSM=207CE6F3B1DA5FDC2FA32F935DF2E64B41A05338