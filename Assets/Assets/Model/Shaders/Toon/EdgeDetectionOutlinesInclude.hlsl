#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

// The sobel effect runs by sampling the texture around a point to see
// if there are any large changes. Each sample is multiplied by a convolution
// matrix weight for the x and y components seperately. Each value is then
// added together, and the final sobel value is the length of the resulting float2.
// Higher values mean the algorithm detected more of an edge

// These are points to sample relative to the starting point
static float2 sobelSamplePoints[9] = {
    float2(-1, 1), float2(0, 1), float2(1, 1),
    float2(-1, 0), float2(0, 0), float2(1, 0),
    float2(-1, -1), float2(0, -1), float2(1, -1),
};

// Weights for the x component
static float sobelXMatrix[9] = {
    1, 0, -1,
    2, 0, -2,
    1, 0, -1
};

// Weights for the y component
static float sobelYMatrix[9] = {
    1, 2, 1,
    0, 0, 0,
    -1, -2, -1
};

// This function runs the sobel algorithm over the depth texture
void DepthSobel_float(float2 UV, float Thickness, out float Out) {
    float2 sobel = 0;
    float depth_current = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV);
    [unroll] for (int i = 0; i < 9; i++) {
        float2 sampleUV = UV + sobelSamplePoints[i] * Thickness;
        float depth_sample = SHADERGRAPH_SAMPLE_SCENE_DEPTH(sampleUV);
        // Compare the depth of the current pixel with the sampled pixel
        if (depth_sample < depth_current) {
            sobel += (depth_current - depth_sample) * float2(sobelXMatrix[i], sobelYMatrix[i]);
        }
    }
    // Get the final sobel value
    Out = length(sobel);
}

#endif