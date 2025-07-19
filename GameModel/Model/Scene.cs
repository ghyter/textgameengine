namespace GameModel.Model;

public class Scene : StatefullGameElements
{
    public string SceneId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<string> Exits { get; set; } = [];
    

}
