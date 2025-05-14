#ifndef SHAPE_HLSL_INCLUDED
#define SHAPE_HLSL_INCLUDED
// Multiplie une matrice 3x3 avec un vecteur 3D
float3x3 mul(float3x3 a, float3x3 b) {
    float3x3 result;
    for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 3; j++) {
            result[i][j] = a[i][0] * b[0][j] + a[i][1] * b[1][j] + a[i][2] * b[2][j];
        }
    }
    return result;
}

// Matrice de rotation autour de l'axe X
float3x3 rotationX(float angle) {
    return float3x3(
        1, 0,           0,
        0, cos(angle), -sin(angle),
        0, sin(angle),  cos(angle)
    );
}

// Matrice de rotation autour de l'axe Y
float3x3 rotationY(float angle) {
    return float3x3(
        cos(angle),  0, sin(angle),
        0,           1, 0,
        -sin(angle), 0, cos(angle)
    );
}

// Matrice de rotation autour de l'axe Z
float3x3 rotationZ(float angle) {
    return float3x3(
        cos(angle), -sin(angle), 0,
        sin(angle),  cos(angle), 0,
        0,           0,          1
    );
}

float SDFBox(float3 samplePosition, float3 offset, float3 halfSize, float round, float smooth, float3 rotationAngles) {
    float3x3 rotX = rotationX(rotationAngles.x);
    float3x3 rotY = rotationY(rotationAngles.y);
    float3x3 rotZ = rotationZ(rotationAngles.z);
    float3x3 rotation = mul(rotZ, mul(rotY, rotX));
    
    // Soustrayez l'offset de la position d'échantillonnage
    float3 translatedPosition = offset - samplePosition ;
    
    // Appliquez la rotation
    float3 rotatedPosition = mul(rotation, translatedPosition);
    
    float3 q = abs(rotatedPosition) - halfSize;
    float rect = length(max(q, 0)) + min(max(q.x, max(q.y, q.z)), 0.0) - round;
    return smoothstep(0, smooth, rect);
}

void SDFBox_float(float3 samplePosition, float3 offset, float3 halfSize, float round, float smooth, float3 rotationAngles, out float _out) {
   
    _out = SDFBox(samplePosition, offset, halfSize, round, smooth, rotationAngles);
}

float SDFSphere(float3 samplePosition, float3 center, float radius, float smooth) {
    float distance = length(samplePosition - center) - radius;
    return smoothstep(-smooth, smooth, distance);
}

void SDFSphere_float(float3 samplePosition, float3 center, float radius, float smooth, out float _out) {
    _out = SDFSphere(samplePosition, center, radius, smooth);
}

#endif // SHAPE_HLSL_INCLUDED