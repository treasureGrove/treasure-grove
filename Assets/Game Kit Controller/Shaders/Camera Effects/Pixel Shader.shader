Shader "Custom/Pixel Shader" 
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		 _TintColor ("Tint Color", Color) = (1.0, 0.6, 0.6, 1.0)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;	
			float2 BlockCount;
			float2 BlockSize;

			 float4 _TintColor;

			fixed4 frag (v2f_img i) : SV_Target
			{
				float2 blockPos = floor(i.uv * BlockCount);
				float2 blockCenter = blockPos * BlockSize + BlockSize * 0.5;

				float4 tex = tex2D(_MainTex, blockCenter) + _TintColor;
				return tex;
			}
			ENDCG
		}
	}
}
