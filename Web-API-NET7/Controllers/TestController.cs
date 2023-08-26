using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_API_NET7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize(Roles ="user")]
        [HttpPost("Test")]
        public IActionResult Test ()
        {
            return Ok("Test user");
        }

        [HttpGet("Dashboard"),Authorize(Roles = "admin")]
        public IActionResult Get ()
        {
            return Ok("test admin");
        }
    }
}
