using System.Collections.Generic;
using System.Linq;

namespace BattleShipBrain
{
    public class Ship
    {
        public string Name { get;  set; } 
        
        public  List<Coordinate> Coordinates { get;  set; }  = new List<Coordinate>();

        public int Length { get;  set; } 
        
        public Coordinate Position { get;  set; } 
        
        public int Height { get;  set; } 

        public Ship(string name, Coordinate position, int length, int height)
        {
            Name = name;
            Position = position;
            Length = length;
            Height = height;
            for (var x = 0; x < Length; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Coordinates.Add(new Coordinate(){X = position.X + x, Y = position.Y + y});
                }
            }
        }

        public List<Coordinate> GetCords()
        {
            return Coordinates;
        }
        
        public int GetShipSize() => Coordinates.Count;
        
        public int GetShipDamageCount(BoardSquareState[,] board) =>
            // count all the items that match the predicate
            Coordinates.Count(coordinate => board[coordinate.X, coordinate.Y].IsBomb);

        public bool IsShipSunk(BoardSquareState[,] board) =>
            // returns true when all the items in the list match predicate
            Coordinates.All(coordinate => board[coordinate.X, coordinate.Y].IsBomb);
    }
}