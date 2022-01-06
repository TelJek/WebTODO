using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BattleShipBrain.Data;

namespace BattleShipBrain;

public class BsBrain
{
    private static string? _basePath;
    private static GameConfig? _gameConfig;
    private readonly GameBoard[] _gameBoards = new GameBoard[4];

    private EPlayer _currentPlayer = EPlayer.PlayerA;
    private bool _playerAShipDone;
    private List<ShipConfig> _playerAShipsLeft;
    private bool _playerBShipDone;
    private List<ShipConfig> _playerBShipsLeft;
    private EPlayer _winnerPlayer = EPlayer.NotDefined;

    public BsBrain(GameConfig? config, string? basePath)
    {
        _gameConfig = config;
        _basePath = basePath;
        _playerAShipsLeft = _gameConfig!.ShipConfigs;
        _playerBShipsLeft = _gameConfig.ShipConfigs;
        _gameBoards[0] = new GameBoard(EGameBoardType.Ships);
        _gameBoards[1] = new GameBoard(EGameBoardType.Mines);
        _gameBoards[2] = new GameBoard(EGameBoardType.Ships);
        _gameBoards[3] = new GameBoard(EGameBoardType.Mines);

        _gameBoards[0].Board = new BoardSquareState[_gameConfig.BoardSizeX, _gameConfig.BoardSizeY];
        _gameBoards[1].Board = new BoardSquareState[_gameConfig.BoardSizeX, _gameConfig.BoardSizeY];
        _gameBoards[2].Board = new BoardSquareState[_gameConfig.BoardSizeX, _gameConfig.BoardSizeY];
        _gameBoards[3].Board = new BoardSquareState[_gameConfig.BoardSizeX, _gameConfig.BoardSizeY];
    }

    public List<ShipConfig> GetPlayerLeftShips(EPlayer player)
    {
        if (player == EPlayer.PlayerA) return _playerAShipsLeft;

        return _playerBShipsLeft;
    }

    public void UsePlayerShips(EPlayer player, string shipName)
    {
        if (player == EPlayer.PlayerA)
            foreach (var ship in _playerAShipsLeft)
                if (ship.Name == shipName && ship.Quantity > 1)
                {
                    ship.Quantity--;
                }
                else if (ship.Name == shipName && ship.Quantity == 1)
                {
                    _playerAShipsLeft.Remove(ship);
                    return;
                }

        if (player == EPlayer.PlayerB)
            foreach (var ship in _playerBShipsLeft)
                if (ship.Name == shipName && ship.Quantity > 1)
                {
                    ship.Quantity--;
                }
                else if (ship.Name == shipName && ship.Quantity == 1)
                {
                    _playerBShipsLeft.Remove(ship);
                    return;
                }
    }

