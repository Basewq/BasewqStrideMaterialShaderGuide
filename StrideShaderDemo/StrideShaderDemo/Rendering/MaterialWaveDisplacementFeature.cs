using StrideShaderDemo.Rendering.DisplacementParameters;
using Stride.Core;
using Stride.Rendering.Materials;
using Stride.Shaders;
using System.Collections.Generic;

namespace StrideShaderDemo.Rendering;

[DataContract]
[Display("Wave Displacement")]
public class MaterialWaveDisplacementFeature : MaterialFeature, IMaterialDisplacementFeature
{
    [DataMember(10)]
    [Display("Waves")]
    public List<WaveDisplacementBase> Waves { get; set; } = new();

    public override void GenerateShader(MaterialGeneratorContext context)
    {
        if (Waves?.Count > 0)
        {
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
