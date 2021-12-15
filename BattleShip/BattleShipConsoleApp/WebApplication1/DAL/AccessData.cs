using System.Text.Json;
using BattleShipBrain;
using DAL;

namespace WebApplication1.DAL;

public class AccessData
{
    public static List<GameConfigSaved> GetDataFromDb()
    {
        List<GameConfigSaved> listOfConfigs = new List<GameConfigSaved>();
        var dbContext = new ApplicationDbContext();
        foreach (var dbConfig in dbContext.GameConfigSaves) listOfConfigs.Add(dbConfig);
        List<GameConfigSaved> listOfConfigs1 = listOfConfigs;
        return listOfConfigs;
    }

    public static void SaveConfigInDb(string configName, int boardSizeX, int boardSizeY, EShipTouchRule newTouchRule,
        List<ShipConfig> shipsConfig)
    {
        using var db = new ApplicationDbContext();
        var gameConfigSave = new GameConfigSaved
        {
            ConfigName = configName,
            GameConfigJsnString = GetConfJsonStr(new GameConfig()
            {
                BoardSizeX = boardSizeX, BoardSizeY = boardSizeY, EShipTouchRule = newTouchRule,
                ShipConfigs = shipsConfig
            })!
        };
        db.GameConfigSaves.Add(gameConfigSave);
        db.SaveChanges();
    }

    public static string GetConfJsonStr(GameConfig config)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var confJsonStr = JsonSerializer.Serialize(config, jsonOptions);

        return confJsonStr;
    }
}