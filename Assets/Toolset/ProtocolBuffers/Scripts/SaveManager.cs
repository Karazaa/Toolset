using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

/// <summary>
/// Static class that provides utility methods for saving/loading protobuf binaries to local storage.
/// </summary>
public static class SaveManager
{
    private const string c_directoryFormat = "{0}/{1}";
    private const string c_fileFormat = "{0}/{1}.bin";
    private const string c_searchPattern = "*.bin";

    /// <summary>
    /// Saves the instance of a class to a specific filename.
    /// Does not support directories within the filename yet. 
    /// Class must have the ProtoContract Attribute.
    /// </summary>
    /// <typeparam name="T">The class type to save. Files are saved in subdirectories grouped by type.</typeparam>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="modelToSave">The instance of the class to save.</param>
    public static void SaveModel<T>(string fileName, T modelToSave) where T : class
    {
        ValidateAttribute<T>("save");

        string filePath = GetDataFilePathForType<T>(fileName);
        Directory.CreateDirectory(GetDataDirectoryPathForType<T>());

        if (File.Exists(filePath))
            File.Delete(filePath);

        FileStream fileStream = File.Create(filePath);

        Serializer.Serialize(fileStream, modelToSave);
        fileStream.Close();
    }

    /// <summary>
    /// Loads the filename into an instance of class T.
    /// Returns null if no file can be found or if deserialization failed.
    /// </summary>
    /// <typeparam name="T">The class type to load. Files are saved in subdirectories grouped by type.</typeparam>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>An instance of the class that has been loaded.</returns>
    public static T LoadModel<T>(string fileName) where T : class
    {
        ValidateAttribute<T>("load");

        string filePath = GetDataFilePathForType<T>(fileName);

        if (!File.Exists(filePath))
            return null;

        FileStream fileStream = File.OpenRead(filePath);
        T output = Serializer.Deserialize<T>(fileStream);
        fileStream.Close();

        return output;
    }

    /// <summary>
    /// Returns a dictionary of filepath to loaded instance of the class. Used
    /// for loading all files of a type at once.
    /// </summary>
    /// <typeparam name="T">The class type to load. Files are saved in subdirectories grouped by type.</typeparam>
    /// <returns>A dictionary of filepath to loaded instance of the class.</returns>
    public static Dictionary<string, T> LoadModelsByType<T>()
    {
        ValidateAttribute<T>("load all");

        string directoryPath = GetDataDirectoryPathForType<T>();

        Dictionary<string, T> output = new Dictionary<string, T>();
        if (!Directory.Exists(directoryPath))
            return output;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath, c_searchPattern);
        foreach (string filePath in filePaths)
        {
            FileStream fileStream = File.OpenRead(filePath);
            output.Add(filePath, Serializer.Deserialize<T>(fileStream));
            fileStream.Close();
        }

        return output;
    }
    
    /// <summary>
    /// Gets the path to the subdirectory for the given type of class.
    /// </summary>
    /// <typeparam name="T">The type of class to find a subdirectory for.</typeparam>
    /// <returns>A path to the subdirectory for the given type of class.</returns>
    public static string GetDataDirectoryPathForType<T>()
    {
        return c_directoryFormat.StringBuilderFormat(Application.dataPath, typeof(T).Name);
    }

    /// <summary>
    /// Gets the full file path to the given file.
    /// </summary>
    /// <typeparam name="T">The type of class to find a path for.</typeparam>
    /// <param name="fileName">The filename to find a path for.</param>
    /// <returns>The full file path to the given file.</returns>
    public static string GetDataFilePathForType<T>(string fileName)
    {
        return c_fileFormat.StringBuilderFormat(GetDataDirectoryPathForType<T>(), fileName);
    }

    private static void ValidateAttribute<T>(string operation)
    {
        if (!typeof(T).IsDefined(typeof(ProtoContractAttribute), true))
            throw new InvalidOperationException("[Toolset.SaveManager] Attempting to {0} data model type {1} which does not have the ProtoContract Attribute."
                                                    .StringBuilderFormat(operation, typeof(T).Name));
    }
}
