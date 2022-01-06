namespace BattleShipBrain.Data;

public class StartedGame
{
    public int StartedGameId { get; set; }
    public int ConnectCode { get; set; }
    public int GameConfigSavedId { get; set; }
    public string? ConfigName { get; set; }
    public string GameConfigJsnString { get; set; } = default!;
    public int GameStateSavedId { get; set; }
    public int GameStateConfigId { get; set; }
    public string SaveName { get; set; } = default!;
    public string SavedGameStateJsnString { get; set; } = default!;
}