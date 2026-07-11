using Content.Server._Cataclysm14.Noise;
using Content.Server.NPC;
using Content.Server.NPC.HTN.Preconditions;

namespace Content.Server._Cataclysm14.NPC.HTN.Preconditions;

public sealed partial class SoundTargetExistsPrecondition : HTNPrecondition
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    public override bool IsMet(NPCBlackboard blackboard)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);
        return _entManager.TryGetComponent(owner, out NoiseListenerComponent? noiseListener) && noiseListener.Target != null;
    }
}
