using System;
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

        public void DrawUi(string level)
        {
            var brain = _brain;
            var inputInMethod = " ";
            var player = "";
            if (brain?.GetPlayerNum() == 0)
            {
                player = "A";
            }

            if (brain?.GetPlayerNum() == 1)
            {
                player = "B";
            } 

            if (level == "main")
            {
                DrawMain(player);
            }

            if (level == "playerABoards")
            {
                inputInMethod = PlayerBoards(0);
            }

            if (level == "playerBBoards")
            {
                inputInMethod = PlayerBoards(1);
            }

            if (level == "playerAShips")
            {
                inputInMethod = PlayerShips(0);
            }

            if (level == "playerBShips")
            {
                inputInMethod = PlayerShips(1);
            }

            if (level == "playerABoardMines")
            {
                if (brain?.GetPlayerNum() == 1)
                {
                    inputInMethod = PlayerMines(3);
                }
                else
                {
                    inputInMethod = ErrorWrongPlayer();
                }
            }

            if (level == "playerBBoardMines")
            {
                if (brain?.GetPlayerNum() == 0)
                {
                    inputInMethod = PlayerMines(1);
                }
                else
                {
                    inputInMethod = ErrorWrongPlayer();
                }
            }

            if (level == "configsMenu")
            {
                Console.WriteLine("=========| Configuration Menu |=========\n");
                Console.WriteLine("1) Create New Configuration\n");
                Console.WriteLine("=============================================");
                Console.Write("R to Return: ");
                inputInMethod = Console.ReadLine()?.Trim().ToUpper();
                if (inputInMethod == "1")
                {
                    inputInMethod = ConfigMenu();
                }
            }

            if (level == "savesMenu")
            {
                Console.WriteLine("=========| Saving Menu |=========\n");
                Console.WriteLine("1) Save");
                Console.WriteLine("2) Load\n");
                Console.WriteLine("=============================================");
                Console.Write("R to Return: ");
                inputInMethod = Console.ReadLine()?.Trim().ToUpper();
                if (inputInMethod == "1")
                {
                    CreateSave();
                    DrawUi("main");
                }

                if (inputInMethod == "2")
                {
                    LoadSave();
                    DrawUi("main");
                }
            }

            if (inputInMethod == "R")
            {
                DrawUi("main");
            }
        }

        public void DrawMain(string player)
        {
            Console.Clear();
            Console.WriteLine($"Loaded ConfigName: {_configName}");
            Console.WriteLine($"Loaded ConfigType: {_configTypeData}\n");
            Console.WriteLine($"Loaded SaveName: {_saveName}");
            Console.WriteLine($"Loaded SaveType: {_saveDataType}\n");
            Console.WriteLine($"EShipTouchRule: {_brain!.GetTouchRule()}\n");
            Console.WriteLine("BattleShip menu\n");
            Console.WriteLine($"Player {player} Turn\n");
            Console.WriteLine("1) Draw Player A Boards");
            Console.WriteLine("2) Draw Player B Boards");
            Console.WriteLine("3) Player A ships setup");
            Console.WriteLine("4) Player B ships setup");
            Console.WriteLine("5) Put a mine to Player A board");
            Console.WriteLine("6) Put a mine to Player B board");
            Console.WriteLine("7) Configuration settings");
            Console.WriteLine("8) Save settings\n");
            Console.WriteLine("Exit to exit\n");
            Console.Write("Your choice:");
            var input = Console.ReadLine()?.Trim().ToUpper();
            Console.WriteLine();
            RunMethod(input);
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
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim();
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

        public string? PlayerBoards(int playerNum)
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
                Console.Write("R to Return: ");
            }
            if (playerNum == 1)
            {
                Console.WriteLine($"--------- | Player B Boards | --------\n");
                Console.WriteLine($"=========| Player B Board with Ships|=========");
                DrawBoard(brain!.GetBoard(2));
                Console.WriteLine($"=========| Player B Board with Mines|=========");
                DrawBoard(brain.GetBoard(3));
                Console.WriteLine("=============================================");
                Console.Write("R to Return: ");
            }
            
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }

        public string? PlayerMines(int playerNum)
        {
            string playerLetter = "";
            if (playerNum == 3) {playerLetter = "A";}
            if (playerNum == 1) {playerLetter = "B";}
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
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }
        
        public string? PlayerShips(int playerNum)
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
                Console.Write("R to Return: ");
                var resError = Console.ReadLine()?.Trim().ToUpper();
                return resError;
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
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }

        public string? ConfigMenu()
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
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
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

        private string ErrorWrongPlayer()
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
            Console.ReadLine()?.Trim().ToUpper();
            return "R";
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

        public void RunMethod(string input)
        {
            switch (input)
            {
                case "1":
                    DrawUi("playerABoards");
                    break;
                case "2":
                    DrawUi("playerBBoards");
                    break;
                case "3":
                    DrawUi("playerAShips");
                    break;
                case "4":
                    DrawUi("playerBShips");
                    break;
                case "5":
                    DrawUi("playerABoardMines");
                    break;
                case "6":
                    DrawUi("playerBBoardMines");
                    break;
                case "7":
                    DrawUi("configsMenu");
                    break;
                case "8":
                    DrawUi("savesMenu");
                    break;
            }
        }
    }
}