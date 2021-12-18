using System.Text.Json;
using BattleShipBrain;
using DAL;

namespace WebApplication1.DAL;

public class AccessData
{
    public static List<GameConfigSaved> GetConfigsFromDb(string? id)
    {
        List<GameConfigSaved>? listOfConfigs = new List<GameConfigSaved>();
        var db = new ApplicationDbContext();
        if (id == null)
        {
            foreach (var dbConfig in db.GameConfigSaves) listOfConfigs.Add(dbConfig);
        }
        else
        {
            foreach (var dbConfigById in db.GameConfigSaves)
            {
                if (dbConfigById!.GameConfigSavedId == int.Parse(id))
                {
                    listOfConfigs.Add(dbConfigById);
                }
            }
        }

        return listOfConfigs;
    }

    public static List<GameStateSaved> GetSavesFromDb(string? id)
    {
        List<GameStateSaved>? listOfSaves = new List<GameStateSaved>();
        var db = new ApplicationDbContext();
        if (id == null)
        {
            foreach (var dbSave in db.GameStateSaves) listOfSaves.Add(dbSave);
        }
        else
        {
            foreach (var dbSaveById in db.GameStateSaves)
            {
                if (dbSaveById!.GameStateSavedId == int.Parse(id))
                {
                    listOfSaves.Add(dbSaveById);
                }
            }
        }

        return listOfSaves;
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

    public static void CreateNewSave(string saveName, string? configId)
    {
        using var db = new ApplicationDbContext();

        var configToSave = RestoreConfigFromJson(configId);

        var brain = new BsBrain(configToSave, null);
        var gameStateSave = new GameStateSaved
        {
            SaveName = saveName,
            SavedGameStateJsnString = brain?.GetBrainJson()!
        };
        db.GameStateSaves.Add(gameStateSave);
        db.SaveChanges();
    }
    
    public static void UpdateSave(BsBrain brainOld, int saveId, string? configId)
    {
        using var db = new ApplicationDbContext();

        var configToSave = RestoreConfigFromJson(configId);

        var brain = brainOld;
        var result = db.GameStateSaves.SingleOrDefault(s => s.GameStateSavedId == saveId);
        if (result != null)
        {
            result.SavedGameStateJsnString = brain.GetBrainJson();
            db.SaveChanges();
        }
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

    public static GameConfig RestoreConfigFromJson(string? configId)
    {
        GameConfig config = new();

        using var db = new ApplicationDbContext();
        var confText =
            db.GameConfigSaves.FirstOrDefault(c => c!.GameConfigSavedId == int.Parse(configId));
        if (confText != null)
            config = JsonSerializer.Deserialize<GameConfig>(confText.GameConfigJsnString) ??
                     throw new InvalidOperationException();

        return config;
    }
    
    public static BsBrain? RestoreSaveFromJson(string? configId, string? saveId)
    {
        var saveGameDto = JsonSerializer.Deserialize<SaveGameDto>(GetSavesFromDb(saveId)![0].SavedGameStateJsnString) ?? throw new InvalidOperationException();
        BsBrain? brain = new BsBrain(RestoreConfigFromJson(configId), null);
        brain.LoadNewGameDto(saveGameDto);
        return brain;
    }
}