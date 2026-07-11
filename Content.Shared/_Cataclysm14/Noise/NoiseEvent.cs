using Robust.Shared.Map;

namespace Content.Shared._Cataclysm14.Noise;

[ByRefEvent]
public record struct NoiseEvent(EntityCoordinates Source, float Strength);
