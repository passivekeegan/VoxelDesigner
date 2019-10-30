Shader "Voxel/Editor/Voxel Component"
{
	Properties
	{
		_Normal ("Normal Colour", Color) = (1, 1, 1, 1)
		_PrimarySelect ("Primary Select Colour", Color) = (1, 1, 1, 1)
		_PrimarySuper ("Primary Super Select Colour", Color) = (1, 1, 1, 1)
		_SecondarySelect ("Secondary Select Colour", Color) = (1, 1, 1, 1)
		_SecondarySuper ("Secondary Super Select Colour", Color) = (1, 1, 1, 1)
	}
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf WrapLambert vertex:vert

		fixed4 _Normal;
		fixed4 _PrimarySelect;
		fixed4 _PrimarySuper;
		fixed4 _SecondarySelect;
		fixed4 _SecondarySuper;

		struct Input {
			fixed colour_index;
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.colour_index = v.texcoord.x;
		}

		half4 LightingWrapLambert(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
			c.a = s.Alpha;
			return c; 
		}

		void surf(Input IN, inout SurfaceOutput o) {
			fixed colour_value = floor(IN.colour_index * 10);
			bool notnormal = false;
			//Primary Select
			bool uvcolour = (colour_value == 1);
			o.Albedo = (uvcolour * _PrimarySelect.rgb);
			o.Alpha = (uvcolour * _PrimarySelect.a);
			notnormal = notnormal || uvcolour;
			//Primary Super Select
			uvcolour = (colour_value == 2);
			o.Albedo += (uvcolour * _PrimarySuper.rgb);
			o.Alpha += (uvcolour * _PrimarySuper.a);
			notnormal = notnormal || uvcolour;
			//Secondary Select
			uvcolour = (colour_value == 3);
			o.Albedo += (uvcolour * _SecondarySelect.rgb);
			o.Alpha += (uvcolour * _SecondarySelect.a);
			notnormal = notnormal || uvcolour;
			//Secondary Super Select
			uvcolour = (colour_value == 4);
			o.Albedo += (uvcolour * _SecondarySuper.rgb);
			o.Alpha += (uvcolour * _SecondarySuper.a);
			notnormal = notnormal || uvcolour;
			//Normal
			o.Albedo += ((!notnormal) * _Normal.rgb);
			o.Alpha += ((!notnormal) * _Normal.a);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