    public void PlayerPlacedShips(EPlayer player)
    {
        switch (player)
        {
            case EPlayer.PlayerA:
                _playerAShipDone = true;
                break;
            case EPlayer.PlayerB:
                _playerBShipDone = true;
                break;
            case EPlayer.NotDefined:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(player), player, "Player ERROR in PlayerPlacedShips");
        }
    }

    public bool DidPlayerWon(EPlayer playerToCheckWin)
    {
        var boardIntToCheck = 0;
        if (playerToCheckWin is EPlayer.PlayerA) boardIntToCheck = 2;

        var boardToCheck = GetBoard(boardIntToCheck);
        for (var x = 0; x < boardToCheck.GetLength(0); x++)
        for (var y = 0; y < boardToCheck.GetLength(1); y++)
            if (boardToCheck[x, y].IsShip && !boardToCheck[x, y].IsBomb)
                return false;

        _winnerPlayer = playerToCheckWin;
        return true;
    }

    public bool CheckPlayerPlacedShips(EPlayer player)
    {
        switch (player)
        {
            case EPlayer.PlayerA:
                return _playerAShipDone;
            case EPlayer.PlayerB:
                return _playerBShipDone;
            case EPlayer.NotDefined:
                break;
            default:
                return false;
        }

        return false;
    }

    public string GetBasePath()
    {
        return _basePath!;
    }

    public EShipTouchRule GetTouchRule()
    {
        return _gameConfig!.EShipTouchRule;
    }

    public GameConfig? GetGameConfig()
    {
        return _gameConfig;
    }

    public EPlayer GetPlayer()
    {
        return _currentPlayer;
    }

    public EPlayer GetWinner()
    {
        return _winnerPlayer;
    }

    public void ChangePlayerNum()
    {
        if (_currentPlayer is EPlayer.PlayerA)
            _currentPlayer = EPlayer.PlayerB;
        else
            _currentPlayer = EPlayer.PlayerA;
    }

    public BoardSquareState[,] GetBoard(int boardNumber)
    {
        return CopyOfBoard(_gameBoards[boardNumber].Board);
    }

    private BoardSquareState[,] CopyOfBoard(BoardSquareState[,] board)
    {
        var res = new BoardSquareState[board.GetLength(0), board.GetLength(1)];

        for (var x = 0; x < board.GetLength(0); x++)
        for (var y = 0; y < board.GetLength(1); y++)
            res[x, y] = board[x, y];

        return res;
    }

    public void PutBomb(int x, int y, EPlayer player)
    {
        switch (player)
        {
            case EPlayer.PlayerA:
                _gameBoards[0].Board[x, y].IsBomb = true;
                _gameBoards[3].Board[x, y].IsBomb = true;
                break;
            case EPlayer.PlayerB:
                _gameBoards[1].Board[x, y].IsBomb = true;
                _gameBoards[2].Board[x, y].IsBomb = true;
                break;
        }
    }

    public bool DidBombHit(int x, int y, EPlayer player)
    {
        switch (player)
        {
            case EPlayer.PlayerA:
                if (_gameBoards[0].Board[x, y].IsShip) return true;
                break;
            case EPlayer.PlayerB:
                if (_gameBoards[2].Board[x, y].IsShip) return true;
                break;
        }

        return false;
    }

    public bool CheckIfCanPutShip(Ship shipToPut, EPlayer player)
    {
        var currentTouchRule = GetTouchRule();
        var config = _gameConfig;
        var integerForBoards = 0;
        if (player is EPlayer.PlayerB) integerForBoards = 2;
        switch (currentTouchRule)
        {
            case EShipTouchRule.NoTouch:
                foreach (var coordinate in shipToPut.GetCords())
                    for (var y = -1; y < 2; y++)
                    for (var x = -1; x < 2; x++)
                    {
                        var xForPlacing = x;
                        var yForPlacing = y;

                        if (coordinate.X > config!.BoardSizeX - 1 ||
                            coordinate.Y > config.BoardSizeY - 1) return false;

                        if (config.BoardSizeX - 1 - coordinate.X == 0 && xForPlacing > 0) xForPlacing = 0;
                        if (config.BoardSizeY - 1 - coordinate.Y == 0 && yForPlacing > 0) yForPlacing = 0;

                        if (coordinate.X == 0 && xForPlacing < 0) xForPlacing = 0;
                        if (coordinate.Y == 0 && yForPlacing < 0) yForPlacing = 0;

                        if (_gameBoards[integerForBoards]
                            .Board[coordinate.X + xForPlacing, coordinate.Y + yForPlacing]
                            .IsShip) return false;
                    }

                return true;

            case EShipTouchRule.CornerTouch:
                foreach (var coordinate in shipToPut.GetCords())
                    for (var y = -1; y < 2; y++)
                    for (var x = -1; x < 2; x++)
                    {
                        var xForPlacing = x;
                        var yForPlacing = y;

                        if (coordinate.X > config!.BoardSizeX - 1 ||
                            coordinate.Y > config.BoardSizeY - 1) return false;

                        if (config.BoardSizeX - 1 - coordinate.X == 0 && xForPlacing > 0) xForPlacing = 0;
                        if (config.BoardSizeY - 1 - coordinate.Y == 0 && yForPlacing > 0) yForPlacing = 0;

                        if (coordinate.X == 0 && xForPlacing < 0) xForPlacing = 0;
                        if (coordinate.Y == 0 && yForPlacing < 0) yForPlacing = 0;

                        if (yForPlacing == 1 || yForPlacing == -1) xForPlacing = 0;

                        if (_gameBoards[integerForBoards]
                            .Board[coordinate.X + xForPlacing, coordinate.Y + yForPlacing]
                            .IsShip) return false;
                    }
                // GameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;

                return true;

            case EShipTouchRule.SideTouch:
                foreach (var coordinate in shipToPut.GetCords())
                {
                    if (coordinate.X > config!.BoardSizeX - 1 ||
                        coordinate.Y > config.BoardSizeY - 1) return false;

                    if (_gameBoards[integerForBoards].Board[coordinate.X, coordinate.Y].IsShip)
                        return false;
                }

                // GameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                return true;
        }
        return false;
    }

    public bool PutShip(EPlayer player, Ship ship)
    {
        switch (player)
        {
            case EPlayer.PlayerA:
                if (CheckIfCanPutShip(ship, player))
                {
                    if (_gameBoards[0].Ships.Count < 1)
                    {
                        var shipsA = new List<Ship> {ship};
                        _gameBoards[0].Ships = shipsA;
                        foreach (var coordinate in ship.GetCords())
                            _gameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                    }
                    else
                    {
                        _gameBoards[0].Ships.Add(ship);
                        foreach (var coordinate in ship.GetCords())
                            _gameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                    }

                    UsePlayerShips(EPlayer.PlayerA, ship.Name);
                    // Ships is placed, returning true 
                    return true;
                }

                // Ships is not placed, returning false, did not get True in CheckIfCanPutShip
                return false;

            case EPlayer.PlayerB:
                if (CheckIfCanPutShip(ship, player))
                {
                    if (_gameBoards[2].Ships.Count < 1)
                    {
                        var shipsB = new List<Ship> {ship};
                        _gameBoards[2].Ships = shipsB;
                        foreach (var coordinate in ship.GetCords())
                            _gameBoards[2].Board[coordinate.X, coordinate.Y].IsShip = true;
                    }
                    else
                    {
                        _gameBoards[0].Ships.Add(ship);
                        foreach (var coordinate in ship.GetCords())
                            _gameBoards[0].Board[coordinate.X, coordinate.Y].IsShip = true;
                        // Ships is placed, returning true 
                    }

                    UsePlayerShips(EPlayer.PlayerB, ship.Name);
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


    private static string GetFileNameConfig(string? configName)
    {
        DirectoryInfo di = new(@$"{_basePath + Path.DirectorySeparatorChar + "Configs"}");
        var files = di.GetFiles("*.json");

        foreach (var file in files)
            if (string.Equals(file.Name.Split(".")[0], configName, StringComparison.CurrentCultureIgnoreCase))
            {
                var fileNameConfig = _basePath + Path.DirectorySeparatorChar + "Configs" +
                                     Path.DirectorySeparatorChar +
                                     $"{file.Name.Split(".")[0].ToUpper()}.json";
                return fileNameConfig;
            }

        return "not found";
    }

    public string GetFileNameSave(string saveName)
    {
        DirectoryInfo di = new(@$"{_basePath + Path.DirectorySeparatorChar + "Saves"}");
        var files = di.GetFiles("*.json");

        foreach (var file in files)
            if (string.Equals(file.Name.Split(".")[0], saveName, StringComparison.CurrentCultureIgnoreCase))
            {
                var fileNameConfig = _basePath + Path.DirectorySeparatorChar + "Saves" +
                                     Path.DirectorySeparatorChar +
                                     $"{file.Name.Split(".")[0]}.json";
                return fileNameConfig;
            }

        return _basePath + Path.DirectorySeparatorChar + "Saves" +
               Path.DirectorySeparatorChar +
               $"{saveName}.json";
    }

    public void SaveConfigLocal(string? configName, GameConfig config)
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

    public bool EditConfiguration(EDataLocationType configLocationType, string configOldName, string configNewName)
    {
        switch (configLocationType)
        {
            case EDataLocationType.Local:
                DirectoryInfo di = new(@$"{GetBasePath() + Path.DirectorySeparatorChar + "Configs"}");
                var files = di.GetFiles("*.json");

                var sourcePath = @$"{GetBasePath()
                                     + Path.DirectorySeparatorChar
                                     + "Configs"
                                     + Path.DirectorySeparatorChar
                                     + $"{configOldName}.json"}";

                var newName = $"{configNewName}.json";
                foreach (var file in files)
                    if (file.Name.Split(".")[0].ToUpper() == configOldName.ToUpper())
                    {
                        var directory = Path.GetDirectoryName(sourcePath);
                        var destinationPath = Path.Combine(directory!, newName);
                        File.Move(sourcePath, destinationPath);
                        return true;
                    }

                break;
        }

        return false;
    }

    public bool DeleteConfiguration(EDataLocationType configLocationType, string configName)
    {
        switch (configLocationType)
        {
            case EDataLocationType.Local:
                DirectoryInfo di = new(@$"{GetBasePath() + Path.DirectorySeparatorChar + "Configs"}");
                di.GetFiles("*.json");

                var sourcePath = @$"{GetBasePath()
                                     + Path.DirectorySeparatorChar
                                     + "Configs"
                                     + Path.DirectorySeparatorChar
                                     + $"{configName}.json"}";
                File.Delete(sourcePath);
                return true;
        }

        return false;
    }

    public bool DeleteSave(EDataLocationType saveLocationType, string saveName)
    {
        switch (saveLocationType)
        {
            case EDataLocationType.Local:
                DirectoryInfo di = new(@$"{GetBasePath() + Path.DirectorySeparatorChar + "Saves"}");
                di.GetFiles("*.json");

                var sourcePath = @$"{GetBasePath()
                                     + Path.DirectorySeparatorChar
                                     + "Saves"
                                     + Path.DirectorySeparatorChar
                                     + $"{saveName}.json"}";
                File.Delete(sourcePath);
                return true;
        }

        return false;
    }

    public string GetBrainJson()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var dto = new SaveGameDto();
        dto.SetGameBoards(_gameBoards);
        dto.CurrentPlayer = _currentPlayer;
        dto.WinnerPlayer = _winnerPlayer;
        dto.GameConfig = _gameConfig;
        dto.PlayerAShipDone = _playerAShipDone;
        dto.PlayerBShipDone = _playerBShipDone;
        dto.PlayerAShipsLeft = _playerAShipsLeft;
        dto.PlayerBShipsLeft = _playerBShipsLeft;
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
        foreach (var gameBoard in dto.GameBoards)
        {
            var listA = gameBoard.Board;


            var str = new BoardSquareState[listA!.Count, listA[0].Count];
            for (var j = 0; j < listA.Count; j++)
            for (var i = 0; i < listA[j].Count; i++)
                str[j, i] = listA[j][i];


            _gameBoards[counter].Board = str;
            counter++;
        }

        _currentPlayer = dto.CurrentPlayer;
        _winnerPlayer = dto.WinnerPlayer;
        _gameConfig = dto.GameConfig;
        _playerAShipDone = dto.PlayerAShipDone;
        _playerBShipDone = dto.PlayerBShipDone;
        _playerAShipsLeft = dto.PlayerAShipsLeft!;
        _playerBShipsLeft = dto.PlayerBShipsLeft!;
    }
}