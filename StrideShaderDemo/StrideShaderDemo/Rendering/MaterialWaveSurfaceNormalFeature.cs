using StrideShaderDemo.Rendering.TextureParameters;
using Stride.Core;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Shaders;
using System.Collections.Generic;

namespace StrideShaderDemo.Rendering;

[DataContract]
[Display("Wave Surface Normal")]
public class MaterialWaveSurfaceNormalFeature : MaterialFeature, IMaterialSurfaceFeature
{
    [DataMember(0)]
    public Texture NormalMap { get; set; }

    [DataMember(20)]
    [Display("Normals")]
    public List<WavePanningNormalMap> Normals { get; set; } = new();

    public override void GenerateShader(MaterialGeneratorContext context)
    {
        if (Normals?.Count > 0)
        {
            // Inform the context that we are using matNormal (from the MaterialSurfaceNormalMap shader)
            context.UseStreamWithCustomBlend(MaterialShaderStage.Pixel, "matNormal", new ShaderClassSource("MaterialStreamNormalBlend"));
            context.Parameters.Set(MaterialKeys.HasNormalMap, true);   

            if (NormalMap is not null)
            {
                context.Parameters.Set(ComputeWaveNormalPanningUvKeys.NormalMap, NormalMap);
            }

            var mixin = new ShaderMixinSource();
            mixin.Mixins.Add(new ShaderClassSource("MaterialWaveSurfaceNormal"));

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
