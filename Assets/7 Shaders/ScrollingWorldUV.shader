Shader "Custom/ScrollingWorldUV"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Tiling("Tiling", Vector) = (1, 1, 1)
		_Speed("Speed", Float) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
		// float2 _Tiling;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		float _Speed;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float2 _Tiling;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float3 scroll_dir = float3(o.Normal.y, o.Normal.z, o.Normal.x);

			float3 pos = IN.worldPos + (float3(_Time.y, _Time.y, _Time.y) * (float3(_Speed, _Speed, _Speed) - scroll_dir));

			float3 c = tex2D(_MainTex, pos.xz * _Tiling.xy).rgb;

			if (o.Normal.y < 0.5f)
			{
				float3 c1 = tex2D(_MainTex, float2(pos.z * _Tiling.x, pos.y * _Tiling.y)).rgb;
				float3 c2 = tex2D(_MainTex, pos.xz * _Tiling.xy).rgb;
				float3 c3 = tex2D(_MainTex, pos.xy * _Tiling.xy).rgb;

				float3 c21 = lerp(c2, c1, abs(o.Normal.x)).rgb;
				float3 c23 = lerp(c21, c3, abs(o.Normal.z)).rgb;

				c = c23;
			}

			// Albedo comes from a texture tinted by color
			o.Albedo = c * _Color.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a;
		}
		ENDCG
		}
			FallBack "Diffuse"
}
