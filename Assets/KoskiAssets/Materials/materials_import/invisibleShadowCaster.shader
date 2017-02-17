Shader "Transparent/invisibleShadowCaster"
{
	Subshader
	{
		UsePass "VertexLit/SHADOWCOLLECTOR"
		UsePass "VertexLit/SHADOWCASTER"
	}

		Fallback off
}

