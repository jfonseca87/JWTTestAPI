using JWTTestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace JWTTestAPI.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private List<User> users = new List<User>
        {
            new User { UserId = 1, UserName = "pepita", Email = "pepita@domain.com", Password = "Abc12345", UserRole = "Admin" },
            new User { UserId = 2, UserName = "pepito", Email = "pepito@domain.com", Password = "Abc12345", UserRole = "User" }
        };

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        private DateTime ExpireTime { get; set; }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody]User user)
        {
            User userRegistered = GetUser(user);

            if (userRegistered != null)
            {
                return Ok(new
                {
                    Token = GenerateToken(userRegistered),
                    Expires = ExpireTime.Ticks,
                    Id = userRegistered.UserId,
                    Name = userRegistered.UserName,
                    userRegistered.Email
                });
            }

            return Unauthorized();
        }

        private User GetUser(User user)
        {
            return users.SingleOrDefault(x => x.UserName.Equals(user.UserName.ToLower(), StringComparison.InvariantCultureIgnoreCase) &&
                                              x.Password.Equals(user.Password, StringComparison.InvariantCulture));
        }

        private string GenerateToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
                new Claim(ClaimTypes.Name, userInfo.UserName),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Role, userInfo.UserRole),
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            ExpireTime = DateTime.Now.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: ExpireTime,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
