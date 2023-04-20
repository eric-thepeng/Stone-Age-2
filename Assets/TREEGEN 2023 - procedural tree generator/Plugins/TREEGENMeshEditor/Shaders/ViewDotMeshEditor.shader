Shader "TREEGEN/ViewDotMeshEditor"
{
	SubShader
	{
    Tags { "Queue" = "Transparent" }
    LOD 200
		Pass
		{
      Blend One OneMinusSrcAlpha
      ZWrite Off
      Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			
			#include "UnityCG.cginc"

      float3 MouseRay;

			struct appdata
			{
				float3 vertex : POSITION;
        fixed4 color : COLOR;
			};

      struct v2f
			{
				float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
			};

			struct g2f
			{
				float4 vertex : SV_POSITION;
        float3 pos : TEXTURE1;
        float3 ray : NORMAL0;
        float radius : TEXTURE0;
        fixed4 color : COLOR;
			};
		
			v2f vert (appdata_full vertex)
			{
				v2f o;
				o.vertex = float4(UnityObjectToViewPos(vertex.vertex.xyz),1);
        o.color = vertex.color;
				return o;
			}
			
      [maxvertexcount(18)]
      void geom(triangle v2f input[3], inout TriangleStream<g2f> OutputStream)
      {
        for(int i=0;i<3;i++)
        {
          float3 pos = input[i].vertex.xyz;
          float l = length(pos)*0.01f;
          float3 vec1 = (pos.x == 0 && pos.z == 0) ? float3(0.0f, 0.0f, pos.y < 0 ? -1.0f : 1.0f) : normalize(float3(pos.z, 0.0f, -pos.x));
          float3 vec2 = normalize(cross(pos, vec1));
          vec1 *= l;
          vec2 *= l;

          g2f v1;
          g2f v2;
          g2f v3;
          g2f v4;
          v1.pos = pos;
          v2.pos = pos;
          v3.pos = pos;
          v4.pos = pos;
          v1.ray = pos+vec1+vec2;
          v2.ray = pos-vec1+vec2;
          v3.ray = pos-vec1-vec2;
          v4.ray = pos+vec1-vec2;
          v1.vertex = mul(UNITY_MATRIX_P,float4(v1.ray*0.995,1));
          v2.vertex = mul(UNITY_MATRIX_P,float4(v2.ray*0.995,1));
          v3.vertex = mul(UNITY_MATRIX_P,float4(v3.ray*0.995,1));
          v4.vertex = mul(UNITY_MATRIX_P,float4(v4.ray*0.995,1));
          v1.radius = l;
          v2.radius = l;
          v3.radius = l;
          v4.radius = l;
          fixed4 color = input[i].color;
          if (dot(mul((float3x3)UNITY_MATRIX_MV,MouseRay),normalize(pos)) > 0.99995f)
            color.xyz=fixed3(1,0,0);
          v1.color = color;
          v2.color = color;
          v3.color = color;
          v4.color = color;

          OutputStream.Append(v1);
          OutputStream.Append(v2);
          OutputStream.Append(v3);
          OutputStream.RestartStrip();
          OutputStream.Append(v3);
          OutputStream.Append(v4);
          OutputStream.Append(v1);
          OutputStream.RestartStrip();
        }
      }

			fixed4 frag (g2f i) : SV_Target
			{
        float b = dot(i.pos,normalize(i.ray));
        float c = dot(i.pos,i.pos) - i.radius * i.radius;
        float d = b*b - c;
        if (d>0)
          return i.color;

				return fixed4(0,0,0,0);
			}
			ENDCG
		}
	}
}