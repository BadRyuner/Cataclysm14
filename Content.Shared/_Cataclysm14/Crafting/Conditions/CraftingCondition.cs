namespace Content.Shared._Cataclysm14.Crafting.Conditions;

[ImplicitDataDefinitionForInheritors]
public abstract partial class CraftingCondition
{
    public abstract bool IsMet();

    public abstract void OnCrafting();
}
