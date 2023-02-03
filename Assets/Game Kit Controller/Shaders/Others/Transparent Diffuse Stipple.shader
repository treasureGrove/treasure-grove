Shader "Custom/Transparent Diffuse Stipple" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" { }
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_BumpMap ("Normalmap", 2D) = "bump" {}

		_TransparentAmount("Transparent Amount", Float) = 1
		_StippleSize("Stipple Size", Float) = 1
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		float _TransparentAmount;

		sampler2D _MainTex;

		fixed4 _Color;

		sampler2D _BumpMap;

		float _StippleSize;

		half _Glossiness;
		half _Metallic;

		static const float4x4 kThresholdMatrix = {
			 1.0 / 17.0,	 9.0 / 17.0,	 3.0 / 17.0,	11.0 / 17.0,
			13.0 / 17.0,	 5.0 / 17.0,	15.0 / 17.0,	 7.0 / 17.0,
			 4.0 / 17.0,	12.0 / 17.0,	 2.0 / 17.0,	10.0 / 17.0,
			16.0 / 17.0,	 8.0 / 17.0,	14.0 / 17.0,	 6.0 / 17.0
		};

		static const float4x4 kKernelMatrix = {
			1, 0, 0, 0, 
			0, 1, 0, 0, 
			0, 0, 1, 0, 
			0, 0, 0, 1 
		};

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldPos;
			  float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex)* _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));

			// Unproject the screen pixel coordiante
			float2 pos = IN.screenPos.xy / IN.screenPos.w;
			pos *= _ScreenParams.xy * _StippleSize;

			// Clip pixel within [start, end] distance from camera
			float interpDist = _TransparentAmount;

			clip(interpDist - kThresholdMatrix[fmod(pos.x, 4)] * kKernelMatrix[fmod(pos.y, 4)]);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
