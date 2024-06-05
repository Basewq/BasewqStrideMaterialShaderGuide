using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Rendering;
using Stride.Rendering.Materials;

namespace StrideShaderDemo.Rendering;

[DataContract]
[Display("Water Color")]
public class MaterialWaterColorFeature : MaterialTransparencyBlendFeature
{
    public Color3 WaterColor { get; set; }
    /// <summary>
    /// Color of the water when viewed at an acute angle.
    /// </summary>
    public Color3 FresnelColor { get; set; }

    /// <summary>
    /// Higher value = more opaque in deeper water.
    /// </summary>
    public float BeersLawFactor { get; set; } = 0.15f;
    /// <summary>
    /// Offset for when the 'opaque' transition begins.
    /// </summary>
    public float DepthOffset { get; set; } = 0.05f;

    public bool IsWaterDistortionEnabled { get; set; } = true;
    public float WaterDistortionScale { get; set; } = 1f;

    public bool IsWaterEdgeEnabled { get; set; } = true;
    public Color3 WaterEdgeColor { get; set; } = new(1, 1, 1);
    public float WaterEdgeDepthDiffThreshold { get; set; } = 0.1f;

    public override void GenerateShader(MaterialGeneratorContext context)
    {
        base.GenerateShader(context);

        context.Parameters.Set(WaterColorTransparencyKeys.WaterColor, WaterColor);
        context.Parameters.Set(WaterColorTransparencyKeys.FresnelColor, FresnelColor);

        context.Parameters.Set(WaterColorTransparencyKeys.BeersLawFactor, BeersLawFactor);
        context.Parameters.Set(WaterColorTransparencyKeys.DepthOffset, DepthOffset);

        context.Parameters.Set(WaterColorTransparencyKeys.IsWaterDistortionEnabled, IsWaterDistortionEnabled);
        context.Parameters.Set(WaterColorTransparencyKeys.WaterDistortionScale, WaterDistortionScale);

        context.Parameters.Set(WaterColorTransparencyKeys.IsWaterEdgeEnabled, IsWaterEdgeEnabled);
        context.Parameters.Set(WaterColorTransparencyKeys.WaterEdgeColor, WaterEdgeColor);
        context.Parameters.Set(WaterColorTransparencyKeys.WaterEdgeDepthDiffThreshold, WaterEdgeDepthDiffThreshold);
    }
}
