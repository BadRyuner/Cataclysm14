using Content.Server.Spawners.EntitySystems;
using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Prototypes;

namespace Content.Server.Spawners.Components;

[RegisterComponent, EntityCategory("Spawner"), Access(typeof(ConditionalSpawnerSystem))]
public sealed partial class EntityTableSpawnerComponent : Component
{
    /// <summary>
    /// Table that determines what gets spawned.
    /// </summary>
    [DataField(required: true)]
    public EntityTableSelector Table = default!;

    //Cataclysm14, >>
    /// <summary>
    /// Whether the result entity should be rotated with the spawner.
    /// </summary>
    [DataField]
    public bool Rotate = false;

    //Cataclysm14, >>
    /// <summary>
    /// Whether the result entity should have a random rotation, overrides spawner rotation and doesn't require it.
    /// </summary>
    [DataField]
    public bool RandomRotation = false;

    /// <summary>
    /// Scatter of entity spawn coordinates
    /// </summary>
    [DataField]
    public float Offset = 0.2f;

    /// <summary>
    /// A variable meaning whether the spawn will
    /// be able to be used again or whether
    /// it will be destroyed after the first use
    /// </summary>
    [DataField]
    public bool DeleteSpawnerAfterSpawn = true;
}

