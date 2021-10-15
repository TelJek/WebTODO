using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BattleShipBrain;


namespace BattleShipConsoleUI
{
    public class BsConsoleUi
    {
        private static BSBrain? _brain;

        public BsConsoleUi(BSBrain brain)
        {
            _brain = brain;
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
            if (brain?.GetPlayerNum() == 0) { player = "A"; }
            if (brain?.GetPlayerNum() == 1) { player = "B"; }
            
            if (level == "main")
            {
                DrawMain(player);
            }
            if (level == "playerABoards")
            {
                inputInMethod = PlayerABoards();
            }
            if (level == "playerBBoards")
            {
                inputInMethod = PlayerBBoards();
            }
            if (level == "playerAShips")
            {
                inputInMethod = PlayerAShips();
            }
            if (level == "playerBShips")
            {
                inputInMethod = PlayerBShips();
            }
            if (level == "playerABoardMines")
            {
                if (brain?.GetPlayerNum() == 1)
                {
                    inputInMethod = PlayerAMines();
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
                    inputInMethod = PlayerBMines();
                }
                else
                {
                    inputInMethod = ErrorWrongPlayer();
                }
            }
            if (level == "configsMenu")
            {
                Console.WriteLine("=========| Configuration Menu |=========");
                Console.WriteLine("1) Create New Configuration");
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
                Console.WriteLine("=========| Saving Menu |=========");
                Console.WriteLine("1) Save");
                Console.WriteLine("2) Load");
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
            Console.WriteLine("BattleShip menu");
            Console.WriteLine($"Player {player} Turn");
            Console.WriteLine("1) Draw Player A Boards");
            Console.WriteLine("2) Draw Player B Boards");
            Console.WriteLine("3) Player A ships setup");
            Console.WriteLine("4) Player B ships setup");
            Console.WriteLine("5) Put a mine to Player A board");
            Console.WriteLine("6) Put a mine to Player B board");
            Console.WriteLine("7) Configuration settings");
            Console.WriteLine("8) Save settings");
            Console.WriteLine("Exit to exit");
            Console.Write("Your choice:");
            var input = Console.ReadLine()?.Trim().ToUpper();
            RunMethod(input);
        }

        public void CreateSave()
        {
            var brain = _brain;
            Console.WriteLine("=========| Saving |=========");
            Console.Write("Enter Save name: ");
            var saveName = Console.ReadLine()?.Trim()!;
            brain?.SaveGameState(saveName);
            Console.WriteLine($"Saved: Save name {saveName}");
            Console.WriteLine("=============================================");
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim();
        }
        
        public void LoadSave()
        {
            var brain = _brain;
            DirectoryInfo di = new DirectoryInfo(@$"{brain?.GetBasePath() + System.IO.Path.DirectorySeparatorChar + "Saves"}");
            FileInfo[] files = di.GetFiles("*.json");
            Console.Clear();
            Console.WriteLine(brain?.GetBasePath() + System.IO.Path.DirectorySeparatorChar + "Saves");
            Console.WriteLine("=========| Load Configuration |=========");
            Console.WriteLine("Available: \n");
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.Name.Split(".")[0]);
            }
            Console.WriteLine();
            Console.WriteLine("=============================================");
            Console.Write("Your choice: ");
            var res = Console.ReadLine()?.Trim();
            var saveGameDto = brain?.RestoreBrainFromJson(res);
            brain?.LoadNewGameDto(saveGameDto);

        }
        
        public string? PlayerABoards()
        {
            var brain = _brain;
            Console.WriteLine("--------- | Player A Boards | --------");
            Console.WriteLine("=========| Player A Board with Ships|=========");
            DrawBoard(brain?.GetBoard(0));
            Console.WriteLine("=========| Player A Board with Mines|=========");
            DrawBoard(brain.GetBoard(1));
            Console.WriteLine("=============================================");
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }
        
        public string? PlayerBBoards()
        {
            var brain = _brain;
            Console.WriteLine("--------- | Player B Boards | --------");
            Console.WriteLine("=========| Player B Board with Ships|=========");
            DrawBoard(brain?.GetBoard(2));
            Console.WriteLine("=========| Player B Board with Mines|=========");
            DrawBoard(brain.GetBoard(3));
            Console.WriteLine("=============================================");
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }
        
