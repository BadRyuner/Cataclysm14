using Content.Shared.FixedPoint;
using Robust.Shared.Map;

namespace Content.Server._Cataclysm14.Noise;

[RegisterComponent]
public sealed partial class NoiseListenerComponent : Component
{
    /// <summary>
    /// Noise source
    /// </summary>
    [ViewVariables]
    public EntityCoordinates? Target;

    [ViewVariables]
    public EntityCoordinates? PreviousTarget;

    [ViewVariables]
    public FixedPoint2 Timer;

    /// <summary>
    /// The time after which the mob should forget about the noise if it has not started to explore its source
    /// </summary>
    [DataField]
    public FixedPoint2 MaxTimer = FixedPoint2.New(15);
}
