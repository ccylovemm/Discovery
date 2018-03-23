Shader "Image Effects/Fog of War"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FogTex0 ("Fog 0", 2D) = "white" {}
		_FogTex1 ("Fog 1", 2D) = "white" {}
		_Unexplored ("Unexplored Color", Color) = (0.05, 0.05, 0.05, 0.05)
		_Explored ("Explored Color", Color) = (0.35, 0.35, 0.35, 0.35)
	}
	SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Fog { Mode off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag vertex:vert
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _FogTex0;
			sampler2D _FogTex1;

			uniform float _BlendFactor;
			uniform half4 _Unexplored;
			uniform half4 _Explored;
			uniform float4x4 _InverseMVP;

			struct Input
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			void vert (inout appdata_base v, out Input o)
			{
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
			}

			float3 CamToWorld (in float2 uv, in float depth)
			{
				float4 pos = float4(uv.x, uv.y, depth, 1.0);
				pos.xyz = pos.xyz * 2.0 - 1.0;
				pos = mul(_InverseMVP, pos);
				return pos.xyz / pos.w;
			}

			fixed4 frag (Input i) : COLOR
			{
				half4 original = tex2D(_MainTex, i.uv);
				float3 pos = CamToWorld(i.uv, 1) * 0.098;
				half4 fog = lerp(tex2D(_FogTex0, pos.xy), tex2D(_FogTex1, pos.xy), _BlendFactor);
				return lerp(lerp(original * _Unexplored, original * _Explored, fog.g), original, fog.r);
			}
			ENDCG
		}
	}
	Fallback off
}

