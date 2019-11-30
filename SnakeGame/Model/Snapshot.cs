using SnakeAPI.Model.Game;
using System;

namespace SnakeAPI.Model
{
    public class Snapshot
    {
        public readonly Game.Game Game;
        public DateTime LastClientGet { get; set; }

        public SnapshotMode Status { get; set; }

        public Snapshot() : this(GameRule.Default)
        {}
        public Snapshot(GameRule rule)
        {
            Status = SnapshotMode.NotStarted;
            LastClientGet = DateTime.Now;
            Game = new Game.Game(rule);
        }
    }

    public enum SnapshotMode
    {
        NotStarted,
        OnTheGame,
        GameIsFinished
    }
}