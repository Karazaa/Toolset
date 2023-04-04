using System;
using Toolset.Core;

/// <summary>
/// ToolsetGuid is a Universal ID system that also is typesafe for different types of records.
/// </summary>
public partial class ToolsetGuid : global::ProtoBuf.IExtensible
{
    public static bool operator==(ToolsetGuid guid, ToolsetGuid otherGuid)
    {
        return guid?.Type == otherGuid?.Type && guid?.Guid == otherGuid?.Guid;
    }
    
    public static bool operator!=(ToolsetGuid guid, ToolsetGuid otherGuid)
    {
        return guid?.Type != otherGuid?.Type || guid?.Guid != otherGuid?.Guid;
    }

    public override bool Equals(object obj)
    {
        ToolsetGuid otherGuid = obj as ToolsetGuid;
        if (otherGuid == null)
            return false;
        
        return Type == otherGuid.Type && Guid == otherGuid.Guid;
    }

    public override int GetHashCode()
    {
        return (Guid + Type).GetHashCode();
    }

    /// <summary>
    /// Constructs a new ToolsetGuid for the specified class type with the passed guid.
    /// </summary>
    /// <param name="guid">The string guid that will be the internal guid of the ToolsetGuid.</param>
    /// <typeparam name="TType">The type of the ToolsetGuid</typeparam>
    /// <returns></returns>
    public static ToolsetGuid ConstructForType<TType>(string guid) where TType : class
    {
        return new ToolsetGuid() { Guid = guid, Type = typeof(TType).Name };
    }
    
    /// <summary>
    /// Checks if the passed in generic type matches the type specified in the Type property of the ToolsetGuid.
    /// </summary>
    /// <typeparam name="TType">The model type to compare against</typeparam>
    /// <exception cref="InvalidOperationException">An invalid operation exception if the types don't match.</exception>
    public void CheckTypeMatch<TType>()
    {
        string modelType = typeof(TType).Name;
        if (modelType != Type)
            throw new InvalidOperationException("[Toolset.ToolsetGuid] Guid type {0} does not match model type {1}!"
            .StringBuilderFormat(Type, modelType));
    }
}