        public string? PlayerAMines()
        {
            var brain = _brain;
            brain?.ChangePlayerNum(0);
            Console.WriteLine("=========| Player B Board with Mines |=========");
            DrawBoard(brain?.GetBoard(3));
            Console.Write("Choose Y side number: ");
            var yBomb = int.Parse(Console.ReadLine()?.Trim()!);
            Console.Write("Choose X side number: ");
            var xBomb = int.Parse(Console.ReadLine()?.Trim()!);
            brain.PutBomb(xBomb, yBomb, 0);
            Console.WriteLine("=============================================");
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }
        
        public string? PlayerBMines()
        {
            var brain = _brain;
            brain?.ChangePlayerNum(1);
            Console.WriteLine("=========| Player B Board with Mines |=========");
            DrawBoard(brain?.GetBoard(1));
            Console.Write("Choose Y side number: ");
            var yBombB = int.Parse(Console.ReadLine()?.Trim()!);
            Console.Write("Choose X side number: ");
            var xBombB = int.Parse(Console.ReadLine()?.Trim()!);
            brain.PutBomb(xBombB, yBombB, 1);
            Console.WriteLine("=============================================");
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }
        
        public string? PlayerAShips()
        {
            var brain = _brain;
            var gameConfig = brain?.GetGameConfig();
            
            brain?.ChangePlayerNum(0);
            Console.WriteLine("=========| Player A Ships |=========");
            DrawBoard(brain?.GetBoard(0));
            Console.WriteLine("=============================================");
            foreach (var ship in gameConfig!.ShipConfigs)
            {
                for (int i = 0; i < ship.Quantity; i++)
                {
                    Console.WriteLine($"Ship selected: Name {ship.Name} Quantity {ship.Quantity} ShipSizeX {ship.ShipSizeX} ShipSizeY {ship.ShipSizeY}");
                    Console.Write("Choose Y side number: ");
                    var yShip = int.Parse(Console.ReadLine()?.Trim()!);
                    Console.Write("Choose X side number: ");
                    var xShip = int.Parse(Console.ReadLine()?.Trim()!);
                    var cord = new Coordinate();
                    cord.X = xShip;
                    cord.Y = yShip;
                    brain.PutShip(0, new Ship(ship.Name, cord, ship.ShipSizeX, ship.ShipSizeY));
                }
            }
            Console.WriteLine("=============================================");
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }

        public string? ConfigMenu()
        {
            var brain = _brain;
            Console.WriteLine("=========| New Configuration |=========");
            Console.Write("Enter a Configuration name: ");
            var configName = Console.ReadLine()?.Trim()!;
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
            brain?.SaveConfig(configName, new GameConfig(){BoardSizeX = boardSizeX, BoardSizeY = boardSizeY, EShipTouchRule = newTouchRule, ShipConfigs = shipsConfig});
            Console.WriteLine($"Configuration {configName} was Created!");
                    
            Console.WriteLine("=============================================");
            Console.Write("R to Return: ");
            var res = Console.ReadLine()?.Trim().ToUpper();
            return res;
        }
        
        public string? PlayerBShips()
        {
            var brain = _brain;
            var gameConfig = brain?.GetGameConfig();
            
            brain?.ChangePlayerNum(0);
            Console.WriteLine("=========| Player B Ships |=========");
            DrawBoard(brain?.GetBoard(2));
            Console.WriteLine("=============================================");
            foreach (var ship in gameConfig!.ShipConfigs)
            {
                Console.WriteLine($"Ship selected: Name {ship.Name} Quantity {ship.Quantity} ShipSizeX {ship.ShipSizeX} ShipSizeY {ship.ShipSizeX}");
                Console.Write("Choose Y side number: ");
                var yShip = int.Parse(Console.ReadLine()?.Trim()!);
                Console.Write("Choose X side number: ");
                var xShip = int.Parse(Console.ReadLine()?.Trim()!);
                var cord = new Coordinate();
                cord.X = xShip;
                cord.Y = yShip;
                brain.PutShip(1, new Ship(ship.Name, cord, ship.ShipSizeX, ship.ShipSizeY));
            }
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
            if (brain?.GetPlayerNum() == 0) { player = "A"; }
            if (brain?.GetPlayerNum() == 1) { player = "B"; }
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
            return new ShipConfig() {Name = shipName, Quantity = shipQuantity, ShipSizeY = shipShipSizeY, ShipSizeX = shipShipSizeX};
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
