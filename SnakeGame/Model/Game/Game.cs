using SnakeAPI.Model.Game.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SnakeAPI.Model.Game
{
    public struct GameBoard
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public GameBoard(int w, int h)
        {
            Width = w;
            Height = h;
        }
    }

    public class Game
    {
        public int turnNumber { get; set; }
        private int _turnTime;

        [JsonIgnore]
        private int FoodAmount { get; set; }

        public double timeUntilNextTurnMilliseconds { get; set; }

        [JsonIgnore]
        public int turnTime
        {
            get => _turnTime;
            set => _turnTime = value <= 0 ? 1_000 : value;
        }

        public IList<(Coordinates, Direction)> RotateZones = new List<(Coordinates, Direction)>();

        public List<Snake> Snake { get; set; }
        public List<Food> Food { get; set; }

        public GameBoard gameBoardSize { get; set; }

        private readonly Func<Coordinates, bool> outOfRange;

        public Game(GameRule rule) : this(rule.TurnTime, rule.GameBoard, rule.FoodAmount)
        { }

        public Game(int turnTime, GameBoard board, int foodAmount = 1)
        {
            var rnd = new Random();
            turnNumber = 0;
            FoodAmount = foodAmount;
            this.turnTime = turnTime;
            gameBoardSize = board;

            outOfRange = coordinates => coordinates.x <= -1 || coordinates.x >= gameBoardSize.Height
                                                            || coordinates.y <= -1
                                                            || coordinates.y >= gameBoardSize.Height;

            Snake = new List<Snake> { new Snake(board.Height / 2, board.Width / 2, Direction.Top) };
            Food = new List<Food>();
            GetNewFood(FoodAmount);
        }

        private void GetNewFood()
        {
            var cords = GetFoodCoords();
            if (cords != Coordinates.Empty)
                Food.Add(new Food(cords));
        }

        private void GetNewFood(int amount)
        {
            for (var i = 0; i < amount; i++)
                GetNewFood();
        }

        private Coordinates GetFoodCoords() => Choose(GetNoSnakePos());

        private static Coordinates Choose(List<List<int>> poses)
        {
            if (poses.Count == 0) return Coordinates.Empty;
            var rnd = new Random();
            var x = rnd.Next(poses.Count);
            var y = rnd.Next(poses[x].Count);
            return new Coordinates(x, y);
        }

        private List<List<int>> GetNoSnakePos()
        {
            var x = Enumerable.Range(0, gameBoardSize.Height)
                .Select(it => Enumerable.Range(0, gameBoardSize.Width).ToList()).ToList();

            if (Food.Any())
                foreach (var food in Food)
                    x[food.Coords.x].RemoveAt(food.Coords.y);

            return x;
        }

        private Coordinates _eatenFruitCord;

        public TurnResult Turn()
        {
            var head = Snake[0];
            //Make a step
            var nodeHead = head.Coords;
            Snake[0].Coords = CalculateStep(Snake[0]);
            for (var i = 1; i < Snake.Count; i++)
            {
                var temp = nodeHead;       //Move the tail behind the head
                nodeHead = Snake[i].Coords;
                Snake[i].Coords = temp;
            }

            //Food eating realization
            if (!(_eatenFruitCord is null) && Snake.All(it => it.Coords != _eatenFruitCord))
            {
                Snake.Add(new Snake(nodeHead, Snake[0].Direction));
                _eatenFruitCord = null;
            }

            if (Food.Any(it => it.Coords == head.Coords))
            {
                var fruit = Food.First(it => it.Coords == head.Coords);
                _eatenFruitCord = fruit.Coords;
                Food.Remove(fruit);
                GetNewFood();
                return TurnResult.Expand;
            }

            if (outOfRange(head.Coords)) return TurnResult.OutOfBorder;

            if (Food.Count == 0) return TurnResult.Win;
            //Check for Snake's tail'eatin' death ._.
            return Snake.Count(it => it.Coords == head.Coords) > 1 ? TurnResult.Death : TurnResult.Ok;
        }

        private Coordinates CalculateStep(Snake old)
        {
            return old.Direction switch
            {
                Direction.Top => new Coordinates(old.x + 1, old.y),
                Direction.Bottom => new Coordinates(old.x - 1, old.y),
                Direction.Left => new Coordinates(old.x, old.y - 1),
                Direction.Right => new Coordinates(old.x, old.y + 1),
                _ => throw new ArgumentOutOfRangeException(nameof(old.Direction), old.Direction, null),
            };
        }
    }

    public enum TurnResult
    {
        Death,
        Expand,
        OutOfBorder,
        Ok,
        Win
    }
}