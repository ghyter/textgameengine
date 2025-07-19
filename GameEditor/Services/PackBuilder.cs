using System.IO.Compression;
using System.Text.Json;
using GameModel;

public static class PackBuilder
{
    public static byte[] SerializeAndCompress(GameData data)
    {
        var json = JsonSerializer.Serialize(data);
        var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

        using var ms = new MemoryStream();
        using (var brotli = new BrotliStream(ms, CompressionLevel.Optimal))
        {
            brotli.Write(jsonBytes, 0, jsonBytes.Length);
        }

        return ms.ToArray();
    }
}
