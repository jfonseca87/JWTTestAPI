using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Utils;

namespace JWTTestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                Console.WriteLine("Previo a middleware de routing");
                await next();
                Console.WriteLine("Posterior a middleware de routing");
            });

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                Console.WriteLine("Previo a middleware de authentication");
                await next();
                Console.WriteLine("Posterior a middleware de authentication");
            });

            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                Console.WriteLine("Previo a middleware de authorization");
                await next();
                Console.WriteLine("Posterior a middleware de authorization");
            });

            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                Console.WriteLine("Previo a middleware del api");
                await next();
                Console.WriteLine("Posterior a middleware del api");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
