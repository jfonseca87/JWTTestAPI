using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API2Test.Controllers
{
    [ApiController]
    [Route("api/info")]
    public class InfoController : ControllerBase
    {
        [HttpGet("infoAdmin")]
        [Authorize(Policy = Policies.Admin)]
        public IActionResult InfoAdmin()
        {
            return Ok("Info for Admin purposes");
        }

        [HttpGet("infoUser")]
        // [AllowAnonymous]
        [Authorize(Policy = Policies.User)]
        public IActionResult InfoUser()
        {
            var asd = HttpContext.User.Claims.ToList();
            return Ok("Info for User purposes");
        }
    }
}
