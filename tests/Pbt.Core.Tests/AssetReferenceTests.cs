using Pbt.Core.Infrastructure;
using Pbt.Core.Models;
using Pbt.Core.Services;
using Xunit;

namespace Pbt.Core.Tests;

public class AssetReferenceTests
{
    [Fact]
    public void ResolveAssetPaths_ExpandsCalculationGroupAndFieldParameterRefs()
    {
        var root = Path.Combine(Path.GetTempPath(), $"pbt_ref_{Guid.NewGuid()}");
        Directory.CreateDirectory(Path.Combine(root, "calculation_groups"));
        Directory.CreateDirectory(Path.Combine(root, "field_parameters"));
        try
        {
            File.WriteAllText(Path.Combine(root, "calculation_groups", "ti.yaml"),
                "name: Time Intelligence\nprecedence: 5\ncalculation_items:\n- name: YTD\n  expression: SELECTEDMEASURE()\n");
            File.WriteAllText(Path.Combine(root, "field_parameters", "axis.yml"),
                "name: Axis\nvalues:\n- name: Sales\n  expression: '[Sales]'\n");

            var model = new ModelDefinition
            {
                CalculationGroups = new() { new() { Ref = "Time Intelligence" } },
                FieldParameters = new() { new() { Ref = "Axis" } }
            };
            new AssetLoader(new YamlSerializer()).ResolveAssetPaths(model, root);

            Assert.Equal(5, model.CalculationGroups![0].Precedence);
            Assert.Single(model.CalculationGroups[0].CalculationItems);
            Assert.Equal("Axis", model.FieldParameters![0].Name);

            model.CalculationGroups = new() { new() { Ref = "Missing" } };
            Assert.Throws<InvalidOperationException>(() => new AssetLoader(new YamlSerializer()).ResolveAssetPaths(model, root));
        }
        finally { Directory.Delete(root, true); }
    }
}
