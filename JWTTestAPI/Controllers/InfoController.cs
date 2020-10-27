using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Utils;

namespace JWTTestAPI.Controllers
{
    [ApiController]
    [Route("api/info")]
    public class InfoController : ControllerBase
    {
        [HttpGet("infoAdmin")]
        // [Authorize(Policy = Policies.Admin)]
        public IActionResult InfoAdmin()
        {
            return Ok("Info for Admin purposes");
        }

        [HttpGet("infoUser")]
        // [Authorize(Policy = Policies.User)]
        public IActionResult InfoUser()
        {
            try
            {
                HttpContext.CustomValidateToken();

                var asd = HttpContext.User.Claims.ToList();
                return Ok("Info for User purposes");
            }
            catch (SecurityTokenExpiredException te)
            {
                return Ok(new
                {
                    Status = "Error",
                    ErrorMessage = te.Message
                });
            }
            catch (System.Exception ex)
            {
                return Ok(new
                {
                    Status = "Error",
                    ErrorMessage = ex.Message
                });
            }
            
        }        
    }
}
