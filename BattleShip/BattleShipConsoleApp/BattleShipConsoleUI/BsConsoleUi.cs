using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BattleShipBrain;
using DAL;


namespace BattleShipConsoleUI
{
    public class BsConsoleUi
    {
        private static BsBrain? _brain;
        private readonly string? _configName;
        private readonly EDataLocationType _configLocationTypeDataLocation;
        private string _saveName = "NewGame";
        private EDataLocationType _saveDataLocationType = EDataLocationType.Local;

        public BsConsoleUi(BsBrain brain, string? confName, EDataLocationType confLocationTypeDataLocation)
        {
            _brain = brain;
            _configName = confName;
            _configLocationTypeDataLocation = confLocationTypeDataLocation;
        }

        public static void DrawBoard(BoardSquareState[,] board)
        {
            Console.Write("     ");
            for (var ySide = 0; ySide < board.GetLength(0); ySide++)
            {
                Console.Write($"| {ySide} |");
            }

            Console.WriteLine();
            for (var y = 0; y < board.GetLength(1); y++)
            {
                Console.Write($"| {y} |");
                for (var x = 0; x < board.GetLength(0); x++)
                {
                    Console.Write(board[x, y]);
                }

                Console.WriteLine();
            }
        }

        public void DrawConsoleUi()
        {
            Console.Clear();
            
            var mainMenu = new Menu(ReturnDataNames, ReturnDataTypes, "BattleShip Main", EMenuLevel.Root);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("A", "Player A", PlayerAMenu),
                new MenuItem("B", "Player B", PlayerBMenu),
                new MenuItem("C", "Configuration", ConfigurationMenu),
                new MenuItem("S", "Saves", SavesMenu)
            });

