namespace SnakeAPI.Model.Game
{
    public struct GameRule
    {
        public GameRule(GameBoard gameBoard, int turnTime, int foodAmount)
        {
            GameBoard = gameBoard;
            TurnTime = turnTime;
            FoodAmount = foodAmount;
        }

        public GameBoard GameBoard { get; }
        public int TurnTime { get; }
        public int FoodAmount { get; }
        public static GameRule Default = new GameRule(new GameBoard(20, 20), 5000, 1);
    }
}