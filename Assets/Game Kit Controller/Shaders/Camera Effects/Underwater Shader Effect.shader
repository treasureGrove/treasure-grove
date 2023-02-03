Shader "Custom/Underwater Shader Effect" {
	// simple masked blur shader.
    // MainTex is used by unity to input camera image
    // Amount is the amount of blur
    // Mask is an image that uses brightness of pixel
    // to determine how much of the blur is applied to pixel
    // white = no blur, black = full blur, values inbetween = mix
 
    // written by christian franz, you have permission to use as you
    // see fit.
 
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // bit input from Unity
        _Amount ("Amount", Range(1, 100)) = 7 // how much blurring
        _Mask("Mask", 2D) = "white" {} // blend mask, white = no blur, black = full blur
        _TintColor ("Tint Color", Color) = (1.0, 0.6, 0.6, 1.0)
    }
 
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
           
 
            struct appdata   {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f  {
                float2 uv : TEXCOORD0;
                float2 muv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };
 
 
            sampler2D _MainTex;
            sampler2D _Mask;
            float _Amount;
            // implicit defines
            float4 _MainTex_ST, _Mask_ST, _MainTex_TexelSize;

            float4 _TintColor;
 
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.muv = TRANSFORM_TEX(v.uv, _Mask);
                return o;
            }
 
 
            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = fixed4(0, 0, 0, 1.0);
                // sum all pixels from -Amount to Amount
                // horizontally
                for (int index = -_Amount; index <= _Amount; index++) {
                    float2 uv = i.uv + float2(index * _MainTex_TexelSize.x, 0);
                    col += tex2D(_MainTex, uv);
                }
 
                // vertically
                for (int index = -_Amount; index <= _Amount; index++) {
                    float2 uv = i.uv + float2(0, index * _MainTex_TexelSize.y);
                    col += tex2D(_MainTex, uv);
                }
                // now divide by the number of samples. Samples is 2 * 2 * amount + 1: Amount = 1: -1, 0, 1 = 3 samples * 2 (horizontally, vertically)
                col /= (_Amount * 4 + 2);
 
                // now sample the original value
                float4 orig = tex2D(_MainTex, i.uv);
                float4 mask = tex2D(_Mask, i.muv);
                float unblurred = mask.r ;
                float blurred = (1.0 - unblurred);
                col = col * blurred + orig * unblurred  + _TintColor;
                return col;
            }
            ENDCG
        }
    }
}
