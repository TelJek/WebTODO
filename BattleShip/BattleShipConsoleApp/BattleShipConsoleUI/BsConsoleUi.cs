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
        private readonly string _configName;
        private readonly EDataType _configTypeData;
        private string _saveName = "NewGame";
        private EDataType _saveDataType = EDataType.Local;

        public BsConsoleUi(BsBrain brain, string confName, EDataType confTypeData)
        {
            _brain = brain;
            _configName = confName;
            _configTypeData = confTypeData;
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
        
        private List<string> ReturnDataNames()
        {
            List<string> listWithHeaderDataNames = new List<string>();
            var playerLetter = "Player Letter Not Found!";
            if (_brain!.GetPlayerNum() == 0) playerLetter = "A";
            if (_brain.GetPlayerNum() == 1) playerLetter = "B";
            listWithHeaderDataNames.Add(playerLetter);
            listWithHeaderDataNames.Add(_configName);
            listWithHeaderDataNames.Add(_saveName);
            return listWithHeaderDataNames;
        }
        
        private List<EDataType> ReturnDataTypes()
        {
            List<EDataType> listWithHeaderDataTypes = new List<EDataType>();
            listWithHeaderDataTypes.Add(_configTypeData);
            listWithHeaderDataTypes.Add(_saveDataType);
            return listWithHeaderDataTypes;
        }

        private string PlayerAMenu()
        {
            var menu = new Menu(ReturnDataNames, ReturnDataTypes, "Player A", EMenuLevel.First);
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
            var menu = new Menu(ReturnDataNames, ReturnDataTypes, "Player B", EMenuLevel.First);
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
                new MenuItem("C1", "Create new configuration", CreateNewConfig)
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
                new MenuItem("S2", "Load game", LoadNewSave)
            });
            var res = menu.Run();
            return res;
        }

        private string ShowPlayerABoards()
        {
            PlayerBoards(0);
            Console.ReadLine();
            return "";
        }
        
        private string ShowPlayerBBoards()
        {
            PlayerBoards(1);
            Console.ReadLine();
            return "";
        }
        
        private string PutShipsPlayerA()
        {
            PlayerShips(0);
            Console.ReadLine();
            return "";
        }
        
        private string PutShipsPlayerB()
        {
            PlayerShips(1);
            Console.ReadLine();
            return "";
        }
        
        private string PutMinesPlayerA()
        {
            if (_brain?.GetPlayerNum() == 0)
            {
                PlayerMines(1);
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
            if (_brain?.GetPlayerNum() == 1)
            {
                PlayerMines(3);
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
        
        private string LoadNewSave()
        {
            LoadSave();
            Console.ReadLine();
            return "";
        }
        
        private string SaveGame()
        {
            CreateSave();
            Console.ReadLine();
            return "";
        }

        public void CreateSave()
        {
            var brain = _brain;
            Console.WriteLine("=========| Saving |=========");
            Console.Write("Enter Save name: ");
            var saveName = Console.ReadLine()?.Trim()!;
            Console.Write("Saving format is: <SaveName> <Local,Db or Both>: ");
            var savingType = Console.ReadLine()?.Trim().ToUpper();
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
                _saveName = saveName!;
                if (saveType == "LOCAL") _saveDataType = EDataType.Local;
                if (saveType == "DB") _saveDataType = EDataType.DataBase;
                if (_saveDataType is EDataType.Local)
                {
                    var saveGameDto = brain?.RestoreBrainFromJson(saveName);
                    brain?.LoadNewGameDto(saveGameDto);
                }

                if (_saveDataType is EDataType.DataBase)
                {
                    var saveText = db.GameStateSaves.FirstOrDefault(s => s.SaveName == saveName);
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

        public void PlayerBoards(int playerNum)
        {
            var brain = _brain;

            if (playerNum == 0)
            {
                Console.WriteLine($"--------- | Player A Boards | --------\n");
                Console.WriteLine($"=========| Player A Board with Ships|=========");
                DrawBoard(brain!.GetBoard(0));
                Console.WriteLine($"=========| Player A Board with Mines|=========");
                DrawBoard(brain.GetBoard(1));
                Console.WriteLine("=============================================");
            }
            if (playerNum == 1)
            {
                Console.WriteLine($"--------- | Player B Boards | --------\n");
                Console.WriteLine($"=========| Player B Board with Ships|=========");
                DrawBoard(brain!.GetBoard(2));
                Console.WriteLine($"=========| Player B Board with Mines|=========");
                DrawBoard(brain.GetBoard(3));
                Console.WriteLine("=============================================");
            }
        }

        public void PlayerMines(int playerNum)
        {
            string playerLetter = "";
            if (playerNum == 3) {playerLetter = "B";}
            if (playerNum == 1) {playerLetter = "A";}
            var brain = _brain;
            brain?.ChangePlayerNum();
            Console.WriteLine($"=========| Player {playerLetter} Board with Mines |=========");
            DrawBoard(brain?.GetBoard(playerNum));
            Console.Write("Choose Y side number: ");
            var yBomb = int.Parse(Console.ReadLine()?.Trim()!);
            Console.Write("Choose X side number: ");
            var xBomb = int.Parse(Console.ReadLine()?.Trim()!);
            if (playerNum == 3) {brain.PutBomb(xBomb, yBomb, 0);}
            if (playerNum == 1) {brain.PutBomb(xBomb, yBomb, 1);}
            Console.WriteLine("=============================================");
        }
        
        public void PlayerShips(int playerNum)
        {
            string playerLetter = "";
            if (playerNum == 0) {playerLetter = "A";}
            if (playerNum == 1) {playerLetter = "B";}

            var brain = _brain;
            var gameConfig = brain?.GetGameConfig();

            if (brain!.CheckPlayerPlacedShips(playerNum) is true || gameConfig!.ShipConfigs.Count == 0)
            {
                Console.WriteLine("\nYou do not have available ships to use!\n");
                Console.WriteLine("=============================================");
            }
            
            Console.WriteLine($"=========| Player {playerLetter} Ships |=========");
            DrawBoard(brain?.GetBoard(playerNum)!);
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
                        if (brain!.PutShip(playerNum, new Ship(ship.Name, cord, ship.ShipSizeX, ship.ShipSizeY)))
                        {
                            i++;
                            shipCounter--;
                        }
                        else
                        {
                            Console.WriteLine($"\nYou cannot place Ship: {ship.Name} in X: {xShip} and Y: {yShip}!\n");
                        }
                    }
                }
                brain!.PlayerPlacedShips(playerNum);
            }

            Console.WriteLine("\nYou have placed all ships!\n");
            Console.WriteLine("=============================================");
        }

        public void ConfigMenu()
        {
            var brain = _brain;
            Console.WriteLine("=========| New Configuration |=========\n");
            Console.Write("Enter a Configuration name: ");
            var configName = Console.ReadLine()?.Trim()!;
            Console.Write("<Local, Db or Both>: ");
            var savingType = Console.ReadLine()?.Trim().ToUpper();
            Console.Write("Choose Board Size X: ");
            var boardSizeX = int.Parse(Console.ReadLine()?.Trim()!);
            Console.Write("Choose Board Size Y: ");
            var boardSizeY = int.Parse(Console.ReadLine()?.Trim()!);
            Console.Write("Choose Ship Touch Rule (NoTouch or CornerTouch or SideTouch) : ");
            var touchRuleInput = Console.ReadLine()?.Trim()!;
            var newTouchRule = NewTouchRule(touchRuleInput);
            Console.Write("Create New Ships (Enter a number how much to create): ");
            var shipsConfig = new List<ShipConfig>();
            var howMuch = int.Parse(Console.ReadLine()?.Trim()!);
            for (int x = 0; x != howMuch; x++)
            {
                shipsConfig.Add(NewShipConfig());
            }

            if (savingType is "LOCAL" or "BOTH")
            {
                brain?.SaveConfig(configName,
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
            var brain = _brain;
            var player = "";
            if (brain?.GetPlayerNum() == 0)
            {
                player = "A";
            }

            if (brain?.GetPlayerNum() == 1)
            {
                player = "B";
            }

            Console.WriteLine("=========| Error |=========");
            Console.WriteLine("It is not your Turn!");
            Console.WriteLine($"It is Player {player} Turn!");
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