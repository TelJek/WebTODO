namespace BattleShipBrain.Data;

public class GameConfigSaved
{
    public int GameConfigSavedId { get; set; }
    public string? ConfigName { get; set; }

    public string GameConfigJsnString { get; set; } = default!;
}