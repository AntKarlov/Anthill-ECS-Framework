// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Anthill/ShockWave" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
		_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
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
			uniform float _PosX;
			uniform float _PosY;
			uniform float _Value;
			uniform float _Size;
			uniform float4 _ScreenResolution;
			uniform float _Scale;

			struct appdata_t
			{
				float4 vertex:POSITION;
				float4 color:COLOR;
				float2 texcoord:TEXCOORD0;
			};

			struct v2f
			{
				half2 texcoord:TEXCOORD0;
				float4 vertex:SV_POSITION;
				fixed4 color:COLOR;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				return OUT;
			}

			float4 frag (v2f IN):COLOR
			{
				float2 uv = IN.texcoord.xy;
				float f = (_ScreenResolution.y / _ScreenResolution.x);
				uv.y = uv.y * f;
				float Dist = distance(uv, float2(_PosX, _PosY * f));
				float Diff = (Dist - _Value * _Scale);
				float v = (1.0 - pow(abs(Diff * 10.0), 0.8));
				float vt = saturate(v * 0.02 * (_Size * _Scale));

				uv = IN.texcoord.xy;
				uv.x = uv.x - vt;
				uv.y = uv.y - vt;

				float4 Color = tex2D(_MainTex, uv);
				Color.rgb += float3(vt, vt, vt);
				return Color; 
			}
			ENDCG
		}
	}
}
