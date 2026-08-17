using Robust.Shared.Serialization;

namespace Content.Shared._Cataclysm14.MapGen;

[RegisterComponent]
public sealed partial class MapGenChunkComponent : Component
{
    [DataField] public RoadConnection RoadConnection = RoadConnection.No;
    [DataField] public string Char = "?";
    [DataField] public Color CharColor = Color.Red;
}

[Serializable, NetSerializable]
public enum RoadConnection
{
    No,
    South,
    North,
    West,
    East,
}
