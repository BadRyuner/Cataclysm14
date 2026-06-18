using Content.Shared._Misfits.WastelandMap;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;
using Robust.Shared.Map;

namespace Content.Server._Misfits.WastelandMap;

public sealed class WastelandMapSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;

    private const int MaxSharedAnnotations = 128;
    private const int MaxStrokePoints = 512;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WastelandMapComponent, AfterActivatableUIOpenEvent>(OnAfterOpen);
        SubscribeLocalEvent<WastelandMapComponent, WastelandMapAddAnnotationMessage>(OnAddAnnotationMessage);
        SubscribeLocalEvent<WastelandMapComponent, WastelandMapRemoveAnnotationMessage>(OnRemoveAnnotationMessage);
        SubscribeLocalEvent<WastelandMapComponent, WastelandMapClearAnnotationsMessage>(OnClearAnnotationsMessage);
    }

    private void OnAfterOpen(EntityUid uid, WastelandMapComponent comp, AfterActivatableUIOpenEvent args)
    {
        var userMap = Transform(args.User).MapID;
        _uiSystem.SetUiState(uid, WastelandMapUiKey.Key, BuildState(comp, userMap, args.User));
    }

    private void OnAddAnnotationMessage(EntityUid uid, WastelandMapComponent comp, WastelandMapAddAnnotationMessage args)
    {
        if (!TryAddAnnotation(args.Actor, comp, Transform(args.Actor).MapID, args.Annotation))
            return;

        UpdateMapUi(uid, comp, Transform(args.Actor).MapID);
    }

    private void OnRemoveAnnotationMessage(EntityUid uid, WastelandMapComponent comp, WastelandMapRemoveAnnotationMessage args)
    {
        if (!TryRemoveAnnotation(args.Actor, comp, Transform(args.Actor).MapID, args.Index))
            return;

        UpdateMapUi(uid, comp, Transform(args.Actor).MapID);
    }

    private void OnClearAnnotationsMessage(EntityUid uid, WastelandMapComponent comp, WastelandMapClearAnnotationsMessage args)
    {
        if (!TryClearAnnotations(args.Actor, comp, Transform(args.Actor).MapID))
            return;

        UpdateMapUi(uid, comp, Transform(args.Actor).MapID);
    }

    public WastelandMapBoundUserInterfaceState BuildState(
        WastelandMapComponent comp,
        MapId mapId,
        EntityUid? actor = null)
    {
        return new WastelandMapBoundUserInterfaceState(
            comp.MapTitle,
            comp.MapTexturePath.ToString(),
            comp.CompactHud,
            comp.WorldBounds.Left,
            comp.WorldBounds.Bottom,
            comp.WorldBounds.Right,
            comp.WorldBounds.Top,
            Array.Empty<WastelandMapTrackedBlip>(),
            comp.SharedAnnotations.ToArray());
    }

    public bool TryAddAnnotation(EntityUid actor, WastelandMapComponent comp, MapId mapId, WastelandMapAnnotation annotation)
    {
        var sanitized = SanitizeAnnotation(annotation);
        if (sanitized == null)
            return false;

        comp.SharedAnnotations.Add(sanitized.Value);

        if (comp.SharedAnnotations.Count > MaxSharedAnnotations)
            comp.SharedAnnotations.RemoveAt(0);

        return true;
    }

    public bool TryRemoveAnnotation(EntityUid actor, WastelandMapComponent comp, MapId mapId, int index)
    {
        if (index < 0 || index >= comp.SharedAnnotations.Count)
            return false;

        comp.SharedAnnotations.RemoveAt(index);
        return true;
    }

    public bool TryClearAnnotations(EntityUid actor, WastelandMapComponent comp, MapId mapId)
    {
        if (comp.SharedAnnotations.Count == 0)
            return false;

        comp.SharedAnnotations.Clear();
        return true;
    }

    private void UpdateMapUi(EntityUid uid, WastelandMapComponent comp, MapId? mapId = null)
    {
        if (!TryComp<UserInterfaceComponent>(uid, out var ui))
            return;

        _uiSystem.SetUiState((uid, ui), WastelandMapUiKey.Key, BuildState(comp, mapId ?? Transform(uid).MapID));
    }

    private static WastelandMapAnnotation? SanitizeAnnotation(WastelandMapAnnotation annotation)
    {
        if (annotation.Type != WastelandMapAnnotationType.Marker &&
            annotation.Type != WastelandMapAnnotationType.Box &&
            annotation.Type != WastelandMapAnnotationType.Draw)
            return null;

        var label = annotation.Label.Trim();

        if (label.Length > 64)
            label = label.Substring(0, 64).TrimEnd();

        if (annotation.Type == WastelandMapAnnotationType.Draw)
        {
            var pts = annotation.StrokePoints;
            if (pts == null || pts.Length < 4)
                return null;

            var count = Math.Min(pts.Length & ~1, MaxStrokePoints);
            var sanitizedPts = new float[count];

            for (var i = 0; i < count; i++)
                sanitizedPts[i] = Math.Clamp(pts[i], 0f, 1f);

            if (string.IsNullOrWhiteSpace(label))
                label = "Drawing";

            return new WastelandMapAnnotation(
                WastelandMapAnnotationType.Draw,
                0f,
                0f,
                0f,
                0f,
                label,
                annotation.PackedColor,
                Math.Clamp(annotation.StrokeWidth, 1f, 12f),
                sanitizedPts);
        }

        var startX = Math.Clamp(annotation.StartX, 0f, 1f);
        var startY = Math.Clamp(annotation.StartY, 0f, 1f);
        var endX = Math.Clamp(annotation.EndX, 0f, 1f);
        var endY = Math.Clamp(annotation.EndY, 0f, 1f);

        if (string.IsNullOrWhiteSpace(label))
            label = annotation.Type == WastelandMapAnnotationType.Marker ? "Marker" : "Box";

        return new WastelandMapAnnotation(
            annotation.Type,
            startX,
            startY,
            endX,
            endY,
            label,
            annotation.PackedColor,
            Math.Clamp(annotation.StrokeWidth, 1f, 12f),
            null);
    }
}