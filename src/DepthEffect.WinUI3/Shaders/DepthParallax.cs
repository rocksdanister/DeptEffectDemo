﻿using ComputeSharp;

namespace DepthEffect.WinUI3.Shaders;

[AutoConstructor]
[EmbeddedBytecode(DispatchAxis.XY)]
internal readonly partial struct DepthParallax : IPixelShader<float4>
{
    public readonly IReadOnlyNormalizedTexture2D<float4> imageTexture;

    public readonly IReadOnlyNormalizedTexture2D<float4> depthTexture;

    public readonly float2 mouse = new(0, 0);

    public readonly float2 intensity = new(1, 1);

    /// <inheritdoc/>
    public float4 Execute()
    { 
        float2 fragCoord = new(ThreadIds.X, DispatchSize.Y - ThreadIds.Y);
        float2 uv = fragCoord / DispatchSize.XY;
        uv.Y = 1.0f - uv.Y;

        // Fill scale
        float screenAspect = (float)DispatchSize.X / DispatchSize.Y;
        float textureAspect = (float)imageTexture.Width / imageTexture.Height;
        float scaleX = 1f, scaleY = 1f;
        if (textureAspect > screenAspect)
            scaleX = screenAspect / textureAspect;
        else
            scaleY = textureAspect / screenAspect;
        uv = new Float2(scaleX, scaleY) * (uv - 0.5f) + 0.5f;

        float depth = depthTexture.Sample(uv).R;
        Float2 parallax = mouse * depth * intensity;

        float4 color = new(imageTexture.Sample(uv + parallax).RGB, 1);
        return color;
    }
}