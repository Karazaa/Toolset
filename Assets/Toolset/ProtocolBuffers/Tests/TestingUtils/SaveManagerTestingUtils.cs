using UnityEditor;
using System;
using System.IO;
using System.Threading.Tasks;
using Toolset.Core.Tests;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Static class to assist with testing the SaveManager
    /// </summary>
    public static class SaveManagerTestingUtils
    {
        /// <summary>
        /// Generates the proto files in the Persistent Proto folder into C# classes.
        /// </summary>
        [MenuItem("Toolset/Testing/Generate Persistent Proto")]
        public static void GeneratePersistentProto()
        {
            string pathToPersistentProtoSourceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/PersistentTesting/ProtoFiles";
            string pathToPersistentProtoGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/PersistentTesting/Generated";
            SaveManager.GenerateCSharpFromProto(pathToPersistentProtoSourceDirectory, pathToPersistentProtoGeneratedDirectory);
        }

        /// <summary>
        /// Asserts whether exceptions occur in the delegate when passed specific invalid filenames.
        /// </summary>
        /// <param name="callbackToTest">The callback to test.</param>
        public static void AssertExceptionsOnInvalidFileNames(Action<string> callbackToTest)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();

            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                callbackToTest(null);
            });

            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                callbackToTest(string.Empty);
            });

            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                callbackToTest("    ");
            });

            for (int i = 0; i < invalidPathChars.Length; ++i)
            {
                ToolsetAssert.Throws<InvalidOperationException>(() =>
                {
                    callbackToTest(invalidPathChars[i].ToString());
                });
            }
        }

        /// <summary>
        /// Asserts whether exceptions occur in the async task when passed specific invalid filenames.
        /// </summary>
        /// <param name="asyncOperationToTest">The async operation to test.</param>
        /// <returns>A task that can be awaited on.</returns>
        public async static Task AssertExceptionsOnInvalidFileNamesAsync(Func<string, Task> asyncOperationToTest)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();

            await ToolsetAssert.ThrowsAsync<InvalidOperationException>(asyncOperationToTest(null));
            await ToolsetAssert.ThrowsAsync<InvalidOperationException>(asyncOperationToTest(string.Empty));
            await ToolsetAssert.ThrowsAsync<InvalidOperationException>(asyncOperationToTest("    "));
            for (int i = 0; i < invalidPathChars.Length; ++i)
            {
                await ToolsetAssert.ThrowsAsync<InvalidOperationException>(asyncOperationToTest(invalidPathChars[i].ToString()));
            }
        }
    }
}
