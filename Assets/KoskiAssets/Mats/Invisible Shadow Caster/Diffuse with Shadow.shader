Shader "Transparent/Diffuse with Shadow" {
	SubShader{

		//LOD 400
		Tags{
		"Queue" = "Transparent"
		//"RenderType" = "Transparent"
		"IgnoreProjector" = "True" 
		"RenderType" = "TransparentCutout"
	}
		

		CGPROGRAM
#pragma surface surf Lambert alpha addshadow

		struct Input {
		float nothing; // Just a dummy because surf expects something
	};

	void surf(Input IN, inout SurfaceOutput o) {
		o.Alpha = 0;
	}
	ENDCG
	}
	

		FallBack "Diffuse"
}