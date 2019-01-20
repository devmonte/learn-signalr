using AdvancedChat.Hubs;
using AdvancedChat.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedChat
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHealthChecks();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                });
            });
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    LifetimeValidator = (before, expires, token, parameters) => expires < DateTime.UtcNow.AddDays(1),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateActor = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["SecretKey"]))
                };
                //x.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //    {
                //        var accessToken = context.Request.Query["access_token"];

                //        if (!string.IsNullOrEmpty(accessToken) &&
                //            (context.HttpContext.WebSockets.IsWebSocketRequest || context.Request.Headers["Accept"] == "text/event-stream"))
                //        {
                //            context.Token = context.Request.Query["access_token"];
                //        }
                //        return Task.CompletedTask;
                //    }
                //};
            });

            services.AddSignalR();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ITokenProviderService, TokenProviderService>();

            services.AddHostedService<BtcPriceBotService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/health");
            //app.UseHttpsRedirection  TODO causing errors!
            app.UseAuthentication();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });

            app.UseMvc();
        }
    }
}
