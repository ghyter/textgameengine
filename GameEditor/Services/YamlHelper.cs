using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using GameModel;

public static class YamlHelper
{
    public static GameData? ParseYaml(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<GameData>(yaml);
    }

    public static string SerializeYaml(GameData data)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return serializer.Serialize(data);
    }
}
