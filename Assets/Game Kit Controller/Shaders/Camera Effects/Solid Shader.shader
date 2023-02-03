Shader "Custom/Solid Shader" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SprTex("Texture", 2D) = "white" {}
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
			sampler2D _SprTex;
			float4 _Color = float4(1, 1, 1, 1);
			float2 BlockCount;
			float2 BlockSize;

			 float4 _TintColor;

			fixed4 frag(v2f_img i) : SV_Target
			{
				float2 blockPos = floor(i.uv * BlockCount);
				float2 blockCenter = blockPos * BlockSize + BlockSize * 0.5;

				float4 del = float4(1, 1, 1, 1) - _Color;

				float4 tex = tex2D(_MainTex, blockCenter) - del + _TintColor;
				float grayscale = dot(tex.rgb, float3(0.3, 0.59, 0.11));
				grayscale = clamp(grayscale, 0.0, 1.0);

				float dx = floor(grayscale * 16.0);

				float2 sprPos = i.uv;
				sprPos -= blockPos*BlockSize;
				sprPos.x /= 16;
				sprPos *= BlockCount;
				sprPos.x += 1.0 / 16.0 * dx;

				float4 tex2 = tex2D(_SprTex, sprPos);
				return tex2;
			}
			ENDCG
		}
	}
}
