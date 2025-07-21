using System.Text.Json;

namespace GameModel.Pack;

public static class GamePackLoader
{
    public static GamePack Load(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GamePack>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}
