using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using ProtoBuf;
using ProtoBuf.Reflection;
using Toolset.Core;

namespace Toolset.ProtocolBuffers
{
    /// <summary>
    /// Static class that provides utility methods for saving/loading protobuf binaries to local storage.
    /// </summary>
    public static class SaveManager
    {
        // Note: Toolset saves all protobuf serialized files as .tso which stands for Toolset Serialized Object
        private const string c_tsoFileFormat = "{0}/{1}.tso";
        private const string c_tsoSearchPattern = "*.tso";
        private const string c_protoFileFormat = "{0}/{1}.proto";
        private const string c_protoSearchPattern = "*.proto";

        private const int c_asyncBufferSize = 4096;

        private static readonly List<char> s_invalidPathCharacters = new List<char>();

        /// <summary>
        /// Saves the instance of a class to a specific filename.
        /// Class must be Protobuf Serializable.
        /// </summary>
        /// <typeparam name="T">The class type to save. Files are saved in subdirectories grouped by type.</typeparam>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="modelToSave">The instance of the class to save.</param>
        public static void SaveModelInstance<T>(string fileName, T modelToSave) where T : class
        {
            ValidateFileName(nameof(SaveModelInstance), fileName);
            ValidateAttribute<T>(nameof(SaveModelInstance));

            InternalSaveModel(fileName, modelToSave);
        }

        /// <summary>
        /// Saves the instance of a class asynchronously to a specific filename.
        /// Class must be Protobuf Serializable.
        /// </summary>
        /// <typeparam name="T">The class type to save. Files are saved in subdirectories grouped by type.</typeparam>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="modelToSave">The instance of the class to save.</param>
        /// <returns>A task that can be awaited or converted to an IEnumerator.</returns>
        public static async Task SaveModelInstanceAsync<T>(string fileName, T modelToSave) where T : class
        {
            ValidateFileName(nameof(SaveModelInstanceAsync), fileName);
            ValidateAttribute<T>(nameof(SaveModelInstanceAsync));

            await InternalSaveModelAsync(fileName, modelToSave);
        }

