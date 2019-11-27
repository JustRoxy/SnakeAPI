using SnakeAPI.Model.Game;
using System;

namespace SnakeAPI.Model
{
    public class Snapshot
    {
        public readonly Game.Game Game;
        public DateTime LastClientGet { get; set; }

        public SnapshotMode Status { get; set; }

        public Snapshot()
        {
            Status = SnapshotMode.NotStarted;
            LastClientGet = DateTime.Now;
            Game = new Game.Game(GameRule.Default);
        }
    }

    public enum SnapshotMode
    {
        NotStarted,
        OnTheGame,
        Win,
        Lose
    }
}