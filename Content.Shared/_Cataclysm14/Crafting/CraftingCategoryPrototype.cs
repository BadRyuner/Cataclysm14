using Robust.Shared.Prototypes;

namespace Content.Shared._Cataclysm14.Crafting;

[Prototype("CraftingCategory")]
public sealed partial class CraftingCategoryPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;
}
