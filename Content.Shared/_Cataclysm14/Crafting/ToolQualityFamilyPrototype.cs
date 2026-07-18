using Content.Shared.Tools;
using Robust.Shared.Prototypes;

namespace Content.Shared._Cataclysm14.Crafting;

[Prototype("ToolQualityFamily")]
public sealed partial class ToolQualityFamilyPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("family", required: true)]
    public Dictionary<ProtoId<ToolQualityPrototype>, byte> FamilyByProto { get; private set; } = default!;

    public Dictionary<byte, ProtoId<ToolQualityPrototype>> FamilyByQuality
    {
        get
        {
            if (_reverseFamily != null)
                return _reverseFamily;

            _reverseFamily = new(FamilyByProto.Count);
            foreach (var (proto, qua) in FamilyByProto)
            {
                _reverseFamily.Add(qua, proto);
            }
            return _reverseFamily;
        }
    }

    private Dictionary<byte, ProtoId<ToolQualityPrototype>>? _reverseFamily;
}
