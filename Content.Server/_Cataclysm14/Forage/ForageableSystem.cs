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
    [Dependency] private readonly SharedMapSystem _map = default!;

    private readonly HashSet<Entity<ForageableComponent>> _respawnTargets = new(128);
    private readonly List<Entity<ForageableComponent>> _processRespawnTargets = new(128);

    private float _respawnTimer = 0f;
    private float _respawnCooldown = 0f;
    private int _respawnMaxPerOnce = 30;
    private int _alreadyRespawned = 0;
    private bool _respawnProcessing = false;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ForageableComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<ForageableComponent, ComponentShutdown>(OnComponentShutdown);

        Subs.CVar(_cfgManager, CataCCVars.ForageRespawnCooldown, value => _respawnCooldown = value, true);
        Subs.CVar(_cfgManager, CataCCVars.ForageMaxRespawnPerOnce, value => _respawnMaxPerOnce = value, true);
    }

    private void OnComponentInit(EntityUid uid, ForageableComponent component, ComponentInit args)
    {
        if (_map.IsInitialized(Transform(uid).MapID)) // skip if it`s entity from mapping or frozen or uninit map
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
        if (!_respawnProcessing)
        {
            _respawnTimer += frameTime;
            if (_respawnTimer < _respawnCooldown)
                return;

            _respawnTimer = 0f;
            _respawnProcessing = true;
            _processRespawnTargets.EnsureCapacity(_respawnTargets.Capacity);
            _processRespawnTargets.AddRange(_respawnTargets);
        }

        var amount = Math.Min(_alreadyRespawned + _respawnMaxPerOnce, _processRespawnTargets.Count);
        for (var i = _alreadyRespawned; i < amount; i++)
        {
            var target = _processRespawnTargets[i];
            Respawn(target.Owner, target.Comp);
        }

        _alreadyRespawned = amount;

        if (_alreadyRespawned == _processRespawnTargets.Count)
        {
            _processRespawnTargets.Clear();
            _alreadyRespawned = 0;
            _respawnProcessing = false;
        }
    }
}
