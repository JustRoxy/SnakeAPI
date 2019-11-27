using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnakeAPI.Model;
using SnakeAPI.Model.Game;
using System;
using System.Linq;
using SnakeAPI.Model.Auth;

namespace SnakeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameboardController : ControllerBase
    {
        public void Set(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = expireTime.HasValue ? 
                DateTime.Now.AddMinutes(expireTime.Value) :
                option.Expires = DateTime.Now.AddHours(2);

            Response.Cookies.Append(key, value, option);
        }
        [HttpGet]
        public ActionResult<Game> Get()
        {
            string token;
            if (Request.Cookies.ContainsKey("Game")) token = Request.Cookies["Game"];
            else
            {
                token = UserIdentify.GenerateGuid();
                Set("Game", token, null);
            }
            var data = UserIdentify.GetHolder(token);
            
            DirectionController.NewDirection += (hash, dir) =>
            {
                if (hash != token) return;
                var game = data.Get(hash);
                game.Game.Snake.First().Direction = dir;
                data.Edit(hash, game);
            };

            var snapshot = data.Get(token);
            switch (snapshot.Status)
            {
                case SnapshotMode.NotStarted:
                    snapshot.Status = SnapshotMode.OnTheGame;
                    return snapshot.Game;

                case SnapshotMode.Lose:
                    return BadRequest("You are dead :(");

                case SnapshotMode.Win:
                    return BadRequest("You are victorious :)");
            }
            var diff = (DateTime.Now - snapshot.LastClientGet).TotalMilliseconds;

            var steps = (int)(diff / snapshot.Game.turnTime);
            snapshot.Game.turnNumber += steps;

            for (var i = 0; i < steps; i++)
            {
                var res = snapshot.Game.Turn();
                switch (res)
                {
                    case TurnResult.Death:
                    case TurnResult.OutOfBorder:
                    {
                        snapshot.Status = SnapshotMode.Lose;
                        data.Edit(token, snapshot); 
                        Response.Cookies.Delete("Game");  
                        return BadRequest("You are dead :(");
                    }
                    case TurnResult.Win:
                    {
                        snapshot.Status = SnapshotMode.Win;
                        data.Edit(token, snapshot);
                        Response.Cookies.Delete("Game");
                        return BadRequest("You are victorious :)");
                    }
                }

                snapshot.LastClientGet = DateTime.Now;
            }

            snapshot.Game.timeUntilNextTurnMilliseconds = snapshot.Game.turnTime - diff % snapshot.Game.turnTime;


            data.Edit(token, snapshot);
            return snapshot.Game;
        }
    }
}