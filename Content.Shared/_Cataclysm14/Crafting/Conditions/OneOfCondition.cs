namespace Content.Shared._Cataclysm14.Crafting.Conditions;

public sealed partial class OneOfCondition : CraftingCondition
{
    [DataField(required: true)]
    public List<CraftingCondition> Conditions { get; private set; }

    public override bool IsMet()
    {
        for (var i = 0; i < Conditions.Count; i++)
        {
            if (Conditions[i].IsMet())
                return true;
        }

        return false;
    }

    public override void OnCrafting()
    {

    }
}
