using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BattleShipBrain
{
    public class BsBrain
    {
        private static string? _basePath;

        private int _currentPlayerNo;
        private readonly GameBoard[] _gameBoards = new GameBoard[4];
        private readonly GameConfig _gameConfig;
        private bool _playerAShipDone;
        private bool _playerBShipDone;

        public BsBrain(GameConfig config, string basePath)
        {
            _gameConfig = config;
            _basePath = basePath;
            _gameBoards[0] = new GameBoard(EGameBoardType.Ships);
            _gameBoards[1] = new GameBoard(EGameBoardType.Mines);
            _gameBoards[2] = new GameBoard(EGameBoardType.Ships);
            _gameBoards[3] = new GameBoard(EGameBoardType.Mines);

            _gameBoards[0].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            _gameBoards[1].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            _gameBoards[2].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            _gameBoards[3].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
        }

        public void PlayerPlacedShips(int playerNum)
        {
            if (playerNum == 0) _playerAShipDone = true;
            if (playerNum == 1) _playerBShipDone = true;
        }

        public bool CheckPlayerPlacedShips(int playerNum)
        {
            if (playerNum == 0) return _playerAShipDone;
            if (playerNum == 1) return _playerBShipDone;
            return false;
        }

        public string GetBasePath()
        {
            return _basePath!;
        }

        public EShipTouchRule GetTouchRule()
        {
            return _gameConfig.EShipTouchRule;
        }

        public GameConfig GetGameConfig()
        {
            return _gameConfig;
        }

        public int GetPlayerNum()
        {
            return _currentPlayerNo;
        }

        public void ChangePlayerNum()
        {
            if (_currentPlayerNo == 0)
                _currentPlayerNo++;
            else
                _currentPlayerNo--;
        }

        public BoardSquareState[,] GetBoard(int playerNo)
        {
            return CopyOfBoard(_gameBoards[playerNo].Board);
        }

        private BoardSquareState[,] CopyOfBoard(BoardSquareState[,] board)
        {
            var res = new BoardSquareState[board.GetLength(0), board.GetLength(1)];

            for (var x = 0; x < board.GetLength(0); x++)
            for (var y = 0; y < board.GetLength(1); y++)
                res[x, y] = board[x, y];

            return res;
        }

        public void PutBomb(int x, int y, int player)
        {
            // 0 to put a bomb on Player A board
            // 1 to put a bomb on Player B board
            switch (player)
            {
                case 0:
                    _gameBoards[0].Board[x, y].IsBomb = true;
                    _gameBoards[3].Board[x, y].IsBomb = true;
                    break;
                case 1:
                    _gameBoards[1].Board[x, y].IsBomb = true;
                    _gameBoards[2].Board[x, y].IsBomb = true;
                    break;
            }
        }

        public bool CheckIfCanPutShip(Ship shipToPut, int player)
        {
            var currentTouchRule = GetTouchRule();
            GameConfig config = _gameConfig;
            if (player == 1) player = 2;

            switch (currentTouchRule)
            {
                case EShipTouchRule.NoTouch:
                    if (_gameBoards[player].Ships is null) return true;
                    foreach (var coordinate in shipToPut.GetCords()!)
                        for (var y = -1; y < 2; y++)
                        for (var x = -1; x < 2; x++)
                        {
                            var xForPlacing = x;
                            var yForPlacing = y;

                            if (config.BoardSizeX - 1 - coordinate.X == 0) xForPlacing = 0;
                            if (config.BoardSizeY - 1 - coordinate.Y == 0) yForPlacing = 0;

                            if (coordinate.X == 0) xForPlacing = 0;
                            if (coordinate.Y == 0) yForPlacing = 0;


                            if (_gameBoards[player].Board[coordinate.X + xForPlacing, coordinate.Y + yForPlacing]
                                .IsShip is true) return false;
                        }
                    // GameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;

                    return true;
                case EShipTouchRule.CornerTouch:
                    if (_gameBoards[player].Ships is null) return true;
                    foreach (var coordinate in shipToPut.GetCords()!)
                        for (var y = -1; y < 2; y++)
                        for (var x = -1; x < 2; x++)
                        {
                            var xForPlacing = x;
                            var yForPlacing = y;

                            if (config.BoardSizeX - 1 - coordinate.X == 0) xForPlacing = 0;
                            if (config.BoardSizeY - 1 - coordinate.Y == 0) yForPlacing = 0;

                            if (coordinate.X == 0) xForPlacing = 0;
                            if (coordinate.Y == 0) yForPlacing = 0;

                            if (yForPlacing == 1 || yForPlacing == -1) xForPlacing = 0;

                            if (_gameBoards[player].Board[coordinate.X + xForPlacing, coordinate.Y + yForPlacing]
                                .IsShip is true) return false;
                        }
                    // GameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;

                    return true;
                case EShipTouchRule.SideTouch:
                    if (_gameBoards[player].Ships is null) return true;
                    foreach (var coordinate in shipToPut.GetCords()!)
                        if (_gameBoards[player].Board[coordinate.X, coordinate.Y].IsShip is true)
                            return false;
                    // GameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                    return true;
            }

            return false;
        }

        public bool PutShip(int player, Ship ship)
        {
            switch (player)
            {
                case 0:
                    List<Ship> shipsA = new();
                    if (CheckIfCanPutShip(ship, player))
                    {
                        if (_gameBoards[0].Ships is null)
                        {
                            shipsA = new List<Ship>();
                            shipsA.Add(ship);
                            _gameBoards[0].Ships = shipsA;
                            foreach (var coordinate in ship.GetCords()!)
                                _gameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                        }
                        else
                        {
                            _gameBoards[0].Ships.Add(ship);
                            foreach (var coordinate in ship.GetCords()!)
                                _gameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                        }

                        // Ships is placed, returning true 
                        return true;
                    }

                    // Ships is not placed, returning false, did not get True in CheckIfCanPutShip
                    return false;

                case 1:
                    if (CheckIfCanPutShip(ship, player))
                    {
                        if (_gameBoards[2].Ships is null)
                        {
                            List<Ship> shipsB = new List<Ship>();
                            shipsB.Add(ship);
                            _gameBoards[2].Ships = shipsB;
                            foreach (var coordinate in ship.GetCords()!)
                                _gameBoards[2].Board[coordinate.X, coordinate.Y].IsShip = true;
                        }
                        else
                        {
                            _gameBoards[0].Ships.Add(ship);
                            foreach (var coordinate in ship.GetCords()!)
                                _gameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                            // Ships is placed, returning true 
                        }

                        return true;
                    }

                    // Ships is not placed, returning false, did not get True in CheckIfCanPutShip
                    return false;
            }

            return false;
        }

        public string GetConfJsonStr(GameConfig config)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var confJsonStr = JsonSerializer.Serialize(config, jsonOptions);

            return confJsonStr;
        }


        private static string GetFileNameConfig(string configName)
        {
            var fileNameConfig = _basePath + Path.DirectorySeparatorChar + "Configs" +
                                 Path.DirectorySeparatorChar + $"{configName}.json";
            return fileNameConfig;
        }

        private static string GetFileNameSave(string saveName)
        {
            var fileNameConfig = _basePath + Path.DirectorySeparatorChar + "Saves" +
                                 Path.DirectorySeparatorChar + $"{saveName}.json";
            return fileNameConfig;
        }

        public void SaveConfig(string configName, GameConfig config)
        {
            var fileNameStandardConfig = GetFileNameConfig(configName);

            var confJsonStr = GetConfJsonStr(config);

            Console.WriteLine($"{configName} conf is in: " + fileNameStandardConfig);
            if (!File.Exists(fileNameStandardConfig))
            {
                Console.WriteLine($"Saving {configName} config!");
                File.WriteAllText(fileNameStandardConfig, confJsonStr);
            }
        }

        public void SaveGameState(string saveName)
        {
            var fileNameSave = GetFileNameSave(saveName);

            var saveJsonStr = GetBrainJson();

            Console.WriteLine($"{saveName} save is in: " + fileNameSave);
            if (!File.Exists(fileNameSave))
            {
                Console.WriteLine($"Saving {saveName}!");
                File.WriteAllText(fileNameSave, saveJsonStr);
            }
        }

        public string GetBrainJson()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var dto = new SaveGameDto();
            dto.SetGameBoards(_gameBoards);
            dto.CurrentPlayerNo = _currentPlayerNo;
            var jsonStr = JsonSerializer.Serialize(dto, jsonOptions);
            return jsonStr;
        }

        public SaveGameDto RestoreBrainFromJson(string saveName)
        {
            var fileNameStandardConfig = GetFileNameSave(saveName);
            SaveGameDto saveGameDto = new();
            if (File.Exists(fileNameStandardConfig))
            {
                Console.WriteLine("Loading config...");
                var saveText = File.ReadAllText(fileNameStandardConfig);
                saveGameDto = JsonSerializer.Deserialize<SaveGameDto>(saveText) ??
                              throw new InvalidOperationException();
            }

            return saveGameDto;
        }

        public void LoadNewGameDto(SaveGameDto dto)
        {
            var counter = 0;
            foreach (SaveGameDto.GameBoardDto gameBoard in dto.GameBoards)
            {
                var listA = gameBoard.Board;


                BoardSquareState[,] str = new BoardSquareState[(int) listA?.Count, (int) listA?[0].Count];
                for (var j = 0; j < listA?.Count; j++)
                for (var i = 0; i < listA[j].Count; i++)
                    str[j, i] = listA[j][i];


                _gameBoards[counter].Board = str;
                counter++;
            }

            _currentPlayerNo = dto.CurrentPlayerNo;
        }
    }
}