using Robust.Shared.Prototypes;

namespace Content.Shared._Cataclysm14.Crafting.Conditions;

public sealed partial class ItemCondition : CraftingCondition
{
    [DataField(required: true)]
    public EntProtoId Item { get; private set; }

    [DataField(required: true)]
    public uint Amount = 1;

    public override bool IsMet()
    {
        return false;
    }

    public override void OnCrafting()
    {

    }
}
