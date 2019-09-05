Shader "Voxel/Editor/Axi"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		LOD 200

		ZWrite Off
			ZTest Less
			Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf WrapLambert alpha

		fixed4 _Color;

		half4 LightingWrapLambert(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
			c.a = s.Alpha;
			return c;
		}

		struct Input {
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG

			ZWrite Off
			ZTest GEqual
			Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf WrapLambert alpha

		fixed4 _Color;

		half4 LightingWrapLambert(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
			c.a = s.Alpha;
			return c;
		}

		struct Input {
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a / 4.0;
		}
		ENDCG


	}
    FallBack "Diffuse"
}
