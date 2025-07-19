using System.IO.Compression;
using System.Text.Json;
using GameModel;

public static class PackLoader
{
    public static GameData? LoadFromBytes(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        using var brotli = new BrotliStream(ms, CompressionMode.Decompress);
        using var reader = new StreamReader(brotli);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<GameData>(json);
    }
}
