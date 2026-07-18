using Robust.Shared.Prototypes;

namespace Content.Shared._Cataclysm14.Crafting;

[Prototype("CraftingSubCategory")]
public sealed partial class CraftingSubCategoryPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public ProtoId<CraftingCategoryPrototype> Parent { get; private set; }
}
