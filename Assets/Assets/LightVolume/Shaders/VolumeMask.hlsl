//#include "Assets/Render/LightVolume/Shaders/Shape.hlsl"

// Matrice de rotation autour de l'axe X
float3x3 rotationXMask(float angle) {
    return float3x3(
        1, 0,           0,
        0, cos(angle), -sin(angle),
        0, sin(angle),  cos(angle)
    );
}

// Matrice de rotation autour de l'axe Y
float3x3 rotationYMask(float angle) {
    return float3x3(
        cos(angle),  0, sin(angle),
        0,           1, 0,
        -sin(angle), 0, cos(angle)
    );
}

// Matrice de rotation autour de l'axe Z
float3x3 rotationZMask(float angle) {
    return float3x3(
        cos(angle), -sin(angle), 0,
        sin(angle),  cos(angle), 0,
        0,           0,          1
    );
}

float SDFBoxMask(float3 samplePosition, float3 offset, float3 halfSize, float round, float smooth, float3 rotationAngles) {
    float3x3 rotX = rotationXMask(rotationAngles.x);
    float3x3 rotY = rotationYMask(rotationAngles.y);
    float3x3 rotZ = rotationZMask(rotationAngles.z);
    float3x3 rotation = mul(rotZ, mul(rotY, rotX));
    
    // Soustrayez l'offset de la position d'échantillonnage
    float3 translatedPosition = offset - samplePosition ;
    
    // Appliquez la rotation
    float3 rotatedPosition = mul(rotation, translatedPosition);
    
    float3 q = abs(rotatedPosition) - halfSize;
    float rect = length(max(q, 0)) + min(max(q.x, max(q.y, q.z)), 0.0) - round;
    return smoothstep(0, smooth, rect);
}

/* Volume Mask */
float4 GetMaskVolume(
	float3 iWorldPosition,
	float3 iMaskPos,
	float3 iMaskRot,
	float iLightRange,
	float iRangeHardness,
	float iIsSphere,
	float isAddArr,
	float iBoxRound,
	float iBoxSoft,
	float4 iBoxSize,
	float4 iColor
	)
		{
		float4 finalColor;
		
		float attenuation = 0;
		if (iIsSphere == 1)
		{
			float3 vecPosToLight = iMaskPos - iWorldPosition;
			float distFromRange = clamp(length(vecPosToLight) / iLightRange, 0.0, 1.0); // 0 : on the light source , > 1 : out of range
			attenuation = (1.0 - distFromRange);
			attenuation = saturate(attenuation * iRangeHardness);
		}
		else
		{
			attenuation =  saturate(1 - SDFBoxMask(iWorldPosition,iMaskPos,iBoxSize / 2, iBoxRound, iBoxSoft, iMaskRot));
		}

		float4 Color = attenuation * iColor;
	
		finalColor = Color;
		return finalColor;
}

uniform int VMCount;
uniform half4 PosArr[16];
uniform half4 RotArr[16];
uniform half RangeArr[16];
uniform half HardnessArr[16];
uniform half IsSphereArr[16];
uniform half isAddArr[16];
uniform half BoxRoundArr[16];
uniform half BoxSoftBorderArr[16];
uniform half4 BoxSizeArr[16];
uniform half4 ColorArr[16];

void GetAllMaskVolume_half(float3 WorldPosition, out float4 finalColor)
{
    finalColor = float4(0, 0, 0, 0);

    for (int i = 0; i < VMCount; i++)
    {
        float4 color = GetMaskVolume(WorldPosition,
			PosArr[i],
			RotArr[i],
			RangeArr[i],
			HardnessArr[i],
			IsSphereArr[i],
			isAddArr[i],
			BoxRoundArr[i],
			BoxSoftBorderArr[i],
			BoxSizeArr[i],
			ColorArr[i]
			);

		finalColor = saturate(color + finalColor);
    }
}