        /// <summary>
        /// Saves the dictionary of file names/protobuf serializable objects.
        /// </summary>
        /// <typeparam name="T">The protobuf serializable class<./typeparam>
        /// <param name="dataToSave">The dictionary mapping desired file names to objects to save.</param>
        public static void SaveModelInstancesByType<T>(Dictionary<string, T> dataToSave) where T : class
        {
            ValidateAttribute<T>(nameof(SaveModelInstanceAsync));

            foreach (KeyValuePair<string, T> pair in dataToSave)
            {
                ValidateFileName(nameof(SaveModelInstanceAsync), pair.Key);
                InternalSaveModel(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Saves the dictionary of file names/protobuf serializable objects asynchronously.
        /// </summary>
        /// <typeparam name="T">The protobuf serializable class.</typeparam>
        /// <param name="dataToSave">The dictionary mapping desired file names to objects to save.</param>
        public static async Task SaveModelInstancesByTypeAsync<T>(Dictionary<string, T> dataToSave) where T : class
        {
            ValidateAttribute<T>(nameof(SaveModelInstanceAsync));

            foreach (KeyValuePair<string, T> pair in dataToSave)
            {
                ValidateFileName(nameof(SaveModelInstanceAsync), pair.Key);
                await InternalSaveModelAsync(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Loads the filename into an instance of class T.
        /// Returns null if no file can be found or if deserialization failed.
        /// </summary>
        /// <typeparam name="T">The class type to load. Files are saved in subdirectories grouped by type.</typeparam>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>An instance of the class that has been loaded.</returns>
        public static T LoadModelInstance<T>(string fileName) where T : class
        {
            ValidateFileName(nameof(LoadModelInstance), fileName);
            ValidateAttribute<T>(nameof(LoadModelInstance));

            string filePath = GetDataFilePathForModelType<T>(fileName);

            if (!File.Exists(filePath))
                return null;

            return InternalLoadModel<T>(filePath);
        }

        /// <summary>
        /// Loads the filename into an instance of class T asynchronously.
        /// Returns null if no file can be found or if deserialization failed.
        /// </summary>
        /// <typeparam name="T">The class type to load. Files are saved in subdirectories grouped by type.</typeparam>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A task that can be awaited with a reult that contains an instance of the class that has been loaded.</returns>
        public static async Task<T> LoadModelInstanceAsync<T>(string fileName) where T : class
        {
            ValidateFileName(nameof(LoadModelInstanceAsync), fileName);
            ValidateAttribute<T>(nameof(LoadModelInstanceAsync));

            string filePath = GetDataFilePathForModelType<T>(fileName);

            if (!File.Exists(filePath))
                return null;

            Task<T> internalWaitTask = InternalLoadModelAsync<T>(filePath);
            await internalWaitTask;
            return internalWaitTask.Result;
        }

        /// <summary>
        /// Returns a dictionary of filepath to loaded instance of the class. Used
        /// for loading all files of a type at once.
        /// </summary>
        /// <typeparam name="T">The class type to load. Files are saved in subdirectories grouped by type.</typeparam>
        /// <returns>A dictionary of filepath to loaded instance of the class.</returns>
        public static Dictionary<string, T> LoadModelInstanceByType<T>() where T : class
        {
            ValidateAttribute<T>(nameof(LoadModelInstanceByType));

            string directoryPath = GetDataDirectoryPathForModelType<T>();

            Dictionary<string, T> output = new Dictionary<string, T>();
            if (!Directory.Exists(directoryPath))
                return output;

            IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath, c_tsoSearchPattern);
            foreach (string filePath in filePaths)
            {
                output.Add(filePath, InternalLoadModel<T>(filePath));
            }

            return output;
        }

        /// <summary>
        /// Returns a Task that contains a dictionary of filepath to loaded instance of the class as its result. 
        /// Used for loading all files of a type in one async operation.
        /// </summary>
        /// <typeparam name="T">The class type to load. Files are saved in subdirectories grouped by type.</typeparam>
        /// <returns>A Task with a resulting dictionary of filepath to loaded instance of the class.</returns>
        public static async Task<Dictionary<string, T>> LoadModelInstancesByTypeAsync<T>() where T : class
        {
            ValidateAttribute<T>(nameof(LoadModelInstancesByTypeAsync));

            string directoryPath = GetDataDirectoryPathForModelType<T>();

            Dictionary<string, T> output = new Dictionary<string, T>();
            if (!Directory.Exists(directoryPath))
                return output;

            IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath, c_tsoSearchPattern);
            foreach (string filePath in filePaths)
            {
                Task<T> internalWaitTask = InternalLoadModelAsync<T>(filePath);
                await internalWaitTask;
                output.Add(filePath, internalWaitTask.Result);
            }

            return output;
        }

        /// <summary>
        /// Deletes the model saved at the given location.
        /// </summary>
        /// <typeparam name="T">The type of class that has an instance serialized save file being deleted.</typeparam>
        /// <param name="fileName">The name of the file to delete.</param>
        public static bool DeleteModelInstance<T>(string fileName) where T : class
        {
            ValidateAttribute<T>(nameof(DeleteModelInstance));
            ValidateFileName(nameof(DeleteModelInstance), fileName);

            string filePath = GetDataFilePathForModelType<T>(fileName);

            return DeleteFileAndMeta(filePath);
        }

        /// <summary>
        /// Deletes all models of the given class type.
        /// </summary>
        /// <typeparam name="T">The type of class having all saved models deleted.</typeparam>
        public static bool DeleteModelInstancesByType<T>() where T : class
        {
            ValidateAttribute<T>(nameof(DeleteModelInstancesByType));

            string directoryPath = GetDataDirectoryPathForModelType<T>();

            return DeleteDirectoryAndMetaRecursively(directoryPath);
        }

        /// <summary>
        /// Gets the path to the subdirectory for the given type of class.
        /// </summary>
        /// <typeparam name="T">The type of class to find a subdirectory for.</typeparam>
        /// <returns>A path to the subdirectory for the given type of class.</returns>
        public static string GetDataDirectoryPathForModelType<T>() where T : class
        {
            return Path.Combine(Application.dataPath, typeof(T).Name);
        }

        /// <summary>
        /// Gets the full file path to the given file.
        /// </summary>
        /// <typeparam name="T">The type of class to find a path for.</typeparam>
        /// <param name="fileName">The filename to find a path for.</param>
        /// <returns>The full file path to the given file.</returns>
        public static string GetDataFilePathForModelType<T>(string fileName) where T : class
        {
            return c_tsoFileFormat.StringBuilderFormat(GetDataDirectoryPathForModelType<T>(), fileName);
        }

        /// <summary>
        /// Deletes the file specified by the path and any associated metafile.
        /// </summary>
        /// <param name="filePath">Path to the file to delete.</param>
        public static bool DeleteFileAndMeta(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                filePath += ".meta";
                if (File.Exists(filePath))
                    File.Delete(filePath);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Recursively deletes the directory specified by the path and any associated metafile.
        /// </summary>
        /// <param name="directoryPath">Path to the directory to delete.</param>
        public static bool DeleteDirectoryAndMetaRecursively(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
                directoryPath += ".meta";
                if (File.Exists(directoryPath))
                    File.Delete(directoryPath);

                return true;
            }
            return false;
        }

        public static List<string> LoadAllFileContentsFromDirectory(string directoryPath, string searchPattern)
        {
            List<string> fileContents = new List<string>();
            if (!Directory.Exists(directoryPath))
                return fileContents;

            IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath, searchPattern);
            foreach (string filePath in filePaths)
            {
                fileContents.Add(File.ReadAllText(filePath));
            }

            return fileContents;
        }

        public static void SerializeObjectAsJsonAndSave(string fileName, string directoryPath, object objectToSerialized)
        {
            Directory.CreateDirectory(directoryPath);
            string fullFileName = Path.Combine(directoryPath, fileName);

            string jsonContent = JsonConvert.SerializeObject(objectToSerialized, Formatting.Indented);
            
            File.WriteAllText(fullFileName, jsonContent);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Compiles the specified .proto file within a directory into a C# script. Can only be used in Unity Editor. 
        /// </summary>
        /// <param name="protoDirectoryPath">Path to the directory that contains the .proto file to generate.</param>
        /// <param name="generatedDirectoryPath">Path to the directory that will contain the generated .cs file.</param>
        /// <param name="protoFileName">The proto file to generate.</param>
        /// <param name="refreshAndRecompile">Whether or not the Unity Editor should immediately force an AssetDatabase refresh and recompilation.</param>
        public static void GenerateSingleCSharpFromProto(string protoDirectoryPath, string generatedDirectoryPath, string protoFileName, bool refreshAndRecompile = true)
        {
            Directory.CreateDirectory(generatedDirectoryPath);

            string protoContents = File.ReadAllText(c_protoFileFormat.StringBuilderFormat(protoDirectoryPath, protoFileName));
            CompilerResult compilerResult = CSharpCodeGenerator.Default.Compile(new CodeFile(protoFileName, protoContents));
            File.WriteAllText(Path.Combine(generatedDirectoryPath, compilerResult.Files[0].Name), compilerResult.Files[0].Text);

            RefreshAndRecompileIfAllowed(refreshAndRecompile);
        }

        /// <summary>
        /// Compiles all .proto files in the specified directory into C# scripts. Can only be used in Unity Editor
        /// </summary>
        /// <param name="protoDirectoryPath">Path to the directory that contains the .proto files to generate.</param>
        /// <param name="generatedDirectoryPath">Path to the directory that will contain the generated .cs files.</param>
        /// <param name="refreshAndRecompile">Whether or not the Unity Editor should immediately force an AssetDatabase refresh and recompilation.</param>
        public static CompilerResult GenerateCSharpFromProto(string protoDirectoryPath, string generatedDirectoryPath, bool refreshAndRecompile = true)
        {
            Directory.CreateDirectory(generatedDirectoryPath);

            string[] files = Directory.GetFiles(protoDirectoryPath, c_protoSearchPattern);
            
            List<CodeFile> codeFiles = new List<CodeFile>();
            for (int i = 0; i < files.Length; ++i)
            {
                string protoContents = File.ReadAllText(c_protoFileFormat.StringBuilderFormat(protoDirectoryPath, Path.GetFileNameWithoutExtension(files[i])));
                codeFiles.Add(new CodeFile(Path.GetFileName(files[i]), protoContents));
            }
            
            CompilerResult compilerResult = CSharpCodeGenerator.Default.Compile(codeFiles.ToArray());
            for (int i = 0; i < compilerResult.Files.Length; ++i)
            {
                File.WriteAllText(Path.Combine(generatedDirectoryPath, compilerResult.Files[0].Name), compilerResult.Files[0].Text);
            }

            RefreshAndRecompileIfAllowed(refreshAndRecompile);
            return compilerResult;
        }

        private static void RefreshAndRecompileIfAllowed(bool isAllowed)
        {
            if (isAllowed)
            {
                UnityEditor.AssetDatabase.Refresh();
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            }
        }
#endif

        private static void ValidateAttribute<T>(string operation)
        {
            if (!ProtoBufUtils.IsSerializableProtoBuf(typeof(T)))
                throw new InvalidOperationException("[Toolset.SaveManager] Attempting {0} operation for model type {1} which is not ProtoBuf serializable."
                                                        .StringBuilderFormat(operation, typeof(T).Name));
        }

        private static void ValidateFileName(string operation, string fileName)
        {
            if (fileName.IsNullOrWhiteSpace() || fileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new InvalidOperationException("[Toolset.SaveManager] File Name {0} passed in {1} operation is not a valid file name."
                                            .StringBuilderFormat(fileName, operation));
        }

        private static void InternalSaveModel<T>(string fileName, T modelToSave) where T : class
        {
            string filePath = GetDataFilePathForModelType<T>(fileName);
            Directory.CreateDirectory(Path.Combine(GetDataDirectoryPathForModelType<T>(), Path.GetDirectoryName(fileName)));

            using (FileStream fileStream = File.Create(filePath))
            {
                Serializer.Serialize(fileStream, modelToSave);
            }
        }

        private static async Task InternalSaveModelAsync<T>(string fileName, T modelToSave) where T : class
        {
            string filePath = GetDataFilePathForModelType<T>(fileName);
            Directory.CreateDirectory(Path.Combine(GetDataDirectoryPathForModelType<T>(), Path.GetDirectoryName(fileName)));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, modelToSave);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, c_asyncBufferSize, FileOptions.Asynchronous))
                {
                    await fileStream.WriteAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                }
            }
        }

        private static T InternalLoadModel<T>(string filePath) where T : class
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                T output = Serializer.Deserialize<T>(fileStream);
                return output;
            }
        }

        private static async Task<T> InternalLoadModelAsync<T>(string filepath) where T : class
        {
            using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read, c_asyncBufferSize, FileOptions.Asynchronous))
            {
                byte[] dataThatWasRead = new byte[fileStream.Length];
                await fileStream.ReadAsync(dataThatWasRead, 0, dataThatWasRead.Length);

                using (MemoryStream memoryStream = new MemoryStream(dataThatWasRead))
                {
                    T output = Serializer.Deserialize<T>(memoryStream);
                    return output;
                }
            }
        }
    }
}