using System.Collections.Generic;
using NUnit.Framework;
using SnakeAPI.Model.Game;
using SnakeAPI.Model.Game.Abstractions;

namespace SnakeAPI.Tests.Game.Tests
{
    [TestFixture]
    internal class SnakeTests
    {
        [SetUp]
        public void Init()
        {
            _game = new Model.Game.Game(new GameRule(new GameBoard(10, 10), 100, 1));
        }

        private Model.Game.Game _game;

        private void TurnAndCompareFood(int expected, Direction? dir = null)
        {
            if (!dir.HasValue) dir = _game.Snake[0].Direction;
            _game.Snake[0].Direction = dir.Value;
            _game.Turn();
            Assert.AreEqual(expected, _game.Food.Count);
        }

        private void PatternCompare(int start, int end, IEnumerable<Direction> dirs)
        {
            foreach (var direction in dirs)
            {
                TurnAndCompareFood(start, direction);
                if (start == end) return;
                start--;
            }
        }

        [Test]
        public void FoodTest()
        {
            _game = new Model.Game.Game(
                new GameRule(new GameBoard(3, 3), 100, 8),
                new List<Snake> {new Snake(2, 0, Direction.Right)});

            PatternCompare(7, 0, new[]
            {
                Direction.Right,
                Direction.Bottom,
                Direction.Left,
                Direction.Bottom,
                Direction.Right,
                Direction.Right,
                Direction.Top,
                Direction.Top
            });

            Assert.AreEqual(TurnResult.Win, _game.Turn());
        }

        [Test]
        public void Game_is_Winnable_and_ForwardFoodTest()
        {
            _game = new Model.Game.Game(new GameRule(new GameBoard(3, 3), 100, 8),
                new List<Snake> {new Snake(2, 0, Direction.Bottom)});
            PatternCompare(7, 0, new[]
            {
                Direction.Bottom,
                Direction.Bottom,
                Direction.Right,
                Direction.Top,
                Direction.Top,
                Direction.Right,
                Direction.Bottom,
                Direction.Bottom
            });
        }

        [Test]
        public void LongSnake_is_Expanded()
        {
            _game.gameBoardSize = new GameBoard(100, 100);
            _game.Food = new List<Food> {new Food(5, 2), new Food(6, 2)};
            _game.Snake = new List<Snake>
            {
                new Snake(4, 2, Direction.Top),
                new Snake(3, 2, Direction.Top),
                new Snake(2, 2, Direction.Top),
                new Snake(1, 2, Direction.Top)
            };
            _game.Turn();
            _game.Food = new List<Food> {new Food(99, 99), new Food(6, 2)};

            Assert.AreEqual(4, _game.Snake.Count, "Змейка сразу же добавляет хвост.");
            _game.Turn();
            _game.Food = new List<Food> {new Food(99, 99), new Food(98, 98)};

            Assert.AreEqual(4, _game.Snake.Count, "Змейка добавляет хвост через ход, после того, как съела еду");
            for (var i = 0; i < 2; i++) _game.Turn();
            Assert.AreEqual(4, _game.Snake.Count, "Не соблюдается дохождения хвоста до точки, где змейка съела еду");
            _game.Turn();
            Assert.AreEqual(5, _game.Snake.Count, "Первый хвост не добавился");
            _game.Turn();
            Assert.AreEqual(5, _game.Snake.Count, "Удлинение первого хвоста не удлинняет создание второго хвоста");
            _game.Turn();
            Assert.AreEqual(6, _game.Snake.Count, "Второй хвост не добавился");
        }

        [Test]
        public void Snake_is_Dead()
        {
            _game.Snake = new List<Snake>
            {
                new Snake(new Coordinates(4, 2), Direction.Right),
                new Snake(new Coordinates(5, 2), Direction.Bottom),
                new Snake(new Coordinates(5, 3), Direction.Left),
                new Snake(new Coordinates(4, 3), Direction.Top),
                new Snake(new Coordinates(3, 3), Direction.Top)
            };
            Assert.AreEqual(TurnResult.Death, _game.Turn());
        }

        [Test]
        public void Snake_is_Expanded()
        {
            _game.Snake = new List<Snake> {new Snake(4, 2, Direction.Top)};
            _game.Food = new List<Food> {new Food(5, 2)};
            Assert.AreEqual(TurnResult.Expand, _game.Turn());
            _game.Turn();
            Assert.AreEqual(2, _game.Snake.Count);
        }

        [Test]
        public void Snake_is_OutOfBorder()
        {
            _game.Snake = new List<Snake>
            {
                new Snake(new Coordinates(9, 5), Direction.Top),
                new Snake(new Coordinates(8, 5), Direction.Top)
            };
            Assert.AreEqual(TurnResult.OutOfBorder, _game.Turn());
        }
    }
}