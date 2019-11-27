using System.Text.Json.Serialization;

namespace SnakeAPI.Model.Game.Abstractions
{
    public class Food
    {
        [JsonIgnore]
        public Coordinates Coords { get; set; }

        public int x => Coords.x;
        public int y => Coords.y;

        public Food(int x, int y) : this(new Coordinates(x, y))
        { }

        public Food(Coordinates coords) => Coords = coords;
    }
}