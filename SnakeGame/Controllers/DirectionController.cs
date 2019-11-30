using System.Linq;
using Microsoft.AspNetCore.Http;
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
            var game = UserIdentify.GetHolder(cookie).Get(cookie).Game;
            game.Snake[0].Direction = dir;
            if (game.Snake[0].Direction != dir) return BadRequest("Bad direction");
            return Ok();
        }
    }
}