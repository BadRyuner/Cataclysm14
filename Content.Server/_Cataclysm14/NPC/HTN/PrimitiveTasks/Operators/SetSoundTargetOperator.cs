using System.Threading;
using System.Threading.Tasks;
using Content.Server._Cataclysm14.Noise;
using Content.Server.NPC;
using Content.Server.NPC.Components;
using Content.Server.NPC.HTN.PrimitiveTasks;

namespace Content.Server._Cataclysm14.NPC.HTN.PrimitiveTasks.Operators;

public sealed partial class SetSoundTargetOperator : HTNOperator
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    private EntityQuery<NoiseListenerComponent> _noiseListenerQuery = default!;
    private EntityQuery<NPCSteeringComponent> _npcSteeringQuery = default!;

    public override void Initialize(IEntitySystemManager sysManager)
    {
        base.Initialize(sysManager);
        _noiseListenerQuery = _entManager.GetEntityQuery<NoiseListenerComponent>();
        _npcSteeringQuery = _entManager.GetEntityQuery<NPCSteeringComponent>();
    }

    public override async Task<(bool Valid, Dictionary<string, object>? Effects)> Plan(NPCBlackboard blackboard, CancellationToken cancelToken)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        if (_noiseListenerQuery.TryGetComponent(owner, out var noiseListener) && noiseListener.Target != null)
        {
            if (noiseListener.PreviousTarget != noiseListener.Target)
            {
                noiseListener.PreviousTarget = noiseListener.Target;
                if (_noiseListenerQuery.HasComp(owner))
                    _entManager.RemoveComponent<NPCSteeringComponent>(owner); // shitcode force to recalc path
            }
            return (true, new()
            {
                { "SoundCoordinates", noiseListener.Target }
            });
        }

        return (false, new());
    }
}
