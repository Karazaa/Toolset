using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using ProtoBuf.Reflection;

/// <summary>
/// Static class that provides utility methods for saving/loading protobuf binaries to local storage.
/// </summary>
public static class SaveManager
{
    // Note: Toolset saves all protobuf serialized files as .tso which stands for Toolset Serialized Object
    private const string c_fileFormat = "{0}/{1}.tso";
    private const string c_searchPattern = "*.tso";
    private const string c_directoryFormat = "{0}/{1}";

    private static readonly List<char> s_invalidPathCharacters = new List<char>();

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
        ValidateAttribute<T>(nameof(SaveModel));
        ValidateFileName(nameof(SaveModel), fileName);

        string filePath = GetDataFilePathForType<T>(fileName);
        Directory.CreateDirectory(GetDataDirectoryPathForType<T>());

        if (File.Exists(filePath))
            File.Delete(filePath);

        using (FileStream fileStream = File.Create(filePath))
        {
            Serializer.Serialize(fileStream, modelToSave);
        }
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
        ValidateAttribute<T>(nameof(LoadModel));
        ValidateFileName(nameof(LoadModel), fileName);

        string filePath = GetDataFilePathForType<T>(fileName);

        if (!File.Exists(filePath))
            return null;

        using (FileStream fileStream = File.OpenRead(filePath))
        {
            T output = Serializer.Deserialize<T>(fileStream);
            return output;
        }        
    }

    /// <summary>
    /// Returns a dictionary of filepath to loaded instance of the class. Used
    /// for loading all files of a type at once.
    /// </summary>
    /// <typeparam name="T">The class type to load. Files are saved in subdirectories grouped by type.</typeparam>
    /// <returns>A dictionary of filepath to loaded instance of the class.</returns>
    public static Dictionary<string, T> LoadModelsByType<T>() where T : class
    {
        ValidateAttribute<T>(nameof(LoadModelsByType));

        string directoryPath = GetDataDirectoryPathForType<T>();

        Dictionary<string, T> output = new Dictionary<string, T>();
        if (!Directory.Exists(directoryPath))
            return output;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath, c_searchPattern);
        foreach (string filePath in filePaths)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                output.Add(filePath, Serializer.Deserialize<T>(fileStream));
            }
        }

        return output;
    }

    /// <summary>
    /// Deletes the model saved at the given location.
    /// </summary>
    /// <typeparam name="T">The type of class that has an instance serialized save file being deleted.</typeparam>
    /// <param name="fileName">The name of the file to delete.</param>
    public static void DeleteModel<T>(string fileName) where T : class
    {
        ValidateAttribute<T>(nameof(DeleteModel));
        ValidateFileName(nameof(DeleteModel), fileName);

        string filePath = GetDataFilePathForType<T>(fileName);

        if (!File.Exists(filePath))
            return;

        File.Delete(filePath);
    }

    /// <summary>
    /// Deletes all models of the given class type.
    /// </summary>
    /// <typeparam name="T">The type of class having all saved models deleted.</typeparam>
    public static void DeleteModelsByType<T>() where T : class
    {
        ValidateAttribute<T>(nameof(DeleteModelsByType));

        string directoryPath = GetDataDirectoryPathForType<T>();

        Directory.Delete(directoryPath, true);
    }

    /// <summary>
    /// Gets the path to the subdirectory for the given type of class.
    /// </summary>
    /// <typeparam name="T">The type of class to find a subdirectory for.</typeparam>
    /// <returns>A path to the subdirectory for the given type of class.</returns>
    public static string GetDataDirectoryPathForType<T>() where T : class
    {
        return Path.Combine(Application.dataPath, typeof(T).Name);
    }

    /// <summary>
    /// Gets the full file path to the given file.
    /// </summary>
    /// <typeparam name="T">The type of class to find a path for.</typeparam>
    /// <param name="fileName">The filename to find a path for.</param>
    /// <returns>The full file path to the given file.</returns>
    public static string GetDataFilePathForType<T>(string fileName) where T : class
    {
        return c_fileFormat.StringBuilderFormat(GetDataDirectoryPathForType<T>(), fileName);
    }

#if UNITY_EDITOR
    public static void GenerateSingleCSharpFromProto(string protoDirectoryPath, string generatedDirectoryPath, string protoFileName, bool refreshAndRecompile = false)
    {
        string protoContents = File.ReadAllText(Path.Combine(protoDirectoryPath, protoFileName));
        CompilerResult compilerResult = CSharpCodeGenerator.Default.Compile(new CodeFile(protoFileName, protoContents));

        Directory.CreateDirectory(generatedDirectoryPath);
        File.WriteAllText(Path.Combine(generatedDirectoryPath, compilerResult.Files[0].Name), compilerResult.Files[0].Text);

        if (refreshAndRecompile)
        {
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
#endif

    private static void ValidateAttribute<T>(string operation)
    {
        if (!typeof(T).IsDefined(typeof(ProtoContractAttribute), true))
            throw new InvalidOperationException("[Toolset.SaveManager] Attempting {0} operation for model type {1} which does not have the ProtoContract Attribute."
                                                    .StringBuilderFormat(operation, typeof(T).Name));
    }

    private static void ValidateFileName(string operation, string fileName)
    {
        if (fileName.IsNullOrWhiteSpace() || fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            throw new InvalidOperationException("[Toolset.SaveManager] File Name {0} passed in {1} operation is not a valid file name."
                                        .StringBuilderFormat(fileName, operation));
    }
}
