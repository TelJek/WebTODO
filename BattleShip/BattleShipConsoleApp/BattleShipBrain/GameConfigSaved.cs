namespace BattleShipBrain
{
    public class GameConfigSaved
    {
        public int GameConfigSavedId { get; set; }
        public string ConfigName { get; set; } = default!;
        
        public string GameConfigJsnString { get; set; } = default!;
    }
}