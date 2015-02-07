Shader "Game/AirplaneBlender" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
		_BurntTex ("Burnt Texture", 2D) = "white" {}
		_BurntLevel ("Burnt Level", Range (0,1)) = 0.5 

		_EnergyTex ("Energy Texture", 2D) = "white" {}
		_EnergyColor ("Color", Color) = (1,1,1)
		_EnergyLevel ("Energy Level", Range (0,0.5)) = 0 
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGINCLUDE
		#include "UnityCG.cginc"	
		ENDCG

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _BurntTex;
		sampler2D _EnergyTex;
		half4 _EnergyColor;
		half _BurntLevel;
		half _EnergyLevel;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BurntTex;
			float2 uv2_EnergyTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c_main   = tex2D (_MainTex, IN.uv_MainTex);
			half4 c_burnt  = tex2D (_BurntTex, IN.uv_BurntTex);

			half2 uv_energy = IN.uv2_EnergyTex;
			uv_energy.y = uv_energy.y + _EnergyLevel;
			half4 c_energy = tex2D (_EnergyTex, uv_energy);
			
			half energy = c_energy.r * _EnergyColor.a;

			o.Albedo = (c_main.rgb * (1-energy) +  _EnergyColor.rgb * energy) *  (1 - _BurntLevel + c_burnt.rgb * _BurntLevel);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
