using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BattleShipBrain
{
    public class BSBrain
    {
        private int _currentPlayerNo = 0;
        private GameBoard[] GameBoards = new GameBoard[4];
        private GameConfig _gameConfig = new GameConfig();

        private readonly Random _rnd = new Random();

        public BSBrain(GameConfig config)
        {
            _gameConfig = config;
            GameBoards[0] = new GameBoard("ships");
            GameBoards[1] = new GameBoard("bombs");
            GameBoards[2] = new GameBoard("ships");
            GameBoards[3] = new GameBoard("bombs");

            GameBoards[0].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            GameBoards[1].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            GameBoards[2].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            GameBoards[3].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
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
        
        public string GetBrainJson()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var dto = new SaveGameDTO();
            var jsonStr = JsonSerializer.Serialize(dto, jsonOptions);
            return jsonStr;
        }


        public void RestoreBrainFromJson(string json)
        {
        }
    }
}