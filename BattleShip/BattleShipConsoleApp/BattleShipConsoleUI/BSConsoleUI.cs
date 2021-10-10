using System;
using System.IO.IsolatedStorage;
using BattleShipBrain;


namespace BattleShipConsoleUI
{
    public class BSConsoleUI
    {
        private static GameConfig? _gameConfig;
        private static BSBrain? _brain;

        public BSConsoleUI(BSBrain brain)
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

        public void DrawUi()
        {
            Console.Clear();
            Console.WriteLine("BattleShip menu");
            Console.WriteLine("1) Draw Player A Boards");
            Console.WriteLine("2) Draw Player B Boards");
            Console.WriteLine("3) Put a mine to Player A board");
            Console.WriteLine("4) Put a mine to Player B board");
            Console.Write("Your choice:");
            var input = Console.ReadLine()?.Trim().ToUpper();
            RunMethod(input);
        }

        public void RunMethod(string input)
        {
            var brain = _brain;
            var inputInMethod = " ";
            switch (input)
            {
                case "1":
                    Console.WriteLine("--------- | Player A Boards | --------");
                    Console.WriteLine("=========| Player A Board with Ships|=========");
                    DrawBoard(brain?.GetBoard(0));
                    Console.WriteLine("=========| Player A Board with Mines|=========");
                    DrawBoard(brain.GetBoard(1));
                    Console.WriteLine("=============================================");
                    Console.Write("R to Return: ");
                    inputInMethod = Console.ReadLine()?.Trim().ToUpper();
                    break;
                case "2":
                    Console.WriteLine("--------- | Player B Boards | --------");
                    Console.WriteLine("=========| Player B Board with Ships|=========");
                    DrawBoard(brain?.GetBoard(2));
                    Console.WriteLine("=========| Player B Board with Mines|=========");
                    DrawBoard(brain.GetBoard(3));
                    Console.WriteLine("=============================================");
                    Console.Write("R to Return: ");
                    inputInMethod = Console.ReadLine()?.Trim().ToUpper();
                    break;
                case "3":
                    Console.WriteLine("=========| Player B Board with Mines|=========");
                    DrawBoard(brain?.GetBoard(3));
                    Console.Write("Choose Y side number: ");
                    var yBomb = int.Parse(Console.ReadLine()?.Trim()!);
                    Console.Write("Choose X side number: ");
                    var xBomb = int.Parse(Console.ReadLine()?.Trim()!);
                    brain.PutBomb(xBomb, yBomb, 0);
                    Console.WriteLine("=============================================");
                    Console.Write("R to Return: ");
                    inputInMethod = Console.ReadLine()?.Trim().ToUpper();
                    break;
                case "4":
                    Console.WriteLine("=========| Player B Board with Mines|=========");
                    DrawBoard(brain?.GetBoard(1));
                    Console.Write("Choose Y side number: ");
                    var yBombB = int.Parse(Console.ReadLine()?.Trim()!);
                    Console.Write("Choose X side number: ");
                    var xBombB = int.Parse(Console.ReadLine()?.Trim()!);
                    brain.PutBomb(xBombB, yBombB, 1);
                    Console.WriteLine("=============================================");
                    Console.Write("R to Return: ");
                    inputInMethod = Console.ReadLine()?.Trim().ToUpper();
                    break;
            }
            if (inputInMethod == "R")
            {
                DrawUi();
            }
        }
    }
}   
