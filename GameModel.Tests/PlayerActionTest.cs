using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameModel;
using GameModel.Actions;

namespace GameEngine.Tests;

[TestClass]
public class PlayerActionTests
{
    [TestMethod]
    public void Parse_LookWithoutTarget_ShouldReturnVerbOnly()
    {
        var action = PlayerAction.Parse("look");

        Assert.AreEqual("look", action.VerbText);
        Assert.AreEqual(0, action.Targets.Count);
    }

    [TestMethod]
    public void Parse_ExamineStatue_ShouldReturnOneTarget()
    {
        var action = PlayerAction.Parse("examine statue");

        Assert.AreEqual("examine", action.VerbText);
        Assert.AreEqual(1, action.Targets.Count);
        Assert.AreEqual("statue", action.Targets[0]);
    }

    [TestMethod]
    public void Parse_MoveToRoom_ShouldReturnTarget()
    {
        var action = PlayerAction.Parse("go hallway");

        Assert.AreEqual("go", action.VerbText);
        Assert.AreEqual(1, action.Targets.Count);
        Assert.AreEqual("hallway", action.Targets[0]);
    }

    [TestMethod]
    public void Parse_UseKeyOnDoor_ShouldReturnTwoTargets()
    {
        var action = PlayerAction.Parse("use key on door");

        Assert.AreEqual("use", action.VerbText);
        Assert.AreEqual(2, action.Targets.Count);
        Assert.AreEqual("key", action.Targets[0]);
        Assert.AreEqual("door", action.Targets[1]);
    }

    [TestMethod]
    public void Parse_CombineOilWithRag_ShouldReturnTwoTargets()
    {
        var action = PlayerAction.Parse("combine oil with rag");

        Assert.AreEqual("combine", action.VerbText);
        Assert.AreEqual(2, action.Targets.Count);
        Assert.AreEqual("oil", action.Targets[0]);
        Assert.AreEqual("rag", action.Targets[1]);
    }

    [TestMethod]
    public void Parse_ExtraSpaces_ShouldStillParseCorrectly()
    {
        var action = PlayerAction.Parse("  use   lantern   on    hook ");

        Assert.AreEqual("use", action.VerbText);
        Assert.AreEqual(2, action.Targets.Count);
        Assert.AreEqual("lantern", action.Targets[0]);
        Assert.AreEqual("hook", action.Targets[1]);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidDataException))]
    public void Parse_EmptyInput_ShouldThrow()
    {
        PlayerAction.Parse("");
    }
}