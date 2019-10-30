Shader "Voxel/Editor/Vertex"
{
	Properties
	{
		_Stripe("Stripe Value", float) = 0.1
		_Primary("Primary Colour", Color) = (1,1,1,1)
		_Secondary("Secondary Colour", Color) = (1,1,1,1)
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf WrapLambert

			float _Stripe;
			fixed4 _Primary;
			fixed4 _Secondary;

			struct Input {
				float3 worldPos;
			};

			half4 LightingWrapLambert(SurfaceOutput s, half3 lightDir, half atten) {
					half NdotL = dot(s.Normal, lightDir);
					half diff = NdotL * 0.5 + 0.5;
					half4 c;
					c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
					c.a = s.Alpha;
					return c;
			}

			void surf(Input IN, inout SurfaceOutput o) {
					float d = dot(IN.worldPos, normalize(fixed3(1, 1, 1)));
					d = lerp(abs(d) + _Stripe, d, step(0, d));
					float t = step(1, (d / _Stripe) % 2);
					o.Albedo = lerp(_Primary.rgb, _Secondary.rgb, t);
					o.Alpha = lerp(_Primary.a, _Secondary.a, t);
			}
			ENDCG
	}
		FallBack "Diffuse"
}