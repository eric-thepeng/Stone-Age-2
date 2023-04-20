Shader "TREEGEN/ViewEdgeMeshEditor"
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
          float3 pos1 = input[i].vertex.xyz;
          int j=((i<2) ? i+1 : 0);
          float3 pos2 = input[j].vertex.xyz;
          float3 pos =(pos1 + pos2) * 0.5f;
          float l1 = length(pos1)*0.005f;
          float l2 = length(pos2)*0.005f;
          float3 vec1 = normalize(pos1 - pos2);
          float3 vec2 = normalize(cross(pos, vec1));

          g2f v1;
          g2f v2;
          g2f v3;
          g2f v4;
          v1.pos = pos;
          v2.pos = pos;
          v3.pos = pos;
          v4.pos = pos;
          v1.ray = pos1+(vec1+vec2)*l1;
          v2.ray = pos2-(vec1-vec2)*l2;
          v3.ray = pos2-(vec1+vec2)*l2;
          v4.ray = pos1+(vec1-vec2)*l1;
          v1.vertex = mul(UNITY_MATRIX_P,float4(v1.ray*0.999,1));
          v2.vertex = mul(UNITY_MATRIX_P,float4(v2.ray*0.999,1));
          v3.vertex = mul(UNITY_MATRIX_P,float4(v3.ray*0.999,1));
          v4.vertex = mul(UNITY_MATRIX_P,float4(v4.ray*0.999,1));
          v1.radius = l1;
          v2.radius = l2;
          v3.radius = l2;
          v4.radius = l1;

          float3 nv1 = cross(v1.ray, v4.ray);
          float3 nv2 = cross(v3.ray, v2.ray);
          float3 nv3 = cross(v1.ray, v2.ray);
          float3 nv4 = cross(v3.ray, v4.ray);

          float3 ray = mul((float3x3)UNITY_MATRIX_MV,MouseRay);
          fixed4 color1 = input[i].color;
          fixed4 color2 = input[j].color;
          if (dot(ray, -nv1) > 0 && dot(ray, -nv2) > 0 && dot(ray, nv3) > 0 && dot(ray, nv4) > 0) {
            color1.xyz=fixed3(1,0,0);
            color2.xyz=fixed3(1,0,0);
          }
          v1.color = color1;
          v2.color = color2;
          v3.color = color2;
          v4.color = color1;

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
        //float b = dot(i.pos,normalize(i.ray));
        //float c = dot(i.pos,i.pos) - i.radius * i.radius;
        //float d = b*b - c;
        //if (d>0)
          return i.color;

				//return fixed4(0,0,0,0);
			}
			ENDCG
		}
  }
}
