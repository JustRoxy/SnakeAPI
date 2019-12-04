using System;
using SnakeAPI.Model.Game;

namespace SnakeAPI.Model
{
    public class Snapshot
    {
        public readonly Game.Game Game;

        public Snapshot() : this(GameRule.Default)
        {
        }

        public Snapshot(GameRule rule)
        {
            Status = SnapshotMode.NotStarted;
            LastClientGet = DateTime.Now;
            Game = new Game.Game(rule);
        }

        public DateTime LastClientGet { get; set; }

        public SnapshotMode Status { get; set; }
    }

    public enum SnapshotMode
    {
        NotStarted,
        OnTheGame,
        GameIsFinished
    }
}