Shader "TREEGEN/ViewFaceMeshEditor"
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
        fixed4 color = input[0].color;
        for(int k=1;k<3;k++) {
          color = min(color,input[k].color);
        }

        g2f v[3];

        for(int i=0;i<3;i++)
        {
          float3 pos = input[i].vertex.xyz;
          float l = length(pos)*0.01f;

          v[i].pos = pos;
          v[i].ray = pos;
          v[i].vertex = mul(UNITY_MATRIX_P,float4(v[i].ray*0.995,1));
          v[i].radius = l;
        }

        float3 nv1 = cross(v[0].ray, v[1].ray);
        float3 nv2 = cross(v[1].ray, v[2].ray);
        float3 nv3 = cross(v[2].ray, v[0].ray);
        float3 ray = mul((float3x3)UNITY_MATRIX_MV,MouseRay);
        if (dot(ray, nv1) > 0 && dot(ray, nv2) > 0 && dot(ray, nv3) > 0)
          color = fixed4(1,0,0,1);
        for(int j=0;j<3;j++)
        {
          v[j].color = color;
          v[j].color.w=0.1f;

          OutputStream.Append(v[j]);
        }
        OutputStream.RestartStrip();
      }

			fixed4 frag (g2f i) : SV_Target
			{
        /*float b = dot(i.pos,normalize(i.ray));
        float c = dot(i.pos,i.pos) - i.radius * i.radius;
        float d = b*b - c;
        if (d>0)*/
        return i.color;

				//return fixed4(0,0,0,0);
			}
			ENDCG
		}
	}
}
