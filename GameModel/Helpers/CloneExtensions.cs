namespace GameModel.Helpers;

// ===== Utility deep clone helper =====
public static class CloneExtensions
{
    // Simple deep clone via System.Text.Json (good enough for typical POCOs)
    // Ensure your model is serializable and avoid cycles.
    public static T DeepClone<T>(this T source)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(source);
        return System.Text.Json.JsonSerializer.Deserialize<T>(json)!;
    }
}
