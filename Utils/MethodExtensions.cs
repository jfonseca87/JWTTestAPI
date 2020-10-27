using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Utils
{
    public static class MethodExtensions
    {
        public static IServiceCollection AddJWTAthenticationAndValidation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = Token.GetTokenValidationParameters();
                });

            return services;
        }

        public static HttpContext CustomValidateToken(this HttpContext context)
        {
            string tokenHeader = context.Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;

            if (string.IsNullOrEmpty(tokenHeader))
            {
                throw new Exception("No token provided");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            string[] tokenArray = tokenHeader.Split(' ');

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(tokenArray[1], Token.GetTokenValidationParameters(), out validatedToken);
            context.User = (ClaimsPrincipal)principal;

            return context;
        }
    }
}
