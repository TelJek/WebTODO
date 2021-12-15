namespace WebBattleShip.Domain
{
    public class GameStateSaved
    {
        public int GameStateSavedId { get; set; }
        public string SaveName { get; set; } = default!;
        
        public string SavedGameStateJsnString { get; set; } = default!;
    }
}