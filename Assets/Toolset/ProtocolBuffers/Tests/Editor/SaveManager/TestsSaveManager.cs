using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Class of unit tests used to validate the SaveManager class.
/// </summary>
public class TestsSaveManager
{
    private Random m_random;
    private readonly List<string> m_batchModelNames = new List<string>() { "example1", "example2", "example3" , "example4" , "example5" };
    private readonly string m_pathToProtoSourceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/ProtoFiles";
    private readonly string m_pathToProtoGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/Generated";

    private const string c_exampleProtoFileName = "addressbook";
    private const string c_expectedGeneratedCSharpFileName = "addressbook.cs";
    private const string c_secondExpectedGeneratedCSharpFileName = "exampleproto.cs";

    [SetUp]
    public void SetUp()
    {
        m_random = new Random();
    }

    [Test]
    public void TestLoadWithoutSaveFile()
    {
        ExampleProtobufModel nullModel = SaveManager.LoadModel<ExampleProtobufModel>("somefakefilepath");

        Assert.IsNull(nullModel);
    }

    [Test]
    public void TestSaveFaultyProtobufModel()
    {
        ToolsetAssert.Throws<InvalidOperationException>(() =>
        {
            SaveManager.SaveModel("Faulty", new ExampleFaultyProtobufModel());
        });
    }

    [Test]
    public void TestLoadFaultyProtobufModel()
    {
        ToolsetAssert.Throws<InvalidOperationException>(() =>
        {
            SaveManager.LoadModel<ExampleFaultyProtobufModel>("Faulty");
        });
    }

    [Test]
    public void TestLoadByTypeFaultyProtobufModel()
    {
        ToolsetAssert.Throws<InvalidOperationException>(() =>
        {
            SaveManager.LoadModelsByType<ExampleFaultyProtobufModel>();
        });
    }

    [Test]
    public void TestDeleteFaultyProtobufModel()
    {
        ToolsetAssert.Throws<InvalidOperationException>(() =>
        {
            SaveManager.DeleteModel<ExampleFaultyProtobufModel>("Faulty");
        });
    }

    [Test]
    public void TestDeleteByTypeFaultyProtobufModel()
    {
        ToolsetAssert.Throws<InvalidOperationException>(() =>
        {
            SaveManager.DeleteModelsByType<ExampleFaultyProtobufModel>();
        });
    }

    [Test]
    public void TestSaveInvalidFileName()
    {
        AssertExceptionsOnInvalidFileNames((callbackParameter) => 
        {
            SaveManager.SaveModel(callbackParameter, new ExampleProtobufModel());
        });
    }

    [Test]
    public void TestLoadInvalidFileName()
    {
        AssertExceptionsOnInvalidFileNames((callbackParameter) =>
        {
            SaveManager.LoadModel<ExampleProtobufModel>(callbackParameter);
        });
    }

    [Test]
    public void TestDeleteInvalidFileName()
    {
        AssertExceptionsOnInvalidFileNames((callbackParameter) =>
        {
            SaveManager.DeleteModel<ExampleProtobufModel>(callbackParameter);
        });
    }

