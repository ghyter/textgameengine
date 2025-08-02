using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameModel.Actions;
using GameModel.Models;
using GameModel.Session;

namespace GameEngine.Tests;

[TestClass]
public class GameSessionTests
{
    [TestMethod]
    public void GetGameElement_TypedAndGeneric_ReturnsObjects()
    {
        var session = GameSession.NewGame("/workspaces/textgameengine/packs/clue.json");

        var scene = session.GetGameElement<Scene>("scene:hall");
        Assert.IsNotNull(scene);
        Assert.AreEqual("scene:hall", scene!.Id);

        var generic = session.GetGameElement("item:candlestick");
        Assert.IsNotNull(generic);
        Assert.AreEqual("item:candlestick", generic!.Id);
    }
}
