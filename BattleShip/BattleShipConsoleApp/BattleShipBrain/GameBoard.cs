using System.Collections.Generic;

namespace BattleShipBrain
{
    public class GameBoard
    {
        public BoardSquareState[,] Board { get; set; } = null!;
        public List<Ship> Ships { get; set; } = default!;
        public EGameBoardType GameBoardType { get; set; }
        
        public GameBoard(EGameBoardType gameBoardType)
        {
            GameBoardType = gameBoardType;
        }
    }
}