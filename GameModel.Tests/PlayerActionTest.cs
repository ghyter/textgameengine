using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameModel;
using GameModel.Actions;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.Tests;

[TestClass]
public class PlayerActionTests
{
    //private ActionRegistry _registry = null!;

    [TestInitialize]
    public void Setup()
    {
        _registry = new ActionRegistry();

        void Register(string canonicalVerb, string[]? aliases = null)
        {
            _registry.Register(new GameAction
            {
                Id = canonicalVerb,
                CanonicalVerb = canonicalVerb,
                VerbAliases = aliases?.ToList() ?? new List<string>(),
                Handler = (session, action, playerAction) => $"Executed {canonicalVerb}"
            });
        }

        Register("look");
        Register("examine");
        Register("go", new[] { "move", "walk" });
        Register("use");
        Register("combine");
        Register("turn on", new[] { "activate", "switch on" });
    }

    [TestMethod]
    public void Parse_LookWithoutTarget_ShouldReturnVerbOnly()
    {
        var parsed = _registry.Parse("look");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("look", parsed!.VerbText);
        Assert.AreEqual(0, parsed.Targets.Count);
    }

    [TestMethod]
    public void Parse_ExamineStatue_ShouldReturnOneTarget()
    {
        var parsed = _registry.Parse("examine statue");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("examine", parsed!.VerbText);
        Assert.AreEqual(1, parsed.Targets.Count);
        Assert.AreEqual("statue", parsed.Targets[0]);
    }

    [TestMethod]
    public void Parse_MoveToRoom_ShouldReturnTarget()
    {
        var parsed = _registry.Parse("go hallway");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("go", parsed!.VerbText);
        Assert.AreEqual(1, parsed.Targets.Count);
        Assert.AreEqual("hallway", parsed.Targets[0]);
    }

    [TestMethod]
    public void Parse_UseKeyOnDoor_ShouldReturnTwoTargets()
    {
        var parsed = _registry.Parse("use key on door");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("use", parsed!.VerbText);
        Assert.AreEqual(2, parsed.Targets.Count);
        Assert.AreEqual("key", parsed.Targets[0]);
        Assert.AreEqual("door", parsed.Targets[1]);
    }

    [TestMethod]
    public void Parse_CombineOilWithRag_ShouldReturnTwoTargets()
    {
        var parsed = _registry.Parse("combine oil with rag");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("combine", parsed!.VerbText);
        Assert.AreEqual(2, parsed.Targets.Count);
        Assert.AreEqual("oil", parsed.Targets[0]);
        Assert.AreEqual("rag", parsed.Targets[1]);
    }

    [TestMethod]
    public void Parse_MultipleWordVerb_OneTarget()
    {
        var parsed = _registry.Parse("turn on flashlight");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("turn on", parsed!.VerbText);
        Assert.AreEqual(1, parsed.Targets.Count);
        Assert.AreEqual("flashlight", parsed.Targets[0]);
    }

    [TestMethod]
    public void Parse_ExtraSpaces_ShouldStillParseCorrectly()
    {
        var parsed = _registry.Parse("  use   lantern   on    hook ");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("use", parsed!.VerbText);
        Assert.AreEqual(2, parsed.Targets.Count);
        Assert.AreEqual("lantern", parsed.Targets[0]);
        Assert.AreEqual("hook", parsed.Targets[1]);
    }

    [TestMethod]
    public void Parse_UnrecognizedVerb_ShouldReturnNull()
    {
        var parsed = _registry.Parse("destroy everything");

        Assert.IsNull(parsed);
    }

    [TestMethod]
    public void Parse_VerifyTargetsAreSplitCorrectly()
    {
        // "use sword on dragon" -> verb: use, targets: sword, dragon
        var parsed = _registry.Parse("use sword on dragon");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("use", parsed!.VerbText);
        Assert.AreEqual(2, parsed.Targets.Count);
        Assert.AreEqual("sword", parsed.Targets[0]);
        Assert.AreEqual("dragon", parsed.Targets[1]);
    }

    [TestMethod]
    public void Parse_ShortSynonym_ShouldResolveToCanonical()
    {
        // "walk" is an alias for "go"
        var parsed = _registry.Parse("walk north");

        Assert.IsNotNull(parsed);
        Assert.AreEqual("go", parsed!.VerbText);
        Assert.AreEqual(1, parsed.Targets.Count);
        Assert.AreEqual("north", parsed.Targets[0]);
    }


public void Parse_Aliases_OnlyNeedOneRegistration()
{
    // Register look with two aliases
    var invoked = new List<string>();
    _registry.Register(new GameAction
    {
        Id = "look",
        CanonicalVerb = "look",
        VerbAliases = new() { "examine", "view" },
        Handler = (session, action, playerAction) =>
        {
            invoked.Add(playerAction.VerbText); // Record which verb is seen as canonical
            return "OK";
        }
    });

    var verbsToTest = new[] { "look", "examine", "view" };

    foreach (var verb in verbsToTest)
    {
        var parsed = _registry.Parse($"{verb} statue");
        Assert.IsNotNull(parsed);
        Assert.AreEqual("look", parsed!.VerbText); // Always canonical
        Assert.AreEqual(1, parsed.Targets.Count);
        Assert.AreEqual("statue", parsed.Targets[0]);
    }
}
}
