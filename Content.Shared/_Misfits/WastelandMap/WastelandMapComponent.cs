using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Misfits.WastelandMap;

[Serializable, NetSerializable]
public enum WastelandMapUiKey : byte
{
    Key,
}

// keep for later tactical re-enable. Cataclysm14 base port returns no tactical blips
[Serializable, NetSerializable]
public enum WastelandMapTrackedBlipKind : byte
{
    Unknown,
    Elder,
    Paladin,
    Knight,
    Scribe,
    Squire,
    LegionCenturion,
    LegionDecanus,
    LegionWarrior,
    LegionRecruit,
    PipBoyContact,
    PipBoyGroupMember,
    TribalHuntTarget,
    DeadBody,
}

[Serializable, NetSerializable]
public enum WastelandMapAnnotationType : byte
{
    Marker,
    Box,
    Draw,
}

// keep so prototype data can be merged later without changing the component API
[Serializable, NetSerializable]
public enum WastelandMapTacticalFeedKind : byte
{
    None,
    Brotherhood,
    Vault,
    NCR,
    Enclave,
    Legion,
    Followers,
}

[Serializable, NetSerializable]
public readonly record struct WastelandMapTrackedBlip(
    float X,
    float Y,
    string Label,
    WastelandMapTrackedBlipKind Kind);

[Serializable, NetSerializable]
public readonly record struct WastelandMapAnnotation(
    WastelandMapAnnotationType Type,
    float StartX,
    float StartY,
    float EndX,
    float EndY,
    string Label,
    uint PackedColor,
    float StrokeWidth,
    float[]? StrokePoints)
{
    public const uint DefaultPackedColor = 0xF27F26FF;
    public const float DefaultStrokeWidth = 3f;
}

[Serializable, NetSerializable]
public sealed class WastelandMapBoundUserInterfaceState : BoundUserInterfaceState
{
    public readonly string MapTitle;
    public readonly string MapTexturePath;
    public readonly bool CompactHud;
    public readonly float BoundsLeft;
    public readonly float BoundsBottom;
    public readonly float BoundsRight;
    public readonly float BoundsTop;
    public readonly WastelandMapTrackedBlip[] TrackedBlips;
    public readonly WastelandMapAnnotation[] SharedAnnotations;

    public WastelandMapBoundUserInterfaceState(
        string mapTitle,
        string mapTexturePath,
        bool compactHud,
        float boundsLeft,
        float boundsBottom,
        float boundsRight,
        float boundsTop,
        WastelandMapTrackedBlip[]? trackedBlips = null,
        WastelandMapAnnotation[]? sharedAnnotations = null)
    {
        MapTitle = mapTitle;
        MapTexturePath = mapTexturePath;
        CompactHud = compactHud;
        BoundsLeft = boundsLeft;
        BoundsBottom = boundsBottom;
        BoundsRight = boundsRight;
        BoundsTop = boundsTop;
		TrackedBlips = trackedBlips ?? Array.Empty<WastelandMapTrackedBlip>();
		SharedAnnotations = sharedAnnotations ?? Array.Empty<WastelandMapAnnotation>();
    }
}
[Serializable, NetSerializable]
public sealed class WastelandMapAddAnnotationMessage : BoundUserInterfaceMessage
{
    public readonly WastelandMapAnnotation Annotation;

    public WastelandMapAddAnnotationMessage(WastelandMapAnnotation annotation)
    {
        Annotation = annotation;
    }
}

[Serializable, NetSerializable]
public sealed class WastelandMapClearAnnotationsMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class WastelandMapRemoveAnnotationMessage : BoundUserInterfaceMessage
{
    public readonly int Index;

    public WastelandMapRemoveAnnotationMessage(int index)
    {
        Index = index;
    }
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WastelandMapComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public ResPath MapTexturePath = default!;

    [DataField, AutoNetworkedField]
    public string MapTitle = "Map";

    [DataField]
    public Box2 WorldBounds = default;

    // tactical-feed fields are intentionally inert in the Cataclysm14 base port, probably wont even use ts   -pierow
    [DataField]
    public bool TrackBrotherhoodHolotags;

    [DataField]
    public WastelandMapTacticalFeedKind TacticalFeed;

    [DataField]
    public bool CompactHud;

    [DataField]
    public List<WastelandMapAnnotation> SharedAnnotations = new();
}
