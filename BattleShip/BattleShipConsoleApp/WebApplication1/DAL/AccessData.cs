using System.Text.Json;
using BattleShipBrain;
using BattleShipBrain.Data;
using DAL;

namespace WebApplication1.DAL;

public class AccessData
{
    public static List<GameConfigSaved> GetConfigsFromDb(string? id)
    {
        List<GameConfigSaved> listOfConfigs = new List<GameConfigSaved>();
        var db = new ApplicationDbContext();
        if (id == null)
        {
            foreach (var dbConfig in db.GameConfigSaves) listOfConfigs.Add(dbConfig);
        }
        else
        {
            foreach (var dbConfigById in db.GameConfigSaves)
            {
                if (dbConfigById.GameConfigSavedId == int.Parse(id))
                {
                    listOfConfigs.Add(dbConfigById);
                }
            }
        }

        return listOfConfigs;
    }

    public static List<GameStateSaved> GetSavesFromDb(string? id)
    {
        List<GameStateSaved> listOfSaves = new List<GameStateSaved>();
        var db = new ApplicationDbContext();
        if (id == null)
        {
            foreach (var dbSave in db.GameStateSaves) listOfSaves.Add(dbSave);
        }
        else
        {
            foreach (var dbSaveById in db.GameStateSaves)
            {
                if (dbSaveById.GameStateSavedId == int.Parse(id))
                {
                    listOfSaves.Add(dbSaveById);
                }
            }
        }

        return listOfSaves;
    }

    public static List<StartedGame> GetAllGamesFromDb(string? shareCode)
    {
        List<StartedGame> listOfGames = new List<StartedGame>();
        var db = new ApplicationDbContext();
        if (shareCode == null)
        {
            foreach (var dbSave in db.StartedGames) listOfGames.Add(dbSave);
        }
        else
        {
            foreach (var dbSaveById in db.StartedGames)
            {
                if (dbSaveById.ConnectCode == int.Parse(shareCode))
                {
                    listOfGames.Add(dbSaveById);
                }
            }
        }
        return listOfGames;
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
            })
        };
        db.GameConfigSaves.Add(gameConfigSave);
        db.SaveChanges();
    }

    public static void CreateNewSave(string saveName, string? configId)
    {
        using var db = new ApplicationDbContext();

        var configToSave = RestoreConfigFromJsonById(configId);

        var brain = new BsBrain(configToSave, null);
        var gameStateSave = new GameStateSaved
        {
            SaveName = saveName,
            GameStateConfigId = int.Parse(configId!),
            SavedGameStateJsnString = brain.GetBrainJson()
        };
        db.GameStateSaves.Add(gameStateSave);
        db.SaveChanges();
    }

    public static void CreateNewGame(string saveId, string? configId, int shareCode)
    {
        using var db = new ApplicationDbContext();

        var config = GetConfigsFromDb(configId)[0];
        var save = GetSavesFromDb(saveId)[0];

        var startedGame = new StartedGame()
        {
            ConfigName = config.ConfigName,
            GameConfigSavedId = config.GameConfigSavedId,
            ConnectCode = shareCode,
            GameConfigJsnString = config.GameConfigJsnString,
            GameStateConfigId = save.GameStateConfigId,
            GameStateSavedId = save.GameStateSavedId,
            SavedGameStateJsnString = save.SavedGameStateJsnString,
            SaveName = save.SaveName
        };

        db.StartedGames.Add(startedGame);
        db.SaveChanges();
    }
    
    public static void UpdateSave(BsBrain brainOld, int shareCode)
    {
        using var db = new ApplicationDbContext();
        var brain = brainOld;
        var shareCodeGame = db.StartedGames.SingleOrDefault(s => s.ConnectCode == shareCode);
        if (shareCodeGame != null)
        {
            shareCodeGame.SavedGameStateJsnString = brain.GetBrainJson();
            var saveData = db.GameStateSaves.SingleOrDefault(s => s.GameStateSavedId == shareCodeGame.GameStateSavedId);
            if (saveData != null)
            {
                saveData.SavedGameStateJsnString = shareCodeGame.SavedGameStateJsnString;
            }
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

    public static GameConfig RestoreConfigFromJson(string? configJsonString)
    {
        GameConfig config = new();

        using var db = new ApplicationDbContext();
        // var confText =
        //     db.GameConfigSaves.FirstOrDefault(c => c!.GameConfigSavedId == int.Parse(configId));
        if (configJsonString != null)
            config = JsonSerializer.Deserialize<GameConfig>(configJsonString) ??
                     throw new InvalidOperationException();

        return config;
    }
    
    public static GameConfig RestoreConfigFromJsonById(string? configId)
    {
        GameConfig config = new();

        using var db = new ApplicationDbContext();
        var confText =
            db.GameConfigSaves.FirstOrDefault(c => c.GameConfigSavedId == int.Parse(configId!));
        if (confText != null)
            config = JsonSerializer.Deserialize<GameConfig>(confText.GameConfigJsnString) ??
                     throw new InvalidOperationException();

        return config;
    }
    
    public static BsBrain RestoreSaveFromJson(string gameStateJsonString, string gameConfigJsonString)
    {
        var saveGameDto = JsonSerializer.Deserialize<SaveGameDto>(gameStateJsonString) ?? throw new InvalidOperationException();
        BsBrain brain = new BsBrain(RestoreConfigFromJson(gameConfigJsonString), null);
        brain.LoadNewGameDto(saveGameDto);
        return brain;
    }
}