using StrideShaderDemo.Rendering.TextureParameters;
using Stride.Core;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Shaders;
using System.Collections.Generic;
using Stride.Core.Annotations;

namespace StrideShaderDemo.Rendering;

[DataContract]
[Display("Wave Surface Normal")]
public class MaterialWaveSurfaceNormalFeature : MaterialFeature, IMaterialSurfaceFeature
{
    [DataMember(0)]
    public Texture NormalMap { get; set; }

    /// <summary>
    /// The amount of influence the wave's position displacement (expansion/contraction) has on the sample point (the texcoord).
    /// </summary>
    [DataMember(5)]
    [DataMemberRange(minimum: 0, maximum: 1, smallStep: 0.01, largeStep: 0.1, decimalPlaces: 2)]
    public float PositionDisplacementInfluence { get; set; } = 0.25f;

    [DataMember(20)]
    [Display("Normals")]
    public List<WavePanningNormalMap> Normals { get; set; } = new();

    public override void GenerateShader(MaterialGeneratorContext context)
    {
        if (NormalMap is not null && Normals?.Count > 0)
        {
            // Inform the context that we are using matNormal (from the MaterialSurfaceNormalMap shader)
            context.UseStreamWithCustomBlend(MaterialShaderStage.Pixel, "matNormal", new ShaderClassSource("MaterialStreamNormalBlend"));
            context.Parameters.Set(MaterialKeys.HasNormalMap, true);

            context.Parameters.Set(ComputeWaveNormalPanningUvKeys.NormalMap, NormalMap);
            context.Parameters.Set(MaterialWaveSurfaceNormalKeys.PositionDisplacementInfluence, PositionDisplacementInfluence);

            var mixin = new ShaderMixinSource();
            mixin.Mixins.Add(new ShaderClassSource("MaterialWaveSurfaceNormal"));

            bool isDisplacementEnabled = context.Parameters.Get(MaterialWaveDisplacementFeature.IsFeatureEnabled);
            if (isDisplacementEnabled)
            {
                mixin.AddMacro("MATERIAL_DISPLACEMENT_ENABLED", 1);
            }

            const string ComposeShaderArrayPropertyName = "SurfaceNormalFunctions";     // Name of property array in MaterialWaveSurfaceNormal.sdsl
            foreach (var norm in Normals)
            {
                if (norm is null)
                {
                    continue;
                }
                var shaderSource = norm.GenerateShaderSource();
                mixin.AddCompositionToArray(ComposeShaderArrayPropertyName, shaderSource);
            }
            context.AddShaderSource(MaterialShaderStage.Pixel, mixin);
        }
    }
}
