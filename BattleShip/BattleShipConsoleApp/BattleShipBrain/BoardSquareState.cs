namespace BattleShipBrain
{
    public struct BoardSquareState
    {
        public int BoardSquareStateId { get; set; }
        public bool IsShip { get; set; }
        public bool IsBomb { get; set; }

        public override string ToString()
        {
            switch (IsEmpty: IsShip, IsBomb)
            {
                case (false, false):
                    return "|-E-|";
                case (false, true):
                    return "|-B-|";
                case (true, false):
                    return "|-S-|";
                case (true, true):
                    return "|-H-|";
            }
        }
    }
    
    
}