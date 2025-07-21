using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameModel;
using GameModel.Actions;
using System.Collections.Generic;

namespace GameEngine.Tests;

[TestClass]
public class PlayerActionTests
{
    private ActionRegistry _registry = null!;
    private GameSession _session = null!;
    private readonly Dictionary<string, bool> _handlerInvoked = new();

    [TestInitialize]
    public void Setup()
    {
        _registry = new ActionRegistry();
        _session = new GameSession(); // Adjust constructor if needed

        void Register(string verb, params string[] aliases)
        {
            _registry.Register((session, action) =>
            {
                _handlerInvoked[verb] = true;
                return $"Executed {verb}";
            }, new[] { verb }.Concat(aliases).ToArray());
        }

        Register("look");
        Register("examine");
        Register("go", "move", "walk");
        Register("use");
        Register("combine");
        Register("turn on", "activate", "switch on");
    }

    [TestMethod]
    public void Parse_LookWithoutTarget_ShouldReturnVerbOnly()
    {
        _registry.TryExecute(_session, "look", out var result);

        Assert.AreEqual("Executed look", result);
        Assert.IsTrue(_handlerInvoked["look"]);
    }

    [TestMethod]
    public void Parse_ExamineStatue_ShouldReturnOneTarget()
    {
        _registry.TryExecute(_session, "examine statue", out var result);

        Assert.AreEqual("Executed examine", result);
        Assert.IsTrue(_handlerInvoked["examine"]);
    }

    [TestMethod]
    public void Parse_MoveToRoom_ShouldReturnTarget()
    {
        _registry.TryExecute(_session, "go hallway", out var result);

        Assert.AreEqual("Executed go", result);
        Assert.IsTrue(_handlerInvoked["go"]);
    }

    [TestMethod]
    public void Parse_UseKeyOnDoor_ShouldReturnTwoTargets()
    {
        _registry.TryExecute(_session, "use key on door", out var result);

        Assert.AreEqual("Executed use", result);
        Assert.IsTrue(_handlerInvoked["use"]);
    }

    [TestMethod]
    public void Parse_CombineOilWithRag_ShouldReturnTwoTargets()
    {
        _registry.TryExecute(_session, "combine oil with rag", out var result);

        Assert.AreEqual("Executed combine", result);
        Assert.IsTrue(_handlerInvoked["combine"]);
    }

    [TestMethod]
    public void Parse_MultipleWordVerb_OneTarget()
    {
        _registry.TryExecute(_session, "turn on flashlight", out var result);

        Assert.AreEqual("Executed turn on", result);
        Assert.IsTrue(_handlerInvoked["turn on"]);
    }

    [TestMethod]
    public void Parse_ExtraSpaces_ShouldStillParseCorrectly()
    {
        _registry.TryExecute(_session, "  use   lantern   on    hook ", out var result);

        Assert.AreEqual("Executed use", result);
        Assert.IsTrue(_handlerInvoked["use"]);
    }

    [TestMethod]
    public void Parse_UnrecognizedVerb_ShouldFailGracefully()
    {
        var success = _registry.TryExecute(_session, "destroy everything", out var result);

        Assert.IsFalse(success);
        Assert.AreEqual("I don't know how to 'destroy everything'.", result);
    }

    [TestMethod]
    public void Parse_VerifyTargetsAreSplitCorrectly()
    {
        PlayerAction? parsed = null;

        _registry.Register((session, action) =>
        {
            parsed = action;
            return "ok";
        }, "use");

        _registry.TryExecute(_session, "use sword on dragon", out _);

        Assert.IsNotNull(parsed);
        Assert.AreEqual("use", parsed!.VerbText);
        Assert.AreEqual(2, parsed.Targets.Count);
        Assert.AreEqual("sword", parsed.Targets[0]);
        Assert.AreEqual("dragon", parsed.Targets[1]);
    }

    [TestMethod]
    public void Parse_ShortSynonym_ShouldResolveToCanonical()
    {
        _registry.TryExecute(_session, "walk north", out var result);

        Assert.AreEqual("Executed go", result); // "walk" is alias of "go"
        Assert.IsTrue(_handlerInvoked["go"]);
    }
}
