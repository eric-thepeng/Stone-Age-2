Shader "TREEGEN/NoiseColor" {
	Properties {
		_Color1 ("Color1", Color) = (1,1,1,1)
    _Color2 ("Color2", Color) = (1,1,1,1)
    _Color3 ("Color3", Color) = (1,1,1,1)
    _NoseForce ("Nose Force", Range(0,2)) = 0.5
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
    _Scale ("Scale", Float) = 1.0
    _Seed ("Seed", Float) = 1.0
    [Toggle]_NNoise ("Normal Noise", float) = 0
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
      float4 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color1;
    fixed4 _Color2;
    fixed4 _Color3;
    float _NoseForce;
    float _Scale;
    float _Seed;
    bool _NNoise;

    float hash3x1(float3 p,float seed) { return frac(cos(dot(p,float3(1,109.0f,187.0f))*27.342f +seed )*345.342f); }
    float4 hash3x4(float3 p,float seed) { float c=dot(p,float3(1,109.0f,187.0f))*27.342f; return frac(cos(float4(0,100,200,300)+c +seed)*345.342f); }

    void vert(inout appdata_full v, out Input o) {
      o.spec_uv = v.vertex.xyz * _Scale;
      o.color = hash3x4(v.vertex.xyz+_NNoise*v.normal,_Seed);
    }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
      float3 normal = o.Normal;
      float3 mix = abs(normal*normal*normal);
      mix = mix / (mix.x + mix.y + mix.z);
      fixed4 c = (tex2D(_MainTex, IN.spec_uv.xy)*mix.z + tex2D(_MainTex, IN.spec_uv.xz)*mix.y + tex2D(_MainTex, IN.spec_uv.zy)*mix.x);
			o.Albedo = c.rgb * lerp(lerp(_Color1,_Color2,clamp(IN.color.w*2.0,0,1)),_Color3,clamp(IN.color.w*2.0-1.0,0,1)) + (IN.color.xyz-0.5)*_NoseForce;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
