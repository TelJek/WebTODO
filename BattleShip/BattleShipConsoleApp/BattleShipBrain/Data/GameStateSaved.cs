namespace BattleShipBrain.Data;

public class GameStateSaved
{
    public int GameStateSavedId { get; set; }

    public int GameStateConfigId { get; set; }
    public string SaveName { get; set; } = default!;

    public string SavedGameStateJsnString { get; set; } = default!;
}