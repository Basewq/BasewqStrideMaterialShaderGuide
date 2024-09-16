using Stride.Core;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Shaders;
using StrideShaderDemo.Rendering.DisplacementParameters;
using System.Collections.Generic;

namespace StrideShaderDemo.Rendering;

[DataContract]
[Display("Wave Displacement")]
public class MaterialWaveDisplacementFeature : MaterialFeature, IMaterialDisplacementFeature
{
    // Special material key, used by MaterialWaveDisplacementFeature to tell MaterialWaveDisplacementFeature it can read the output stream variable.
    // There is a quirk where streams variables passing through different stages (vertex -> pixel) will crash if the variables in the prior
    // stage is not set. ie. If the Displacement feature is not enabled (in vertex stage), the Surface Normal feature (in pixel stage)
    // will crash. This key makes MaterialWaveSurfaceNormalFeature allow reading streams.WaveDisplacementPositionOffset in MaterialWaveSurfaceNormal shader
    // only if it has been flagged by MaterialWaveDisplacementFeature.
    public static readonly PermutationParameterKey<bool> IsFeatureEnabled = ParameterKeys.NewPermutation<bool>();

    [DataMember(10)]
    [Display("Waves")]
    public List<WaveDisplacementBase> Waves { get; set; } = new();

    public override void GenerateShader(MaterialGeneratorContext context)
    {
        if (Waves?.Count > 0)
        {
            context.Parameters.Set(IsFeatureEnabled, true);

            var mixin = new ShaderMixinSource();
            mixin.Mixins.Add(new ShaderClassSource("MaterialWaveDisplacement"));

            const string ComposeShaderArrayPropertyName = "DisplacementFunctions";     // Name of property array in MaterialWaveDisplacement.sdsl
            foreach (var wave in Waves)
            {
                if (wave is null)
                {
                    continue;
                }
                var shaderSource = wave.GenerateShaderSource();
                mixin.AddCompositionToArray(ComposeShaderArrayPropertyName, shaderSource);
            }
            context.AddShaderSource(MaterialShaderStage.Vertex, mixin);
        }
    }
}
