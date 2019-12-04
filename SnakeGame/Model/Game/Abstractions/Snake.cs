using System;
using System.Text.Json.Serialization;

namespace SnakeAPI.Model.Game.Abstractions
{
    public class Snake
    {
        //Top (1), Bottom (-1) -> Opposite -> |1| == |-1|
        //Left (2), Right (-2) -> Opposite -> |2| == |-2|


        private Direction _direction;

        public Snake(int x, int y, Direction dir) : this(new Coordinates(x, y), dir)
        {
        }

        public Snake(Coordinates coords, Direction dir)
        {
            Coords = coords;
            _direction = dir;
        }

        [JsonIgnore] public Coordinates Coords { get; set; }

        public int x => Coords.x;
        public int y => Coords.y;

        [JsonIgnore]
        public Direction Direction
        {
            get => _direction;
            set
            {
                if (!IsOpposite(_direction, value)) _direction = value;
            }
        }

        private bool IsOpposite(Direction direction1, Direction direction2)
        {
            return Math.Abs((int) direction1) == Math.Abs((int) direction2);
        }
    }
}