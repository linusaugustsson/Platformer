Shader "Custom/EyeBeam"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Speed("Speed", Float) = 2
		_Amount("Amount", Float) = 16.0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Speed;
			float _Amount;
			float _Distance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float t = (sin(((i.uv.x) + (_Time.y * _Speed)) * _Amount) * 2.0f) * 2.0f;

				fixed4 col = _Color;
				col.a *= (t * 0.125f) + 0.75f;

				if (col.a < 0.0f)
				{
					col.a = 0.0f;
				}

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
