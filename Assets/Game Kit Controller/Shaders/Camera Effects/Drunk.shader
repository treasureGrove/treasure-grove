Shader "Custom/Drunk" 
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_HorizontalAmount("Horizontal Amount", Float) = 1
		_VerticalAmount("Vertical Amount", Float) = 1

		_HorizontalMultiplier("Horizontal Multiplier", Float) = 1
		_VerticalMultiplier("Vertical Multiplier", Float) = 1

		_HorizontalSpeed("Horizontal Speed", Float) = 1
		_VerticalSpeed("Vertical Speed", Float) = 1

		_BlurAmount("Blur Amount", Float) = 1
		_BlurSpeed("Blur Speed", Float) = 1

		_BrightAmount("Bright Amount", Float) = 1
		_BrightMultipler("Bright Multiplier", Float) = 1

		_ReverseImageX("Reverse Image X", Float) = 1
		_ReverseImageY("Reverse Image Y", Float) = 1
	}
	Subshader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vertex_shader
			#pragma fragment pixel_shader
			#pragma target 2.0

			sampler2D _MainTex;

			float _HorizontalAmount;
			float _VerticalAmount;

			float _HorizontalMultiplier;
			float _VerticalMultiplier;

			float _HorizontalSpeed;
			float _VerticalSpeed;

			float _BlurAmount;
			float _BlurSpeed;

			float _ReverseImageX;
			float _ReverseImageY;

			float _BrightMultipler;
			float _BrightAmount;

			float _BrightLerp;

			float4 vertex_shader (float4 vertex:POSITION):SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}

			float4 pixel_shader (float4 vertex:POSITION):COLOR
			{
				vector <float,2> uv = vertex.xy/_ScreenParams.xy;

				uv.x+= cos(uv.y*_HorizontalMultiplier+_Time.g * _HorizontalSpeed)*_HorizontalAmount;
				uv.y+= sin(uv.x*_VerticalMultiplier+_Time.g * _VerticalSpeed)*_VerticalAmount;

				float offset = sin(_Time.g *_BlurSpeed) * _BlurAmount;    

				float2 value = float2(uv.x,uv.y);

				if(_ReverseImageX==1){
					value.x = 1-value.x;
				}

				if(_ReverseImageY==1){
					value.y = 1-value.y;
				}

				uv =value;

				float4 a = tex2D(_MainTex,uv);    
				float4 b = tex2D(_MainTex,uv-float2(sin(offset),0.0));    
				float4 c = tex2D(_MainTex,uv+float2(sin(offset),0.0));    
				float4 d = tex2D(_MainTex,uv-float2(0.0,sin(offset)));    
				float4 e = tex2D(_MainTex,uv+float2(0.0,sin(offset)));  

				_BrightLerp = sin(_BrightLerp + _Time.g *_BrightMultipler);    

				return (a+b+c+d+e)/(_BrightAmount + _BrightLerp);
			}
			ENDCG
		}
	}
}