// Uses base namespace to extend ContainerFillSystem
using System.Numerics;
using Content.Shared._Cataclysm14.Containers;
using Content.Shared.EntityTable;
using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.Random.Helpers;
using Robust.Shared.Containers;
using Robust.Shared.Map;

namespace Content.Shared.Containers;

public sealed partial class ContainerFillSystem
{
    private void OnOutfitMapInit(Entity<EntityTableOutfitContainerFillComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp(ent, out ContainerManagerComponent? containerComp))
            return;

        if (TerminatingOrDeleted(ent) || !Exists(ent))
            return;

        var comp = ent.Comp;

        if (comp.Containers.Count == 0 && comp.Outfits.Count == 0)
        {
            Log.Error(
                $"Entity {ToPrettyString(ent)} with a {nameof(EntityTableOutfitContainerFillComponent)} " +
                "has no containers or outfit sets defined.");
            return;
        }

        // Start with base containers, then overlay the selected outfit's slots.
        var merged = new Dictionary<string, EntityTableSelector>(comp.Containers);

        if (comp.Outfits.Count > 0)
        {
            var weights = new Dictionary<OutfitSet, float>(comp.Outfits.Count);
            foreach (var outfit in comp.Outfits)
            {
                weights[outfit] = outfit.Weight;
            }

            // Weighted pick via SharedRandomExtensions, not the uniform IReadOnlyCollection overload.
            var selected = _random.Pick(weights);

            // Outfit slots override container slots on conflict.
            foreach (var (containerId, table) in selected.Slots)
            {
                merged[containerId] = table;
            }
        }

        var xform = Transform(ent);
        var coords = new EntityCoordinates(ent, Vector2.Zero);

        foreach (var (containerId, table) in merged)
        {
            if (!_containerSystem.TryGetContainer(ent, containerId, out var container, containerComp))
            {
                Log.Error(
                    $"Entity {ToPrettyString(ent)} with a {nameof(EntityTableOutfitContainerFillComponent)} " +
                    $"is missing a container ({containerId}).");
                continue;
            }

            var spawns = _entityTable.GetSpawns(table);
            foreach (var proto in spawns)
            {
                var spawn = Spawn(proto, coords);
                if (!_containerSystem.Insert(spawn, container, containerXform: xform))
                {
                    Log.Error(
                        $"Entity {ToPrettyString(ent)} with a {nameof(EntityTableOutfitContainerFillComponent)} " +
                        $"failed to insert an entity: {ToPrettyString(spawn)}.");
                    _transform.AttachToGridOrMap(spawn);
                    break;
                }
            }
        }
    }
}
