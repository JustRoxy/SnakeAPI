using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SnakeAPI.Model.Game.Abstractions;

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
        private readonly List<Coordinates> _eatenFruitCord = new List<Coordinates>();

        private readonly Func<Coordinates, bool> outOfRange;
        private int _turnTime;

        public Game(GameRule rule) : this(rule.TurnTime, rule.GameBoard, rule.FoodAmount)
        {
        }

        public Game(int turnTime, GameBoard board, int foodAmount = 1)
        {
            turnNumber = 0;
            FoodAmount = foodAmount;
            this.turnTime = turnTime;
            gameBoardSize = board;

            outOfRange = coordinates => coordinates.x <= -1 || coordinates.x >= gameBoardSize.Height
                                                            || coordinates.y <= -1
                                                            || coordinates.y >= gameBoardSize.Height;
            Snake = new List<Snake> {new Snake(board.Height / 2, board.Width / 2, Direction.Top)};
            Food = new List<Food>();
            GetNewFood(FoodAmount);
        }

        public Game(GameRule rule, List<Snake> snake, List<Food> food = null) : this(rule)
        {
            Snake = snake;
            if (food != null)
            {
                Food = food;
            }
            else
            {
                Food.Clear();
                GetNewFood(FoodAmount);
            }
        }

        public int turnNumber { get; set; }

        [JsonIgnore] private int FoodAmount { get; }

        public double timeUntilNextTurnMilliseconds { get; set; }

        [JsonIgnore]
        public int turnTime
        {
            get => _turnTime;
            set => _turnTime = value <= 0 ? 1_000 : value;
        }

        public List<Snake> Snake { get; set; }
        public List<Food> Food { get; set; }

        public GameBoard gameBoardSize { get; set; }

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

        private Coordinates GetFoodCoords()
        {
            return Choose(GetNoSnakePos());
        }

        private static Coordinates Choose(List<Coordinates> poses)
        {
            if (poses.Count == 0) return Coordinates.Empty;
            var rnd = new Random();
            return poses[rnd.Next(poses.Count)];
        }

        private List<Coordinates> GetNoSnakePos()
        {
            var poses = new List<Coordinates>();
            for (var i = 0; i < gameBoardSize.Height; i++)
            for (var j = 0; j < gameBoardSize.Width; j++)
            {
                var coords = new Coordinates(i, j);
                if (Food.All(d => d.Coords != coords) && Snake.All(d => d.Coords != coords))
                {
                    var last = Snake[Snake.Count - 1];
                    var phantomSnake = new List<Snake> {new Snake(last.Coords, Reverse(last.Direction))};
                    for (var k = 0; k < _eatenFruitCord.Count; k++)
                        phantomSnake.Add(new Snake(phantomSnake[k].Coords, Reverse(phantomSnake[k].Direction)));
                    if (!(_eatenFruitCord.Any() && phantomSnake.All(it => it.Coords != coords)))
                        poses.Add(coords);
                }
            }

            return poses;
        }

        private Direction Reverse(Direction dir)
        {
            return (Direction) ((int) dir * -1);
        }


        public TurnResult Turn()
        {
            var head = Snake[0];
            //Make a step
            var nodeHead = head.Coords;
            Snake[0].Coords = CalculateStep(Snake[0]);
            for (var i = 1; i < Snake.Count; i++)
            {
                var temp = nodeHead; //Move the tail behind the head
                nodeHead = Snake[i].Coords;
                Snake[i].Coords = temp;
            }


            //Food eating realization         //Any food which don't overlaps the snake
            if (_eatenFruitCord.Count != 0 && _eatenFruitCord.Any(it => Snake.All(sit => sit.Coords != it)))
            {
                var food = _eatenFruitCord.First(it => Snake.All(sit => sit.Coords != it));
                Snake.Add(new Snake(nodeHead, Snake[0].Direction));
                _eatenFruitCord.Remove(food);
            }

            if (Food.Any(it => it.Coords == head.Coords))
            {
                var fruit = Food.First(it => it.Coords == head.Coords);
                _eatenFruitCord.Add(fruit.Coords);
                Food.Remove(fruit);
                GetNewFood();
                return TurnResult.Expand;
            }

            if (Food.Count == 0) return TurnResult.Win;
            if (outOfRange(head.Coords)) return TurnResult.OutOfBorder;

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