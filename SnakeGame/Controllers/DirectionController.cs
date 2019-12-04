using Microsoft.AspNetCore.Mvc;
using SnakeAPI.Model.Auth;
using SnakeAPI.Model.Game.Abstractions;

namespace SnakeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectionController : ControllerBase
    {
        [HttpPut]
        [HttpPost]
        public ActionResult Post([FromHeader(Name = "Direction")] Direction dir)
        {
            if (!Request.Cookies.ContainsKey("Game")) return NotFound();
            var cookie = Request.Cookies["Game"];

            var game = UserIdentify.StrongGetHolder(cookie)?.Get(cookie).Game;
            if (game == null) return NotFound("Token does not exist");

            game.Snake[0].Direction = dir;
            if (game.Snake[0].Direction != dir) return BadRequest("Bad direction");
            return Ok();
        }
    }
}