using Content.Shared._Cataclysm14.Forage;
using Robust.Client.GameObjects;

namespace Content.Client._Cataclysm14.Forage;

public sealed class ForageableVisualizerSystem : VisualizerSystem<ForageableComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, ForageableComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (args.AppearanceData.TryGetValue(ForageableVisuals.State, out var state) && state is string stateName)
            SpriteSystem.LayerSetRsiState((uid, args.Sprite), 0, stateName);
        else // fallback
            SpriteSystem.LayerSetRsiState((uid, args.Sprite), 0, component.ReadyToForage ? component.ForageableState : component.HarvestedState);
    }
}
