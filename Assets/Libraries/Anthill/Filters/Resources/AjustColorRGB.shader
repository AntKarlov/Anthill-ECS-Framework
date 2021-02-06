// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Anthill/AdjustColorRGB"
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
			uniform float _Red;
			uniform float _Green;
			uniform float _Blue;
			uniform float _Bright;

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

			float4 frag (v2f IN) : COLOR
			{
				float4 compo = tex2D(_MainTex, IN.texcoord.xy);
				compo.r += _Red;
				compo.g += _Green;
				compo.b += _Blue;
				compo.rgb += _Bright;
				return compo;
			}
			ENDCG
		}
	}
}
