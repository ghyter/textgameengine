using System.Text.Json.Serialization;

namespace GameModel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActionDifficulty
{
    Trivial = 0, //must exceed 0 on a d20
    Easy = 5,  // must exceed 5 on a d20
    Normal = 10, // must exceed 10 on a d20
    Difficult = 15,  //must exceed 15 on a d20

    VeryDifficult = 20,  //must exceed 25 on a d20
    NearlyImpossible = 30 //must exceed 30 on a d20
    
}
