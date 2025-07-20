using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameModel;
using GameModel.Actions;
using GameModel.Model;

namespace GameEngine.Tests;

[TestClass]
public class GameSessionTests
{
    [TestMethod]
    public void GetGameElement_TypedAndGeneric_ReturnsObjects()
    {
        var session = GameSession.NewGame("/workspaces/textgameengine/packs/clue.json");

        var scene = session.GetGameElement<Scene>("hall");
        Assert.IsNotNull(scene);
        Assert.AreEqual("hall", scene!.Id);

        var generic = session.GetGameElement("candlestick");
        Assert.IsNotNull(generic);
        Assert.AreEqual("candlestick", generic!.Id);
    }
}
