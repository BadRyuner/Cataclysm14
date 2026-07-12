using Content.Shared._Cataclysm14.CCVar;
using Content.Shared._Cataclysm14.Forage;
using Robust.Shared.Configuration;
using Robust.Shared.Random;
using Robust.Shared.Threading;

namespace Content.Server._Cataclysm14.Forage;

public sealed class ForageableSystem : SharedForageableSystem
{
    [Dependency] private readonly IConfigurationManager _cfgManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IParallelManager _parallel = default!;

    private readonly HashSet<Entity<ForageableComponent>> _respawnTargets = new(128);

    private float _respawnTimer = 0f;
    private float _respawnCooldown = 0f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ForageableComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<ForageableComponent, ComponentShutdown>(OnComponentShutdown);

        Subs.CVar(_cfgManager, CataCCVars.ForageRespawnCooldown, value => _respawnCooldown = value, true);
    }

    private void OnComponentInit(EntityUid uid, ForageableComponent component, ComponentInit args)
    {
        Respawn(uid, component, true);
    }

    private void OnComponentShutdown(EntityUid uid, ForageableComponent component, ComponentShutdown args)
    {
        _respawnTargets.Remove((uid, component));
    }

    private void Respawn(EntityUid uid, ForageableComponent component, bool onCompInit = false)
    {
        component.SpawnAmount = _random.Next(0, component.MaxSpawnAmount + 1); // (min included)...(max excluded)
        component.ReadyToForage = component.SpawnAmount != 0;

        if (component.ReadyToForage)
            _respawnTargets.Remove((uid, component));
        else if (onCompInit)
            _respawnTargets.Add((uid, component));

        Dirty(uid, component);

        Appearance.SetData(uid, ForageableVisuals.State, component.ReadyToForage ? component.ForageableState : component.HarvestedState);
    }

    protected override void QueryRespawn(EntityUid uid, ForageableComponent component)
    {
        _respawnTargets.Add((uid, component));
    }

    public override void Update(float frameTime)
    {
        _respawnTimer += frameTime;
        if (_respawnTimer < _respawnCooldown)
            return;

        _respawnTimer = 0f;
        foreach (var respawnTarget in _respawnTargets)
        {
            Respawn(respawnTarget.Owner, respawnTarget.Comp);
        }
    }
}