    [Test]
    public void TestSaveAndLoad()
    {
        // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
        // this saved model has different values from the ones in TestOverwriteAndLoad. Although generating these models
        // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
        // fail edge case in these tests.
        ExampleProtobufModel modelToSave = new ExampleProtobufModel()
        {
            ExampleInt = 456,
            ExampleString = "Some random words.",
            ExampleInternalModel = new ExampleInternalProtobufModel()
            { 
                ExampleString1 = "string 1",
                ExampleString2 = "string 2",
                ExampleString3 = "string 3"
            },
            ExampleIntList = new List<int>() { 9, 8, 7, 6, 5, 4, 3, 2, 1 }
        };

        SaveManager.SaveModel(m_batchModelNames[0], modelToSave);

        Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[0])));

        ExampleProtobufModel modelToLoad = SaveManager.LoadModel<ExampleProtobufModel>(m_batchModelNames[0]);

        AssertModelsAreEqual(modelToSave, modelToLoad);
    }

    [Test]
    public void TestOverwriteAndLoad()
    {
        TestSaveAndLoad();

        ExampleProtobufModel preSaveLoadedModel = SaveManager.LoadModel<ExampleProtobufModel>(m_batchModelNames[0]);

        // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
        // this saved model has different values from the ones in TestSaveAndLoad. Although generating these models
        // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
        // fail edge case in these tests.
        ExampleProtobufModel modelToSave = new ExampleProtobufModel()
        {
            ExampleInt = 789,
            ExampleString = "More random words",
            ExampleInternalModel = new ExampleInternalProtobufModel()
            {
                ExampleString1 = "string A",
                ExampleString2 = "string B",
                ExampleString3 = "string C"
            },
            ExampleIntList = new List<int>() { 4, 4, 4, 4 }
        };

        AssertModelsAreNotEqual(modelToSave, preSaveLoadedModel);

        SaveManager.SaveModel(m_batchModelNames[0], modelToSave);

        Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[0])));

        ExampleProtobufModel modelToLoad = SaveManager.LoadModel<ExampleProtobufModel>(m_batchModelNames[0]);

        AssertModelsAreEqual(modelToSave, modelToLoad);
    }

    [Test]
    public void TestLoadAll()
    {
        Dictionary<string, ExampleProtobufModel> modelsToSave = new Dictionary<string, ExampleProtobufModel>();

        for (int i = 0; i < m_batchModelNames.Count; ++i)
        {
            modelsToSave.Add(m_batchModelNames[i], GenerateRandomValidProtobuf());
        }

        foreach (KeyValuePair<string, ExampleProtobufModel> pair in modelsToSave)
        {
            SaveManager.SaveModel(pair.Key, pair.Value);
        }

        foreach (KeyValuePair<string, ExampleProtobufModel> pair in modelsToSave)
        {
            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(pair.Key)));
        }

        Dictionary<string, ExampleProtobufModel> loadedModels = SaveManager.LoadModelsByType<ExampleProtobufModel>();

        foreach (KeyValuePair<string, ExampleProtobufModel> pair in loadedModels)
        {
            AssertModelsAreEqual(modelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
        }
    }

    [Test]
    public void TestDelete()
    {
        SaveManager.SaveModel(m_batchModelNames[0], GenerateRandomValidProtobuf());

        string filePath = SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[0]);
        Assert.IsTrue(File.Exists(filePath));

        SaveManager.DeleteModel<ExampleProtobufModel>(m_batchModelNames[0]);
        Assert.IsFalse(File.Exists(filePath));
    }

    [Test]
    public void TestDeleteAllByType()
    {
        for (int i = 0; i < m_batchModelNames.Count; ++i)
        {
            SaveManager.SaveModel(m_batchModelNames[i], GenerateRandomValidProtobuf());
            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[i])));
        }

        SaveManager.DeleteModelsByType<ExampleProtobufModel>();

        for (int i = 0; i < m_batchModelNames.Count; ++i)
        {
            Assert.IsFalse(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[i])));
        }
    }

    [Test]
    public void TestGenerateSingleCSharpFromProto()
    {
        SaveManager.GenerateSingleCSharpFromProto(m_pathToProtoSourceDirectory, m_pathToProtoGeneratedDirectory, c_exampleProtoFileName, false);
        Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_expectedGeneratedCSharpFileName)));
    }

    [Test]
    public void TestGenerateCSharpFromProto()
    {
        SaveManager.GenerateCSharpFromProto(m_pathToProtoSourceDirectory, m_pathToProtoGeneratedDirectory, false);
        Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_expectedGeneratedCSharpFileName)));
        Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_secondExpectedGeneratedCSharpFileName)));
    }

    [TearDown]
    public void TearDown()
    {
        for (int i = 0; i < m_batchModelNames.Count; ++i)
        {
            string filePath = SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[i]);
            DeleteFileAndMetaIfExists(filePath);
        }

        string directoryPath = SaveManager.GetDataDirectoryPathForType<ExampleProtobufModel>();
        DeleteDirectoryAndMetaIfExists(directoryPath);

        if (Directory.Exists(m_pathToProtoGeneratedDirectory))
        {
            DeleteFileAndMetaIfExists(Path.Combine(m_pathToProtoGeneratedDirectory, c_expectedGeneratedCSharpFileName));
            DeleteFileAndMetaIfExists(Path.Combine(m_pathToProtoGeneratedDirectory, c_secondExpectedGeneratedCSharpFileName));
            DeleteDirectoryAndMetaIfExists(m_pathToProtoGeneratedDirectory);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        UnityEditor.AssetDatabase.Refresh();
    }

    private void DeleteFileAndMetaIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            filePath += ".meta";
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    private void DeleteDirectoryAndMetaIfExists(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath);
            directoryPath += ".meta";
            if (File.Exists(directoryPath))
                File.Delete(directoryPath);
        }
    }

    private void AssertModelsAreEqual(ExampleProtobufModel expected, ExampleProtobufModel actual)
    {
        Assert.AreEqual(expected.ExampleInt, actual.ExampleInt);
        Assert.AreEqual(expected.ExampleString, actual.ExampleString);
        Assert.AreEqual(expected.ExampleInternalModel.ExampleString1, actual.ExampleInternalModel.ExampleString1);
        Assert.AreEqual(expected.ExampleInternalModel.ExampleString2, actual.ExampleInternalModel.ExampleString2);
        Assert.AreEqual(expected.ExampleInternalModel.ExampleString3, actual.ExampleInternalModel.ExampleString3);
    }

    private void AssertModelsAreNotEqual(ExampleProtobufModel expected, ExampleProtobufModel actual)
    {
        Assert.AreNotEqual(expected.ExampleInt, actual.ExampleInt);
        Assert.AreNotEqual(expected.ExampleString, actual.ExampleString);
        Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString1, actual.ExampleInternalModel.ExampleString1);
        Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString2, actual.ExampleInternalModel.ExampleString2);
        Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString3, actual.ExampleInternalModel.ExampleString3);
    }

    private ExampleProtobufModel GenerateRandomValidProtobuf()
    {
        ExampleProtobufModel output = new ExampleProtobufModel()
        {
            ExampleInt = m_random.Next(),
            ExampleString = m_random.Next().ToString(),
            ExampleInternalModel = new ExampleInternalProtobufModel()
            {
                ExampleString1 = m_random.Next().ToString(),
                ExampleString2 = m_random.Next().ToString(),
                ExampleString3 = m_random.Next().ToString()
            },
            ExampleIntList = new List<int>() { }
        };

        for (int i = 0; i < m_random.Next(1, 10); ++i)
        {
            output.ExampleIntList.Add(m_random.Next());
        }

        return output;
    }

    private void AssertExceptionsOnInvalidFileNames(Action<string> callbackToTest)
    {
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

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

        for (int i = 0; i < invalidFileNameChars.Length; ++i)
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                callbackToTest(invalidFileNameChars[i].ToString());
            });
        }
    }
}
