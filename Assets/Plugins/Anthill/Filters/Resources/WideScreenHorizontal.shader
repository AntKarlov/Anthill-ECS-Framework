Shader "Anthill/WideScreenHorizontal"
{ 
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
	}

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			#pragma glsl
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float _TimeX;
			uniform float _Size;
			uniform float _Smooth;
			uniform float4 _Color;

			struct appdata_t
			{
				float4 vertex: POSITION;
				float4 color: COLOR;
				float2 texcoord: TEXCOORD0;
			};

			struct v2f
			{
				half2 texcoord: TEXCOORD0;
				float4 vertex: SV_POSITION;
				fixed4 color: COLOR;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				return OUT;
			}

			float4 frag (v2f i): COLOR
			{
				float2 uv = i.texcoord.xy / 1.0;
				float4 tex = tex2D(_MainTex, uv);
				float dist2 = 1.0 - smoothstep(_Size, _Size - _Smooth, length(float2(0.5, 0.5) - uv.y));

				//float3 color = float3(0.0, 0.0, 0.0);
				float3 color = float3(_Color.r, _Color.g, _Color.b);
				float3 ret = lerp(tex, color, dist2);

				return float4(ret, 1.0);
			}
			ENDCG
		}
	}
}
