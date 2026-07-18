using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared._Cataclysm14.Crafting.Conditions;

[TypeSerializer]
public sealed class CraftingConditionTypeSerializer  : ITypeReader<CraftingCondition, MappingDataNode>
{
    private Type? GetType(MappingDataNode node)
    {
        if (node.Has("item"))
        {
            return typeof(ItemCondition);
        }

        if (node.Has("oneOf"))
        {
            return typeof(OneOfCondition);
        }

        if (node.Has("tool"))
        {
            return typeof(ToolCondition);
        }

        return null;
    }

    public ValidationNode Validate(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        var type = GetType(node);

        if (type == null)
            return new ErrorNode(node, "No crafting recipe condition type found.");

        return serializationManager.ValidateNode(type, node, context);
    }

    public CraftingCondition Read(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<CraftingCondition>? instanceProvider = null)
    {
        var type = GetType(node) ??
                   throw new ArgumentException(
                       "Tried to convert invalid YAML node mapping to CraftingCondition!");

        return (CraftingCondition)serializationManager.Read(type, node, hookCtx, context)!;
    }
}
