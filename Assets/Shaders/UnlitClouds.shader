// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/UnlitCloud"
{
    Properties
    {
		_Noise1Tex("Texture", 2D) = "white" {}
		_Noise2Tex ("Texture", 2D) = "white" {}
		_RadialGradient("Texture", 2D) = "white" {}

		_MainCloudSpeed("Primary Cloud Speed", Float) = 1.0
		_SecondaryCloudSpeed("Secondary Cloud Speed", Float) = 1.0
		_CloudOffset("Cloud Offset", Float) = 1.0

		_StepAmount("Step Amount", Float) = 0.2
		_CloudCutoff("Cloud Cutoff", Float) = 0.2
		_CloudSoftness("Cloud Softness", Float) = 1.0
		_TaperPower("Taper Power", Float) = 1.0

	}
		SubShader
		{
			Tags { "RenderType" = "Fade" }
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float3 wpos : TEXCOORD1;
				float2 radialUV : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			sampler2D _Noise1Tex;
			sampler2D _Noise12ex;
			sampler2D _RadialGradient;

			float4 _Noise1Tex_ST;
			float4 _RadialGradient_ST;

			float _MainCloudSpeed;
			float _SecondaryCloudSpeed;
			float _CloudOffset;

			float _StepAmount;
			float _CloudCutoff;
			float _CloudSoftness;

			float _CloudMiddleYPosition;
			float _CloudHeight;
			float _TaperPower;

			float4 Remap(float4 In, float2 InMinMax, float2 OutMinMax)
			{
				return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			}

            v2f vert (appdata v)
            {
				v2f o;
				o.uv = TRANSFORM_TEX(v.uv, _Noise1Tex);

                o.vertex = UnityObjectToClipPos(v.vertex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.wpos = worldPos;
				o.radialUV = TRANSFORM_TEX(v.uv, _RadialGradient);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				fixed4 noisePass1 = tex2D(_Noise1Tex, i.uv + _Time.y * _MainCloudSpeed);
				fixed4 noisePass2 = tex2D(_Noise1Tex, i.uv - _Time.y * _SecondaryCloudSpeed + float2(0, _CloudOffset));
				fixed4 radialTex = tex2D(_RadialGradient, i.radialUV);

				noisePass1 *= radialTex.r;
				noisePass2 *= radialTex.r;

				fixed4 col = lerp(noisePass1, noisePass2, 0.5f);
				
				col = Remap(col, float2(_CloudCutoff, 1.0f), float2(0.0f, 1.0f));
				col = saturate(col);
				col = pow(col, _CloudSoftness);

				col.a = col.r;

				//do middle gradient
				float gradient = abs(_CloudMiddleYPosition - i.wpos.y);
				gradient /= _CloudHeight;
				gradient = 1.0f - saturate(gradient);
				gradient = pow(gradient, _TaperPower);

				col.a *= gradient;
				col.rgb *= UNITY_LIGHTMODEL_AMBIENT	* gradient;

				//clip(col.a - 0.5f); //clip anything less than 0.5f
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDCG
        }
    }
}
