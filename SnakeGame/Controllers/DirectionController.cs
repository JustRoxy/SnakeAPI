using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnakeAPI.Model.Auth;
using SnakeAPI.Model.Game.Abstractions;

namespace SnakeAPI.Controllers
{
    public delegate void DirectionHandler<in THash>(THash hash, Direction dir);

    [Route("api/[controller]")]
    [ApiController]
    public class DirectionController : ControllerBase
    {
        public static event DirectionHandler<string> NewDirection;

        [HttpPost]
        public ActionResult Post([FromHeader(Name = "Direction")] Direction dir)
        {
            if (NewDirection == null) return BadRequest("No game instance");
            if (!Request.Cookies.ContainsKey("Game")) return NotFound();
            NewDirection(Request.Cookies["Game"], dir);
            return Ok();
        }
    }
}