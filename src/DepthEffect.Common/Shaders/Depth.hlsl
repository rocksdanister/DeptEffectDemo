#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_COMPLEX
#define D2D_INPUT1_SIMPLE

#define D2D_REQUIRES_SCENE_POSITION

#include "d2d1effecthelpers.hlsli"

float2 mouse = {0, 0};
float2 intensity = {1, 1};

D2D_PS_ENTRY(main)
{
    float2 uv = D2DGetScenePosition().xy;

    float depth = D2DGetInput(1).r;
    float2 parallax = mouse * depth * intensity;

    float4 color = D2DSampleInputAtPosition(0, uv + parallax);
    return color;
}