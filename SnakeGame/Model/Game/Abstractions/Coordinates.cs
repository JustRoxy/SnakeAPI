using System;

namespace SnakeAPI.Model.Game.Abstractions
{
    public class Coordinates
    {
        public static Coordinates Empty = new Coordinates(-1, -1);
        public int x { get; set; }
        public int y { get; set; }

        public static bool operator ==(Coordinates one, Coordinates two)
        {
            if (one is null || two is null) throw new NullReferenceException("Null-Coordinates comparing");
            return one.x == two.x && one.y == two.y;
        }

        public static bool operator !=(Coordinates one, Coordinates two) => !(one == two);

        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}