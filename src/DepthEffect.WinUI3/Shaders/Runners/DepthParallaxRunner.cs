using ComputeSharp.WinUI;
using ComputeSharp;
using System;
using System.IO;

namespace DepthEffect.WinUI3.Shaders.Runners;

public sealed class DepthParallaxRunner : IShaderRunner
{
    private readonly Func<float2> mouse;
    private readonly ReadOnlyTexture2D<Rgba32, float4> image, depth;
    private float2 mouseOffset = new(0, 0);
    private float2 intensity = new(0.75f, 1f);

    public DepthParallaxRunner(Func<float2> mouse)
    {
        var graphics = GraphicsDevice.GetDefault();
        image = graphics.LoadReadOnlyTexture2D<Rgba32, float4>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Image1.jpg"));
        depth = graphics.LoadReadOnlyTexture2D<Rgba32, float4>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Depth1.jpg"));
        this.mouse = mouse;
    }

    public bool TryExecute(IReadWriteNormalizedTexture2D<float4> texture, TimeSpan timespan, object? parameter)
    {
        mouseOffset.X += (-0.075f * mouse().X - mouseOffset.X) * 0.08f;
        mouseOffset.Y += (-0.075f * mouse().Y - mouseOffset.Y) * 0.08f;
        texture.GraphicsDevice.ForEach(texture, new DepthParallax(image, depth, mouseOffset, intensity));

        return true;
    }
}
