using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvalAppBackEnd.Controllers
{
    [ApiController]
    [Route("api/protected")]
    [Authorize]
    public class ProtectedController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AdminEndpoint()
        {
            return Ok("Hello, Admin!");
        }

        [HttpGet("player")]
        [Authorize(Policy = "PlayerOnly")]
        public IActionResult PlayerEndpoint()
        {
            return Ok("Hello, Player!");
        }
    }
}