            mainMenu.Run();
        }
        
        private List<string?> ReturnDataNames()
        {
            List<string?> listWithHeaderDataNames = new()
            {
                _brain!.GetPlayer().ToString(),
                _configName,
                _saveName
            };
            return listWithHeaderDataNames;
        }
        
        private List<EDataLocationType> ReturnDataTypes()
        {
            List<EDataLocationType> listWithHeaderDataTypes = new()
            {
                _configLocationTypeDataLocation,
                _saveDataLocationType
            };
            return listWithHeaderDataTypes;
        }

        private string PlayerAMenu()
        {
            var menu = new Menu(ReturnDataNames, ReturnDataTypes, EPlayer.PlayerA.ToString(), EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("A1", "Show my boards", ShowPlayerABoards),
                new MenuItem("A2", "Put my ships", PutShipsPlayerA),
                new MenuItem("A3", "Put mine", PutMinesPlayerA)
            });
            var res = menu.Run();
            return res;
        }
        
        private string PlayerBMenu()
        {
            var menu = new Menu(ReturnDataNames, ReturnDataTypes, EPlayer.PlayerB.ToString(), EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("B1", "Show my boards", ShowPlayerBBoards),
                new MenuItem("B2", "Put my ships", PutShipsPlayerB),
                new MenuItem("B3", "Put mines", PutMinesPlayerB),
            });
            var res = menu.Run();
            return res;
        }
        
        private string ConfigurationMenu()
        {
            var menu = new Menu(ReturnDataNames, ReturnDataTypes, "Configuration Settings", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("C1", "Create new configuration", CreateNewConfig),
                new MenuItem("C2", "Edit configuration", EditConfig),
                new MenuItem("C3", "Delete configuration", DeleteConfig)
            });
            var res = menu.Run();
            return res;
        }
        
        private string SavesMenu()
        {
            var menu = new Menu(ReturnDataNames, ReturnDataTypes, "Saves Settings", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("S1", "Save game", SaveGame),
                new MenuItem("S2", "Load game", LoadNewSave),
                new MenuItem("S3", "Delete saved game", DeleteSave)
            });
            var res = menu.Run();
            return res;
        }

        private string ShowPlayerABoards()
        {
            PlayerBoards(EPlayer.PlayerA);
            Console.ReadLine();
            return "";
        }
        
        private string ShowPlayerBBoards()
        {
            PlayerBoards(EPlayer.PlayerB);
            Console.ReadLine();
            return "";
        }
        
        private string PutShipsPlayerA()
        {
            PlayerShips(EPlayer.PlayerA);
            Console.ReadLine();
            return "";
        }
        
        private string PutShipsPlayerB()
        {
            PlayerShips(EPlayer.PlayerB);
            Console.ReadLine();
            return "";
        }
        
        private string PutMinesPlayerA()
        {
            if (_brain?.GetPlayer() is EPlayer.PlayerA)
            {
                PlayerMines(EPlayer.PlayerA);
            }
            else
            {
                ErrorWrongPlayer();
            }
            Console.ReadLine();
            return "";
        }
        
        private string PutMinesPlayerB()
        {
            if (_brain?.GetPlayer() is EPlayer.PlayerB)
            {
                PlayerMines(EPlayer.PlayerB);
            }
            else
            {
                ErrorWrongPlayer();
            }
            Console.ReadLine();
            return "";
        }
        
        private string CreateNewConfig()
        {
            ConfigMenu();
            Console.ReadLine();
            return "";
        }
        
        private string EditConfig()
        {
            ConfigEditMenu();
            Console.ReadLine();
            return "";
        }
        
        private string DeleteConfig()
        {
            ConfigDeleteMenu();
            Console.ReadLine();
            return "";
        }
        
        private string LoadNewSave()
        {
            LoadSave(); 
            return "";
        }
        
        private string SaveGame()
        {
            CreateSave();
            Console.ReadLine();
            return "";
        }
        
        private string DeleteSave()
        {
            SaveDeleteMenu();
            return "";
        }

        public void CreateSave()
        {
            var brain = _brain;
            Console.WriteLine("=========| Saving |=========");
            var saveName = "";
            var savingType = "";
            while (saveName == "" || savingType == "")
            {
                Console.Write("Enter Save name: ");
                saveName = Console.ReadLine()?.Trim()!;
                Console.Write("Saving format is: <SaveName> <Local,Db or Both>: ");
                savingType = Console.ReadLine()?.Trim().ToUpper(); 
            }
            if (savingType is "LOCAL" or "BOTH")
            {
                brain?.SaveGameState(saveName);
            }
            if (savingType is "DB" or "BOTH")
            {
                using var db = new ApplicationDbContext();
                var gameStateSave = new GameStateSaved
                {
                    SaveName = saveName,
                    SavedGameStateJsnString = brain?.GetBrainJson()!
                };
                db.GameStateSaves.Add(gameStateSave);
                db.SaveChanges();
            }
            Console.WriteLine($"Saved: Save name {saveName}");
            Console.WriteLine("=============================================");
        }

        public void LoadSave()
        {
            var brain = _brain;
            using var db = new ApplicationDbContext();
            DirectoryInfo di =
                new DirectoryInfo(@$"{brain?.GetBasePath() + System.IO.Path.DirectorySeparatorChar + "Saves"}");
            FileInfo[] files = di.GetFiles("*.json");
            Console.Clear();
            Console.WriteLine(brain?.GetBasePath() + System.IO.Path.DirectorySeparatorChar + "Saves");
            Console.WriteLine("=========| Load Save |=========\n");
            Console.WriteLine("Available: \n");
            Console.WriteLine("=========| Load Local Save |=========");
            Console.WriteLine();
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.Name.Split(".")[0]);
            }
            Console.WriteLine();
            Console.Write("=========| Load DB Save |=========\n");
            foreach (var dbConfig in db.GameStateSaves)
            {
                Console.WriteLine(dbConfig.SaveName);
            }
            Console.WriteLine();
            Console.WriteLine("=============================================");
            Console.Write("Db or Local: ");
            var saveType = Console.ReadLine()?.Trim().ToUpper();
            Console.Write("Your choice: ");
            var saveName = Console.ReadLine()?.Trim();
            if (saveType != "" && saveName != "")
            {
                _saveName = saveName;
                if (saveType == "LOCAL") _saveDataLocationType = EDataLocationType.Local;
                if (saveType == "DB") _saveDataLocationType = EDataLocationType.DataBase;
                if (_saveDataLocationType is EDataLocationType.Local)
                {
                    var saveGameDto = brain?.RestoreBrainFromJson(saveName);
                    brain?.LoadNewGameDto(saveGameDto);
                }

                if (_saveDataLocationType is EDataLocationType.DataBase)
                {
                    var saveText = db.GameStateSaves.FirstOrDefault(s => s.SaveName! == saveName!);
                    if (saveText != null)
                    {
                        var saveGameDto = JsonSerializer.Deserialize<SaveGameDto>(saveText.SavedGameStateJsnString) ?? throw new InvalidOperationException();
                        brain?.LoadNewGameDto(saveGameDto);
                    }
                } 
            }
            else
            {
                Console.WriteLine("ERROR SaveName or SaveType is empty! You have to choice smth");
                Console.ReadLine();
                LoadSave();
            }
        }

        public void PlayerBoards(EPlayer player)
        {
            var brain = _brain;

            if (player is EPlayer.PlayerA)
            {
                Console.WriteLine($"--------- | Player A Boards | --------\n");
                Console.WriteLine($"=========| Player A Board with Ships|=========");
                DrawBoard(brain!.GetBoard(0));
                Console.WriteLine($"=========| Player A Board with Mines|=========");
                DrawBoard(brain.GetBoard(1));
                Console.WriteLine("=============================================");
            }
            if (player is EPlayer.PlayerB)
            {
                Console.WriteLine($"--------- | Player B Boards | --------\n");
                Console.WriteLine($"=========| Player B Board with Ships|=========");
                DrawBoard(brain!.GetBoard(2));
                Console.WriteLine($"=========| Player B Board with Mines|=========");
                DrawBoard(brain.GetBoard(3));
                Console.WriteLine("=============================================");
            }
        }

        public void PlayerMines(EPlayer player)
        {
            var brain = _brain;
            if (brain!.GetWinner() is EPlayer.NotDefined)
            {
                Console.WriteLine($"=========| {player} Board with Mines |=========");
                switch (player)
                {
                    case EPlayer.PlayerA:
                        DrawBoard(brain?.GetBoard(1)!);
                        break;
                    case EPlayer.PlayerB:
                        DrawBoard(brain?.GetBoard(3)!);
                        break;
                }
                Console.Write("Choose Y side number: ");
                var yBomb = int.Parse(Console.ReadLine()?.Trim()!);
                Console.Write("Choose X side number: ");
                var xBomb = int.Parse(Console.ReadLine()?.Trim()!);
            
                var playerToHit = EPlayer.NotDefined;
                if (player is EPlayer.PlayerA) playerToHit = EPlayer.PlayerB;
                if (player is EPlayer.PlayerB) playerToHit = EPlayer.PlayerA;
            
                if (brain!.DidBombHit(xBomb, yBomb, playerToHit))
                {
                    Console.WriteLine("Nice hit!\n");
                }
                else
                {
                    Console.WriteLine("You miss!\n");
                    brain.ChangePlayerNum();
                }
                brain!.PutBomb(xBomb, yBomb, playerToHit);
            
                // check if player won
                if (brain.DidPlayerWon(player))
                {
                    Console.WriteLine("You won!\n");
                    Console.WriteLine($"{playerToHit} have lost all ships!\n");
                }
            
                Console.WriteLine("=============================================");
            }
            else
            {
                Console.WriteLine("=============================================");
                Console.WriteLine($"\n {brain.GetWinner()} has already won! GG \n");
                Console.WriteLine("=============================================");
            }

        }
        
        public void PlayerShips(EPlayer player)
        {
            var brain = _brain;
            var gameConfig = brain?.GetGameConfig();

            if (brain!.CheckPlayerPlacedShips(player) is true || gameConfig!.ShipConfigs.Count == 0)
            {
                Console.WriteLine("\nYou do not have available ships to use!\n");
                Console.WriteLine("=============================================");
                
            }
            else
            {
                Console.WriteLine($"=========| {player} Ships |=========");
                if (player is EPlayer.PlayerA) DrawBoard(brain?.GetBoard(0)!);
                if (player is EPlayer.PlayerB) DrawBoard(brain?.GetBoard(1)!);
                Console.WriteLine("=============================================");
            
                if (gameConfig!.ShipConfigs.Count != 0)
                {
                    foreach (var ship in gameConfig!.ShipConfigs)
                    {
                        int shipCounter = ship.Quantity;
                        int i = 0;
                        while (i < ship.Quantity)
                        {
                            Console.WriteLine(
                                $"Ship selected: Name {ship.Name} Quantity {shipCounter} ShipSizeX {ship.ShipSizeX} ShipSizeY {ship.ShipSizeY}");
                            Console.Write("Choose Y side number: ");
                            var yShip = int.Parse(Console.ReadLine()?.Trim()!);
                            Console.Write("Choose X side number: ");
                            var xShip = int.Parse(Console.ReadLine()?.Trim()!);
                            var cord = new Coordinate();
                            cord.X = xShip;
                            cord.Y = yShip;
                            if (brain!.PutShip(player, new Ship(ship.Name, cord, ship.ShipSizeX, ship.ShipSizeY)))
                            {
                                i++;
                                shipCounter--;
                            }
                            else
                            {
                                Console.WriteLine($"\nYou cannot place Ship: {ship.Name} in X: {yShip} and Y: {xShip}!\n");
                            }
                        }
                    }
                    brain!.PlayerPlacedShips(player);
                }
                Console.WriteLine("\nYou have placed all ships!\n");
                Console.WriteLine("=============================================");
            }
        }

        public void ConfigEditMenu()
        {
            DisplayDataByType(EDataType.Configuration);
            var inputDataConfigOldName = "";
            var inputDataConfigNewName = "";
            var configDataTypeStr = "";
            while (configDataTypeStr == "" || inputDataConfigOldName == "")
            {
                Console.Write("Db or Local: ");
                configDataTypeStr = Console.ReadLine()?.Trim().ToUpper();
                Console.Write("Enter the name of configuration you want to edit: ");
                inputDataConfigOldName = Console.ReadLine()?.Trim();
                Console.Write("Enter a new name of configuration you want to edit: ");
                inputDataConfigNewName = Console.ReadLine()?.Trim();
                Console.WriteLine();
            }

            EDataLocationType configDataLocationType = EDataLocationType.NotDefined;
            if (configDataTypeStr == "DB") configDataLocationType = EDataLocationType.DataBase;
            if (configDataTypeStr == "LOCAL") configDataLocationType = EDataLocationType.Local;
            switch (configDataLocationType)
            {
                case EDataLocationType.Local:
                {
                    if (!_brain!.EditConfiguration(configDataLocationType, inputDataConfigOldName, inputDataConfigNewName))
                    {
                        Console.WriteLine($"Local config with name {inputDataConfigOldName} was not found!");
                    }
                    Console.WriteLine($"Local config with name {inputDataConfigOldName} was renamed!");
                    break;
                }
                case EDataLocationType.DataBase:
                {
                    using var dbContext = new ApplicationDbContext();
                    GameConfigSaved? dbConfigToRename = null;
                    var canSave = false;
                    foreach (var dbConfig in dbContext.GameConfigSaves)
                    {
                        if (dbConfig!.ConfigName!.ToUpper() == inputDataConfigOldName!.ToUpper())
                        {
                            dbConfigToRename = dbConfig;
                            canSave = true;
                        }
                    }
                    if (canSave)
                    {
                        if (dbConfigToRename is not null)
                        {
                            dbConfigToRename.ConfigName = inputDataConfigNewName;
                            dbContext.SaveChanges();
                            Console.WriteLine($"DataBase config with name {inputDataConfigOldName} was renamed!");
                            return;
                        }
                    }
                    Console.WriteLine($"DataBase config with name {inputDataConfigOldName} was not found!");
                    break;
                }
            }
        }

        public void ConfigDeleteMenu()
        {
            DisplayDataByType(EDataType.Configuration);

            var inputConfigNameToDelete = "";
            var configDataTypeStr = "";
            while (configDataTypeStr == "" || inputConfigNameToDelete == "")
            {
                Console.Write("Db or Local: ");
                configDataTypeStr = Console.ReadLine()?.Trim().ToUpper();
                Console.Write("Enter the name of configuration you want to delete: ");
                inputConfigNameToDelete = Console.ReadLine()?.Trim();
                Console.WriteLine();
            }

            EDataLocationType configDataLocationType = EDataLocationType.NotDefined;
            if (configDataTypeStr == "DB") configDataLocationType = EDataLocationType.DataBase;
            if (configDataTypeStr == "LOCAL") configDataLocationType = EDataLocationType.Local;
            switch (configDataLocationType)
            {
                case EDataLocationType.Local:
                {
                    if (!_brain!.DeleteConfiguration(configDataLocationType, inputConfigNameToDelete))
                    {
                        Console.WriteLine($"Local config with name {inputConfigNameToDelete} was not found!");
                    }

                    Console.WriteLine($"Local config with name {inputConfigNameToDelete} was deleted!");
                    break;
                }
                case EDataLocationType.DataBase:
                {
                    using var dbContext = new ApplicationDbContext();
                    if (!dbContext.DeleteFromDbByName(inputConfigNameToDelete, "GameConfigSaves", "ConfigName"))
                    {
                        Console.WriteLine($"DataBase config with name {inputConfigNameToDelete} was not found!");
                    }
                    break;
                }
            }
        }

        public void SaveDeleteMenu()
        {
            DisplayDataByType(EDataType.Save);

            var inputSaveNameToDelete = "";
            var configDataTypeStr = "";
            while (configDataTypeStr == "" || inputSaveNameToDelete == "")
            {
                Console.Write("Db or Local: ");
                configDataTypeStr = Console.ReadLine()?.Trim().ToUpper();
                Console.Write("Enter the name of save you want to delete: ");
                inputSaveNameToDelete = Console.ReadLine()?.Trim();
                Console.WriteLine();
            }

            EDataLocationType saveDataLocationType = EDataLocationType.NotDefined;
            if (configDataTypeStr == "DB") saveDataLocationType = EDataLocationType.DataBase;
            if (configDataTypeStr == "LOCAL") saveDataLocationType = EDataLocationType.Local;
            switch (saveDataLocationType)
            {
                case EDataLocationType.Local:
                {
                    if (!_brain!.DeleteSave(saveDataLocationType, inputSaveNameToDelete))
                    {
                        Console.WriteLine($"Local save with name {inputSaveNameToDelete} was not found!");
                    }

                    Console.WriteLine($"Local save with name {inputSaveNameToDelete} was deleted!");
                    break;
                }
                case EDataLocationType.DataBase:
                {
                    using var dbContext = new ApplicationDbContext();
                    if (!dbContext.DeleteFromDbByName(inputSaveNameToDelete, "GameStateSaves", "SaveName"))
                    {
                        Console.WriteLine($"DataBase save with name {inputSaveNameToDelete} was not found!");
                    }
                    break;
                }
            }
        }

        public void DisplayDataByType(EDataType dataToDisplay)
        {
            using var db = new ApplicationDbContext();

            if (dataToDisplay is EDataType.Configuration)
            {
                DirectoryInfo configsDirectory = new(@$"{_brain!.GetBasePath() + Path.DirectorySeparatorChar + "Configs"}");
                FileInfo[] files = configsDirectory.GetFiles("*.json");
                Console.WriteLine("=========| Configurations |=========\n");
                Console.WriteLine("=========| Local Configurations |=========\n");
                Console.WriteLine();
                foreach (FileInfo file in files) Console.WriteLine(file.Name.Split(".")[0]);
                Console.WriteLine();
                Console.WriteLine("=========| DB Configurations |=========");
                Console.WriteLine();
                foreach (var dbConfig in db.GameConfigSaves) Console.WriteLine(dbConfig!.ConfigName); 
            }

            if (dataToDisplay is EDataType.Save)
            {
                DirectoryInfo savesDirectory = new(@$"{_brain!.GetBasePath() + Path.DirectorySeparatorChar + "Saves"}");
                FileInfo[] files = savesDirectory.GetFiles("*.json");
                Console.WriteLine("=========| Saves |=========\n");
                Console.WriteLine("=========| Local Saves |=========\n");
                Console.WriteLine();
                foreach (FileInfo file in files) Console.WriteLine(file.Name.Split(".")[0]);
                Console.WriteLine();
                Console.WriteLine("=========| DB Saves |=========");
                Console.WriteLine();
                foreach (var dbConfig in db.GameStateSaves) Console.WriteLine(dbConfig!.SaveName); 
            }

            db.Dispose();
            Console.WriteLine();
            Console.WriteLine("=============================================");
        }

        public void ConfigMenu()
        {
            var brain = _brain;
            Console.WriteLine("=========| New Configuration |=========\n");
            var configName = "";
            var savingType = "";
            var touchRuleInput = "";
            var boardSizeX = 0;
            var boardSizeY = 0;
            var howMuch = 0;
            var newTouchRule = EShipTouchRule.NotDefined;
            while (configName == "" || savingType == "" || touchRuleInput == "" || boardSizeX == 0 || boardSizeY == 0 || howMuch == 0)
            {
                Console.Write("Enter a Configuration name: ");
                configName = Console.ReadLine()?.Trim()!;
                Console.Write("<Local, Db or Both>: ");
                savingType = Console.ReadLine()?.Trim().ToUpper();
                Console.Write("Choose Board Size X: ");
                boardSizeX = int.Parse(Console.ReadLine()?.Trim()!);
                Console.Write("Choose Board Size Y: ");
                boardSizeY = int.Parse(Console.ReadLine()?.Trim()!);
                Console.Write("Choose Ship Touch Rule (NoTouch or CornerTouch or SideTouch) : ");
                touchRuleInput = Console.ReadLine()?.Trim()!;
                newTouchRule = NewTouchRule(touchRuleInput);
                Console.Write("Create New Ships (Enter a number how much to create): ");
                howMuch = int.Parse(Console.ReadLine()?.Trim()!);  
            }
            
            var shipsConfig = new List<ShipConfig>();
            for (int x = 0; x != howMuch; x++)
            {
                shipsConfig.Add(NewShipConfig());
            }

            if (savingType is "LOCAL" or "BOTH")
            {
                brain?.SaveConfigLocal(configName,
                    new GameConfig()
                    {
                        BoardSizeX = boardSizeX, BoardSizeY = boardSizeY, EShipTouchRule = newTouchRule,
                        ShipConfigs = shipsConfig
                    });
            }
            if (savingType is "DB" or "BOTH")
            {
                using var db = new ApplicationDbContext();
                var gameConfigSave = new GameConfigSaved
                {
                    ConfigName = configName,
                    GameConfigJsnString = brain?.GetConfJsonStr(new GameConfig()
                    {
                        BoardSizeX = boardSizeX, BoardSizeY = boardSizeY, EShipTouchRule = newTouchRule,
                        ShipConfigs = shipsConfig
                    })!
                };
                db.GameConfigSaves.Add(gameConfigSave);
                db.SaveChanges();
            }

            Console.WriteLine($"Configuration {configName} was Created and Saved!");

            Console.WriteLine("=============================================");
        }

        public EShipTouchRule NewTouchRule(string rule)
        {
            switch (rule)
            {
                case "NoTouch":
                    return EShipTouchRule.NoTouch;
                case "CornerTouch":
                    return EShipTouchRule.CornerTouch;
                case "SideTouch":
                    return EShipTouchRule.SideTouch;
                default:
                    return EShipTouchRule.NoTouch;
            }
        }

        private void ErrorWrongPlayer()
        {
            Console.WriteLine("=========| Error |=========");
            Console.WriteLine("It is not your Turn!");
            Console.WriteLine($"It is {_brain!.GetPlayer()} Turn!");
            Console.WriteLine("=============================================");
            Console.WriteLine("Press ENTER to continue");
        }

        public ShipConfig NewShipConfig()
        {
            Console.WriteLine("Create New Ship (Name, Quantity, ShipSizeY, ShipSizeX): ");
            Console.Write("New Ship Name: ");
            var shipName = Console.ReadLine()?.Trim()!;
            Console.Write("New Ship Quantity: ");
            var shipQuantity = int.Parse(Console.ReadLine()?.Trim()!);
            Console.Write("New Ship ShipSizeY: ");
            var shipShipSizeY = int.Parse(Console.ReadLine()?.Trim()!);
            Console.Write("New Ship ShipSizeX: ");
            var shipShipSizeX = int.Parse(Console.ReadLine()?.Trim()!);
            return new ShipConfig()
                {Name = shipName, Quantity = shipQuantity, ShipSizeY = shipShipSizeY, ShipSizeX = shipShipSizeX};
        }
    }
}