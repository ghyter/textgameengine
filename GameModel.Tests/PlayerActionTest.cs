using GameModel;
namespace GameModel.Tests;

[TestClass]
public sealed class PlayerActionTests
{
    [TestMethod]
    public void Parse_LookWithoutTarget_ShouldReturnLookVerb()
    {
        var action = PlayerAction.Parse("look");
        Assert.AreEqual(Verbs.look, action.Verb);
        Assert.AreEqual(0, action.Targets.Count);
    }

    [TestMethod]
    public void Parse_ExamineAlias_ShouldReturnLookVerb()
    {
        var action = PlayerAction.Parse("examine statue");
        Assert.AreEqual(Verbs.look, action.Verb);
        Assert.AreEqual(1, action.Targets.Count);
        Assert.AreEqual("statue", action.Targets[0]);
    }

    [TestMethod]
    public void Parse_MoveWithTarget_ShouldReturnMoveVerb()
    {
        var action = PlayerAction.Parse("go hallway");
        Assert.AreEqual(Verbs.move, action.Verb);
        Assert.AreEqual(1, action.Targets.Count);
        Assert.AreEqual("hallway", action.Targets[0]);
    }

    [TestMethod]
    public void Parse_UseWithClause_ShouldReturnTwoTargets()
    {
        var action = PlayerAction.Parse("use key on door");
        Assert.AreEqual(Verbs.use, action.Verb);
        Assert.AreEqual(2, action.Targets.Count);
        Assert.AreEqual("key", action.Targets[0]);
        Assert.AreEqual("door", action.Targets[1]);
    }

    [TestMethod]
    public void Parse_CombineWithClause_ShouldReturnTwoTargets()
    {
        var action = PlayerAction.Parse("combine oil with rag");
        Assert.AreEqual(Verbs.combine, action.Verb);
        Assert.AreEqual(2, action.Targets.Count);
        Assert.AreEqual("oil", action.Targets[0]);
        Assert.AreEqual("rag", action.Targets[1]);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidDataException))]
    public void Parse_UnknownVerb_ShouldThrow()
    {
        PlayerAction.Parse("launch rocket");
    }

    [TestMethod]
    public void Parse_ExtraSpaces_ShouldStillParse()
    {
        var action = PlayerAction.Parse("  use   lantern   on    hook ");
        Assert.AreEqual(Verbs.use, action.Verb);
        Assert.AreEqual(2, action.Targets.Count);
        Assert.AreEqual("lantern", action.Targets[0]);
        Assert.AreEqual("hook", action.Targets[1]);
    }
}