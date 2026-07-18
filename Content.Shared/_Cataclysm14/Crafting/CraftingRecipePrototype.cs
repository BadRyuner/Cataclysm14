using Content.Shared._Cataclysm14.Crafting.Conditions;
using Robust.Shared.Prototypes;

namespace Content.Shared._Cataclysm14.Crafting;

[Prototype("CraftingRecipe")]
public sealed partial class CraftingRecipePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public string Name { get; private set; } = default!;

    [DataField(required: true)]
    public ProtoId<CraftingSubCategoryPrototype> SubCategory { get; private set; }

    [DataField(required: true)]
    public EntProtoId Item { get; private set; }

    [DataField(required: true)]
    public float DoAfter { get; private set; }

    [DataField(required: true)]
    public List<CraftingCondition> Conditions { get; private set; } = default!;
}
