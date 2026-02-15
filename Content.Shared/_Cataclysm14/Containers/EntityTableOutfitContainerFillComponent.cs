using Content.Shared.Containers;
using Content.Shared.EntityTable.EntitySelectors;

namespace Content.Shared._Cataclysm14.Containers;

/// <summary>
/// Fills containers on MapInit using a combination of always-filled container slots
/// and weighted outfit sets where one is selected at random.
/// Container slots are filled first, then the selected outfit's slots are applied,
/// overriding any container slot with the same ID.
/// </summary>
[RegisterComponent, Access(typeof(ContainerFillSystem))]
public sealed partial class EntityTableOutfitContainerFillComponent : Component
{
    /// <summary>
    /// Slots that are always filled regardless of outfit selection.
    /// </summary>
    [DataField]
    public Dictionary<string, EntityTableSelector> Containers = new();

    /// <summary>
    /// List of outfit sets. One is picked randomly (weighted) on MapInit.
    /// Its slots are merged with Containers, overriding on conflict.
    /// </summary>
    [DataField]
    public List<OutfitSet> Outfits = new();
}

/// <summary>
/// A weighted set of container-to-entity-table mappings.
/// One OutfitSet is selected at random (by weight) during MapInit,
/// and its slots are used to fill the entity's containers.
/// </summary>
[DataDefinition]
public sealed partial class OutfitSet
{
    /// <summary>
    /// Relative weight for random selection. Higher values increase selection probability.
    /// </summary>
    [DataField]
    public float Weight = 1f;

    /// <summary>
    /// Mapping of container IDs to entity table selectors.
    /// These slots override any matching container slots when this outfit is selected.
    /// </summary>
    [DataField(required: true)]
    public Dictionary<string, EntityTableSelector> Slots = new();
}
