using Content.Shared.Tools;
using Robust.Shared.Prototypes;

namespace Content.Shared._Cataclysm14.Crafting.Conditions;

public sealed partial class ToolCondition : CraftingCondition
{
    [DataField(required: true)]
    public ProtoId<ToolQualityPrototype> Tool { get; private set; }

    public override bool IsMet()
    {
        return false;
    }

    public override void OnCrafting()
    {

    }
}
