using Robust.Shared.GameStates;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;

namespace Content.Shared._Cataclysm14.MapGen;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState()]
public sealed partial class MapGenComponent : Component
{
    public const int ChunkSize = 16;

    [DataField] public FastNoiseLite Noise { get; private set; } = new(0);

    [DataField] public Box2i MapSize { get; private set; } = new(-1, -1, 1, 1);

    [DataField] public int Cities { get; private set; } = 1;

    [DataField] public int CitySizeInChunks { get; private set; } = 8; // 8x8

    public Dictionary<Vector2i, EntProtoId<MapGenChunkComponent>[][]> Chunks
    {
        get
        {
            if (_chunks != null)
                return _chunks;
            _chunks = new(MapSize.Height * MapSize.Width);
            for (var x = MapSize.Left; x <= MapSize.Right; x++)
            {
                for (var y = MapSize.Bottom; y <= MapSize.Top; y++)
                {
                    var arr = new EntProtoId<MapGenChunkComponent>[ChunkSize][];
                    for (var i = 0; i < ChunkSize; i++)
                    {
                        arr[i] = new EntProtoId<MapGenChunkComponent>[ChunkSize];
                    }
                    _chunks.Add(new (x, y), arr);
                }
            }
            return _chunks;
        }
    }

    [DataField, AutoNetworkedField] private Dictionary<Vector2i, EntProtoId<MapGenChunkComponent>[][]>? _chunks;
}
