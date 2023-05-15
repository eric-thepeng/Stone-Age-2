Shader "TREEGEN/NO_UV" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
    _Scale ("Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float3 spec_uv;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
    float _Scale;

    void vert(inout appdata_full v, out Input o) {
      o.spec_uv = v.vertex.xyz * _Scale;
    }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
      float3 normal = o.Normal;
      float3 mix = abs(normal*normal*normal);
      mix = mix / (mix.x + mix.y + mix.z);
      fixed4 c = (tex2D(_MainTex, IN.spec_uv.xy)*mix.z + tex2D(_MainTex, IN.spec_uv.xz)*mix.y + tex2D(_MainTex, IN.spec_uv.zy)*mix.x)*_Color;// tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
