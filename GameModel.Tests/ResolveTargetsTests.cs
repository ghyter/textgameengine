using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameModel;
using GameModel.Actions;
using GameModel.Model;
using System.Collections.Generic;

namespace GameEngine.Tests;

[TestClass]
public class ResolveTargetsTests
{
    private GameSession _session = null!;
    private ActionRegistry _registry = null!;

    [TestInitialize]
    public void Setup()
    {
        _session = new GameSession();
        _registry = _session.ActionRegistry;

        // Setup scenes and elements as before...
        _session.Elements["scene:hall"] = new GameElementInfo { Id = "scene:hall", Element = new Scene { Id = "hall", Name = "Great Hall" }, Location = null };
        _session.Elements["item:silverkey"] = new GameElementInfo { Id = "item:silverkey", Element = new Item { Id = "item:silverkey", Name = "Silver Key" }, Location = "scene:hall" };
        _session.Elements["npc:guard"] = new GameElementInfo { Id = "npc:guard", Element = new Npc { Id = "npc:guard", Name = "Guard" }, Location = "scene:hall" };
        _session.Elements["item:goldcoin"] = new GameElementInfo { Id = "item:goldcoin", Element = new Item { Id = "item:goldcoin", Name = "Gold Coin" }, Location = "_inventory" };

        // Set up player in "scene:hall"
        _session.Elements["player:player"] = new GameElementInfo
        {
            Id = "player:player",
            Element = new Player { Id = "player:player", Name = "Hero" },
            Location = "scene:hall"
        };

        // Now populate ordinals for this scene/inventory state
        _session.PopulateOrdinals();
        Console.WriteLine(_session.PrintSceneOrdinals());

    }



    [TestMethod]
    public void ResolveTargets_LeavesDirectIdUnchanged()
    {
        var action = new PlayerAction { VerbText = "look", Targets = new List<string> { "item:silverkey" } };
        action = _registry.ResolveTargets(_session, action);
        Assert.AreEqual("item:silverkey", action.Targets[0]);
    }

    [TestMethod]
    public void ResolveTargets_ResolvesSceneOrdinal()
    {
    
        var action = new PlayerAction { VerbText = "look", Targets = new List<string> { "2" } };
        action = _registry.ResolveTargets(_session, action);
        Assert.AreEqual("item:silverkey", action.Targets[0]);
    }

    [TestMethod]
    public void ResolveTargets_ResolvesInventoryOrdinal()
    {
        var action = new PlayerAction { VerbText = "get", Targets = new List<string> { "I1" } };
        action = _registry.ResolveTargets(_session, action);
        Assert.AreEqual("Iitem:goldcoin", action.Targets[0]);
    }

    [TestMethod]
    public void ResolveTargets_ResolvesColonPrefixedId()
    {
        var action = new PlayerAction { VerbText = "look", Targets = new List<string> { "item:silverkey" } };
        action = _registry.ResolveTargets(_session, action);
        Assert.AreEqual("item:silverkey", action.Targets[0]);
    }

    [TestMethod]
    public void ResolveTargets_LeavesInvalidOrdinalUnchanged()
    {
        var action = new PlayerAction { VerbText = "look", Targets = new List<string> { "99" } }; // Invalid ordinal
        action = _registry.ResolveTargets(_session, action);
        Assert.AreEqual("99", action.Targets[0]);
    }

    [TestMethod]
    public void ResolveTargets_MultipleTargets_SomeOrdinals_SomeIds()
    {
        var action = new PlayerAction { VerbText = "use", Targets = new List<string> { "2", "npc:guard" } };
        action = _registry.ResolveTargets(_session, action);
        Assert.AreEqual("item:silverkey", action.Targets[0]);
        Assert.AreEqual("npc:guard", action.Targets[1]);
    }

[TestMethod]
public void ResolveTargets_InventoryOrdinal_ResolvesToInventoryId()
{
    var action = new PlayerAction { VerbText = "get", Targets = new List<string> { "I1" } };
    action = _registry.ResolveTargets(_session, action);
    Assert.AreEqual("Iitem:goldcoin", action.Targets[0]);
}

[TestMethod]
public void ResolveTargets_LeavesDirectInventoryIdUnchanged()
{
    var action = new PlayerAction { VerbText = "get", Targets = new List<string> { "Iitem:goldcoin" } };
    action = _registry.ResolveTargets(_session, action);
    Assert.AreEqual("Iitem:goldcoin", action.Targets[0]);
}

[TestMethod]
public void ResolveTargets_LeavesInvalidInventoryOrdinalUnchanged()
{
    var action = new PlayerAction { VerbText = "get", Targets = new List<string> { "I9" } }; // Invalid (no 9th item)
    action = _registry.ResolveTargets(_session, action);
    Assert.AreEqual("I9", action.Targets[0]);
}

[TestMethod]
public void ResolveTargets_MultipleTargets_SceneAndInventoryOrdinals()
{
    // Let's use "2" for scene ordinal, "I1" for inventory
    var action = new PlayerAction { VerbText = "use", Targets = new List<string> { "2", "I1" } };
    action = _registry.ResolveTargets(_session, action);

    // Depending on scene contents/order, "2" resolves as before
    Assert.IsTrue(_session.SceneOrdinals.Contains(action.Targets[0]));
    Assert.AreEqual("Iitem:goldcoin", action.Targets[1]);
    Assert.AreNotEqual("2", action.Targets[0]);
}



}
