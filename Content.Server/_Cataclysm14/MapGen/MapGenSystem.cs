using System.Linq;
using Content.Shared._Cataclysm14.MapGen;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Cataclysm14.MapGen;

public sealed class MapGenSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IComponentFactory _compFactory = default!;

    // ass hardcode
    private static readonly EntProtoId<MapGenChunkComponent> Forest = "StructureSpawnerForestBaseGrass13x13";
    private static readonly EntProtoId<MapGenChunkComponent> Marsh = "StructureSpawnerMarshBaseGrass13x13";
    private static readonly EntProtoId<MapGenChunkComponent> Plains = "StructureSpawnerPlainBaseGrass13x13";
    private static readonly EntProtoId<MapGenChunkComponent> Mountain = "StructureSpawnerMountainRandomBase13x13";

    private List<(EntProtoId<MapGenChunkComponent>, MapGenChunkComponent)> _allMapGenChunks = new(64);

    /// <inheritdoc/>
    public override void Initialize()
    {
        #if true
        SubscribeLocalEvent<MapGenComponent, ComponentInit>(OnCompInit); // for debugging purposes
        #else
        SubscribeLocalEvent<MapGenComponent, MapInitEvent>(OnMapInit);
        #endif

        var mapGenChunkCompName = _compFactory.GetComponentName<MapGenChunkComponent>();

        _allMapGenChunks.AddRange( _proto.EnumeratePrototypes<EntityPrototype>()
            .Where(p => p.Components.ContainsKey(mapGenChunkCompName))
            .Select(p => (new EntProtoId<MapGenChunkComponent>(p.ID), (MapGenChunkComponent)p.Components[mapGenChunkCompName].Component)));
    }

    private void OnCompInit(EntityUid uid, MapGenComponent component, ComponentInit args)
    {
        GenerateMap(uid, component);
    }

    private void OnMapInit(EntityUid uid, MapGenComponent component, MapInitEvent args)
    {
        GenerateMap(uid, component);
    }

    private void GenerateMap(EntityUid uid, MapGenComponent component)
    {
        var map = Comp<MapComponent>(uid).MapId;
        var chunks = component.Chunks;
        var noise = component.Noise;

        var minX = component.MapSize.Left * MapGenComponent.ChunkSize;
        var maxX = (component.MapSize.Right + 1) * MapGenComponent.ChunkSize; // ass fix
        var minY = component.MapSize.Bottom * MapGenComponent.ChunkSize;
        var maxY = (component.MapSize.Top + 1) * MapGenComponent.ChunkSize; // ass fix

        var rot = new Angle[maxX - minX, maxY - minY];

        noise.SetSeed(_random.Next());

        // cities
        for (var cityNum = 0; cityNum < component.Cities; cityNum++)
        {
            var cityX = _random.Next(minX, maxX - component.CitySizeInChunks);
            var cityY = _random.Next(minY, maxY - component.CitySizeInChunks);

            for (var x = 0; x < component.CitySizeInChunks; x++)
            {
                for (var y = 0; y < component.CitySizeInChunks; y++)
                {
                    ref var chunk = ref GetChunk(cityX + x, cityY + y);
                    var placeRoadHor = (x - 1) % 3 == 0;
                    var placeRoadVer = (y - 1) % 3 == 0;
                    if (placeRoadHor && placeRoadVer)
                    {
                        chunk = "StructureSpawnerRoadGrayWhiteSidewalkBaseFourway13x13";
                    }
                    else if (placeRoadHor)
                    {
                        chunk = "StructureSpawnerRoadGrayWhiteSidewalkBaseStraight13x13";
                    }
                    else if (placeRoadVer)
                    {
                        chunk = "StructureSpawnerRoadGrayWhiteSidewalkBaseStraight13x13";
                        GetRot(cityX + x, cityY + y) = new Angle(Math.PI / 2);
                    }
                    else
                    {
                        chunk = "StructureSpawnerHouseSmall26x26";
                    }
                }
            }
        }

        // forest and etc
        for (var x = minX; x < maxX; x++)
        {
            for (var y = minY; y < maxY; y++)
            {
                ref var chunk = ref GetChunk(x, y);
                if (!string.IsNullOrEmpty(chunk.Id)) // already spawned things
                    continue;

                chunk = noise.GetNoise(x, y) switch // between -1 and +1
                {
                    // i love c# switch syntax
                    <= -0.38f => Marsh,
                    >= -0.42f and <= 0.17f => Forest,
                    >= 0.7f => Mountain,
                    _ => Plains,
                };
            }
        }

        // spaaaaaaaaaaawn
        for (var x = minX; x < maxX; x++)
        {
            for (var y = minY; y < maxY; y++)
            {
                SpawnChunk(x, y);
            }
        }

        Dirty(uid, component);

        ref EntProtoId<MapGenChunkComponent> GetChunk(int x, int y)
        {
            var bigChunk = ToBigChunk(x, y);
            return ref chunks[bigChunk][x - bigChunk.X * MapGenComponent.ChunkSize][y - bigChunk.Y * MapGenComponent.ChunkSize];
        }

        ref Angle GetRot(int x, int y)
        {
            return ref rot[x - minX, y - minY];
        }

        Vector2i ToBigChunk(int x, int y)
        {
            return new Vector2i((int)MathF.Round((float)x / MapGenComponent.ChunkSize, 0, MidpointRounding.ToNegativeInfinity),
                (int)MathF.Round((float)y / MapGenComponent.ChunkSize, 0, MidpointRounding.ToNegativeInfinity));
        }

        void SpawnChunk(int x, int y)
        {
            var bigChunk = ToBigChunk(x, y);
            var chunk = chunks[bigChunk][x - bigChunk.X * MapGenComponent.ChunkSize][y - bigChunk.Y * MapGenComponent.ChunkSize];
            Spawn(chunk, new MapCoordinates(x * 13f + (13f / 2), y * 13f + (13f / 2), map), null, GetRot(x, y));
        }
    }
}
