using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace BattleShipBrain
{
    public class BSBrain
    {
        private int _currentPlayerNo = 0;
        private GameBoard[] GameBoards = new GameBoard[4];
        private GameConfig _gameConfig;
        private static string _basePath = "";

        private readonly Random _rnd = new Random();

        public BSBrain(GameConfig config, string basePath)
        {
            _gameConfig = config;
            _basePath = basePath;
            GameBoards[0] = new GameBoard("ships");
            GameBoards[1] = new GameBoard("mines");
            GameBoards[2] = new GameBoard("ships");
            GameBoards[3] = new GameBoard("mines");

            GameBoards[0].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            GameBoards[1].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            GameBoards[2].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            GameBoards[3].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
        }

        public string GetBasePath()
        {
            return _basePath;
        }
        
        public GameConfig GetGameConfig()
        {
            return _gameConfig;
        }

        public int GetPlayerNum()
        {
            return _currentPlayerNo;
        }
        
        public void ChangePlayerNum(int nextPlayer)
        {
            _currentPlayerNo = nextPlayer;
        }
        
        public GameConfig CreateNewConfig(int newBoardSizeX, int newBoardSizeY, EShipTouchRule newTouchRule,
            List<ShipConfig> newShipConfigs)
        { 
            GameConfig newGameConfig = new GameConfig();
            newGameConfig.BoardSizeX = newBoardSizeX;
            newGameConfig.BoardSizeY = newBoardSizeY;
            newGameConfig.EShipTouchRule = newTouchRule;
            newGameConfig.ShipConfigs = newShipConfigs;
            return newGameConfig;
        }


        
        public void FillBoards()
        {
            for (var i = 0; i < 4; i++)
            {
                for (var x = 0; x < _gameConfig.BoardSizeX; x++)
                {
                    for (var y = 0; y < _gameConfig.BoardSizeY; y++)
                    {
                        GameBoards[i].Board[x, y] = new BoardSquareState
                        {
                            IsBomb = false,
                            IsShip = false
                        };
                    }
                }
            }
        }

        public BoardSquareState[,] GetBoard(int playerNo)
        {
            return CopyOfBoard(GameBoards[playerNo].Board);
        }

        private BoardSquareState[,] CopyOfBoard(BoardSquareState[,] board)
        {
            var res = new BoardSquareState[board.GetLength(0), board.GetLength(1)];

            for (var x = 0; x < board.GetLength(0); x++)
            {
                for (var y = 0; y < board.GetLength(1); y++)
                {
                    res[x, y] = board[x, y];
                }
            }

            return res;
        }

        public void PutBomb(int x, int y, int player)
        {
            switch (player)
            {
                case 0:
                    GameBoards[0].Board[x, y].IsBomb = true;
                    GameBoards[3].Board[x, y].IsBomb = true;
                    break;
                case 1:
                    GameBoards[1].Board[x, y].IsBomb = true;
                    GameBoards[2].Board[x, y].IsBomb = true;
                    break;
            }
        }

        public void PutShip(int player, Ship ship)
        {
            switch (player)
            {
                case 0:
                    List<Ship> ships = new List<Ship>();
                    if (GameBoards[0].Ships == null)
                    {
                        ships = new List<Ship>();
                        ships.Add(ship);
                        GameBoards[0].Ships = ships;
                        foreach (var coordinate in ship.GetCords()!)
                        {
                            GameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                        }
                        break;
                    }
                    GameBoards[0].Ships.Add(ship);
                    foreach (var coordinate in ship.GetCords()!)
                    {
                        GameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                    }
                    break;
                case 1:
                    GameBoards[2].Ships.Add(ship);
                    foreach (var coordinate in ship.GetCords()!)
                    {
                        GameBoards[2].Board[coordinate.X, coordinate.Y].IsShip = true;
                    }
                    break;
            }
        }

        public string GetConfJsonStr(GameConfig config)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            
            var confJsonStr = JsonSerializer.Serialize(config, jsonOptions);

            return confJsonStr;
        }
        
        
        public static string GetFileNameConfig(string configName)
        {
            var fileNameConfig = _basePath + System.IO.Path.DirectorySeparatorChar + "Configs" + System.IO.Path.DirectorySeparatorChar + $"{configName}.json";
            return fileNameConfig;
        }
        
        public static string GetFileNameSave(string configName)
        {
            var fileNameConfig = _basePath + System.IO.Path.DirectorySeparatorChar + "Saves" + System.IO.Path.DirectorySeparatorChar + $"{configName}.json";
            return fileNameConfig;
        }

        public void SaveConfig(string configName, GameConfig config)
        {
            var fileNameStandardConfig = GetFileNameConfig(configName);

            var confJsonStr = GetConfJsonStr(config);

            Console.WriteLine($"{configName} conf is in: " + fileNameStandardConfig);
            if (!System.IO.File.Exists(fileNameStandardConfig))
            {
                Console.WriteLine($"Saving {configName} config!");
                System.IO.File.WriteAllText(fileNameStandardConfig, confJsonStr);
            }
        }
        
        public void SaveGameState(string saveName)
        {
            var fileNameSave = GetFileNameSave(saveName);

            var saveJsonStr = GetBrainJson();

            Console.WriteLine($"{saveName} save is in: " + fileNameSave);
            if (!System.IO.File.Exists(fileNameSave))
            {
                Console.WriteLine($"Saving {saveName}!");
                System.IO.File.WriteAllText(fileNameSave, saveJsonStr);
            }
        }

        public string GetBrainJson()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var dto = new SaveGameDTO();
            dto.SetGameBoards(GameBoards);
            dto.CurrentPlayerNo = _currentPlayerNo;
            var jsonStr = JsonSerializer.Serialize(dto, jsonOptions);
            return jsonStr;
        }
        
        public SaveGameDTO RestoreBrainFromJson(string saveName)
        {
            var fileNameStandardConfig = GetFileNameSave(saveName);
            SaveGameDTO saveGameDto = new SaveGameDTO();
            if (System.IO.File.Exists(fileNameStandardConfig))
            {
                Console.WriteLine("Loading config...");
                var saveText = System.IO.File.ReadAllText(fileNameStandardConfig);
                saveGameDto = JsonSerializer.Deserialize<SaveGameDTO>(saveText) ?? throw new InvalidOperationException();
            }

            return saveGameDto;
        }

        public void LoadNewGameDto(SaveGameDTO dto)
        {
            var counter = 0;
            foreach (SaveGameDTO.GameBoardDTO gameBoard in dto.GameBoards)
            {
                var listA = gameBoard.Board;


                BoardSquareState[,] str = new BoardSquareState[(int) listA?.Count, (int) listA?[0].Count];
                for (int j = 0; j < listA?.Count; j++)
                {
                    for (int i = 0; i < listA[j].Count; i++)
                    {
                        str[j, i] = listA[j][i];
                    }
                }
                

                GameBoards[counter].Board = str;
                counter++;
            }
            _currentPlayerNo = dto.CurrentPlayerNo;
        }
    }
}