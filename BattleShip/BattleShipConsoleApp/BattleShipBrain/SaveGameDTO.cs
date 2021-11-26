using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BattleShipBrain
{
    // DTO - Data Transfer Object
    public class SaveGameDto
    {
        public EPlayer CurrentPlayer { get; set; } = EPlayer.NotDefined;
        public GameBoardDto[] GameBoards  { get; set; } = new GameBoardDto[4];
        
        public class GameBoardDto
        {
            public List<List<BoardSquareState>>? Board { get; set; }
            public List<Ship>? Ships { get; set; }
        }

        public void SetGameBoards(GameBoard[] gameBoards)
        {
            GameBoards[0] = new GameBoardDto();
            GameBoards[1] = new GameBoardDto();
            GameBoards[2] = new GameBoardDto();
            GameBoards[3] = new GameBoardDto();

            for (int x = 0; x < 4; x++)
            {
                var board = gameBoards[x].Board;
                var list = Enumerable.Range(0, board.GetLength(0))
                    .Select(row => Enumerable.Range(0, board.GetLength(1))
                        .Select(col => board[row, col]).ToList()).ToList();

                GameBoards[x].Board = list;
                GameBoards[x].Ships = gameBoards[x].Ships;
            }
        }
    }
}