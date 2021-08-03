using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Class of unit tests used to validate the SaveManager class.
/// </summary>
public class TestsSaveManager
{
    [Test]
    public void TestLoadWithoutSaveFile()
    {
        ExampleProtobufModel nullModel = SaveManager.LoadModelIfSaved<ExampleProtobufModel>();

        Assert.IsNull(nullModel);
    }

    [Test]
    public void TestSaveFaultyProtobufModel()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            SaveManager.SaveModel(new ExampleFaultyProtobufModel());
        });
    }

    [Test]
    public void TestLoadFaultyProtobufModel()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            SaveManager.LoadModelIfSaved<ExampleFaultyProtobufModel>();
        });
    }

    [Test]
    public void TestSaveAndLoad()
    {
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

        SaveManager.SaveModel(modelToSave);

        Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>()));

        ExampleProtobufModel modelToLoad = SaveManager.LoadModelIfSaved<ExampleProtobufModel>();

        Assert.AreEqual(modelToSave.ExampleInt, modelToLoad.ExampleInt);
        Assert.AreEqual(modelToSave.ExampleString, modelToLoad.ExampleString);
        Assert.AreEqual(modelToSave.ExampleInternalModel.ExampleString1, modelToLoad.ExampleInternalModel.ExampleString1);
        Assert.AreEqual(modelToSave.ExampleInternalModel.ExampleString2, modelToLoad.ExampleInternalModel.ExampleString2);
        Assert.AreEqual(modelToSave.ExampleInternalModel.ExampleString3, modelToLoad.ExampleInternalModel.ExampleString3);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>()))
            File.Delete(SaveManager.GetDataFilePathForType<ExampleProtobufModel>());
    }
}
