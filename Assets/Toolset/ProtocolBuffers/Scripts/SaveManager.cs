using System;
using System.IO;
using UnityEngine;
using ProtoBuf;

/// <summary>
/// Static class that provides utility methods for saving/loading protobuf binaries to local storage.
/// </summary>
public static class SaveManager
{
    private const string c_filePathSuffix = "/{0}.bin";

    /// <summary>
    /// Saves the given Protobuf model instance. Note that only one file can be generated per Protobuf Model. 
    /// If repeated instances of data are needed, the pattern should be to make a parent model with a list of internal models to be serialized.
    /// </summary>
    /// <typeparam name="T">The type of Protobuf model to save.</typeparam>
    /// <param name="modelToSave">The instance of the Protobuf model to save.</param>
    public static void SaveModel<T>(T modelToSave) where T : class
    {
        if (!typeof(T).IsDefined(typeof(ProtoContractAttribute), true))
            throw new InvalidOperationException("[Toolset.SaveManager] Attempting to save data model {0} that does not have the ProtoContract Attribute."
                                                    .StringBuilderFormat(typeof(T).Name));

        string filePath = GetDataFilePathForType<T>();
        FileStream fileStream;

        if (File.Exists(filePath))
            fileStream = File.OpenWrite(filePath);
        else
            fileStream = File.Create(filePath);

        Serializer.Serialize(fileStream, modelToSave);
        fileStream.Close();
    }

    /// <summary>
    /// Loads the Protobuf model from a file in local save data if it exists. Otherwise returns null.
    /// </summary>
    /// <typeparam name="T">The type of Protobuf model to load.</typeparam>
    /// <returns>An instance of the Protobuf model or null.</returns>
    public static T LoadModelIfSaved<T>() where T : class
    {
        if (!typeof(T).IsDefined(typeof(ProtoContractAttribute), true))
            throw new InvalidOperationException("[Toolset.SaveManager] Attempting to load data model {0} that does not have the ProtoContract Attribute."
                                                    .StringBuilderFormat(typeof(T).Name));

        string filePath = GetDataFilePathForType<T>();

        if (!File.Exists(filePath))
            return null;

        FileStream fileStream = File.OpenRead(filePath);
        T output = Serializer.Deserialize<T>(fileStream);
        fileStream.Close();

        return output;
    }

    /// <summary>
    /// Gets the path in local storage to a save file for a given Protobuf model type. 
    /// </summary>
    /// <typeparam name="T">The type of Protobuf model to get a path for.</typeparam>
    /// <returns>The path to the save file in local storage.</returns>
    public static string GetDataFilePathForType<T>()
    {
        return Application.dataPath + (c_filePathSuffix.StringBuilderFormat(typeof(T).Name));
    }
}
