Shader "Custom/Mask"
{
	Properties{}

	SubShader{

		Tags {
			"RenderType" = "Opaque"
			//"Queue" = "Transparent+1"
		}

		Pass {
			//Blend Zero One
			ZWrite Off
			ColorMask 0
		}
	}
}