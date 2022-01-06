namespace BattleShipBrain;

public class StartedGame
{
    public int StartedGameId { get; set; }
    public int ConnectCode { get; set; } = default!;
    public int GameConfigSavedId { get; set; } = default!;
    public string? ConfigName { get; set; } = default!;
    public string GameConfigJsnString { get; set; } = default!;
    public int GameStateSavedId { get; set; } = default!;
    public int GameStateConfigId { get; set; } = default!;
    public string SaveName { get; set; } = default!;
    public string SavedGameStateJsnString { get; set; } = default!;
}