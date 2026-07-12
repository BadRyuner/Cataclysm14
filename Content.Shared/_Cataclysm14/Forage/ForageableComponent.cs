using Content.Shared.Destructible.Thresholds;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Cataclysm14.Forage;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true)]
public sealed partial class ForageableComponent : Component
{
    [DataField]
    public string HarvestedState = "harvested";

    [DataField]
    public string ForageableState = "summer";

    [DataField, AutoNetworkedField]
    public bool ReadyToForage = true;

    [DataField, AutoNetworkedField]
    public int SpawnAmount = 1;

    [DataField]
    public int MaxSpawnAmount = 1;

    [DataField]
    public EntProtoId FoodProto;

    [DataField("sound", required: true)] public SoundSpecifier ForageSound { get; set; } = default!;
}
