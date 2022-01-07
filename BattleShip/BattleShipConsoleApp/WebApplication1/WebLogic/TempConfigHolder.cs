using System.Text.Json;
using BattleShipBrain;

namespace WebApplication1.WebLogic;

public class TempConfigHolder
{

    public string ConfigName { get; set; } = "NotDefined!";
    public string WhereToSave { get; set; } = "NotDefined";
    public int BoardSizeX { get; set; } = 10;
    public int BoardSizeY { get; set; } = 10;
    public int ShipsToCreate { get; set; } = 1;
    public string? Error { get; set; } = null;
    public List<ShipConfig>? ShipConfigs { get; set; } = new List<ShipConfig>()
    {
    };
    
    public EShipTouchRule EShipTouchRule { get; set; } = EShipTouchRule.NotDefined;

    public override string ToString()
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(this, jsonOptions);
    }
}