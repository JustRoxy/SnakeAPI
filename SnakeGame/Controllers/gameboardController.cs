using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnakeAPI.Model;
using SnakeAPI.Model.Auth;
using SnakeAPI.Model.Game;

namespace SnakeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameboardController : ControllerBase
    {
        private void Set(string key, string value)
        {
            var option = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1)
            };

            Response.Cookies.Append(key, value, option);
        }

        [HttpGet]
        public ActionResult<Game> Get()
        {
            if (!Request.Cookies.TryGetValue("Game", out var token))
            {
                token = UserIdentify.GenerateGuid();
                Set("Game", token);
            }

            var data = UserIdentify.GetHolder(token);


            var snapshot = data.Get(token);

            BadRequestObjectResult EndOfGame(string message)
            {
                snapshot.Status = SnapshotMode.GameIsFinished;
                data.Delete(token);
                Response.Cookies.Delete("Game");
                return BadRequest(message);
            }

            if (snapshot.Status == SnapshotMode.NotStarted)
            {
                snapshot.Status = SnapshotMode.OnTheGame;
                return snapshot.Game;
            }

            var diff = (DateTime.Now - snapshot.LastClientGet).TotalMilliseconds;

            var steps = (int) (diff / snapshot.Game.turnTime);
            snapshot.Game.turnNumber += steps;


            for (var i = 0; i < steps; i++)
            {
                switch (snapshot.Game.Turn())
                {
                    case TurnResult.Death:
                    case TurnResult.OutOfBorder:
                        return EndOfGame("You are dead :(");

                    case TurnResult.Win:
                        return EndOfGame("You are victorious :)");
                }

                snapshot.LastClientGet = DateTime.Now;
            }

            snapshot.Game.timeUntilNextTurnMilliseconds =
                (int) (snapshot.Game.turnTime - diff % snapshot.Game.turnTime);


            data.Edit(token, snapshot);
            return snapshot.Game;
        }
    }
}