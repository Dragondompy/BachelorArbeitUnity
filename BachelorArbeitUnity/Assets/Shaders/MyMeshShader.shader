Shader "Custom/MyMeshShader"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1) 
	}
	SubShader
	{
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert

		float4 _Color;

		struct Input
		{
			float3 vertColors;
		};

		void vert(inout appdata_full v, out Input o)
		{
			o.vertColors = v.color.rgb;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			float x = max(IN.vertColors.r,IN.vertColors.g);
			float y = max(x,IN.vertColors.b);
			float v = max(0,sign(x-0.90));
			float4 pixelColor = float4(v,v,v,1.0);
			o.Albedo = pixelColor + _Color;
		}
		ENDCG
		
	}
}