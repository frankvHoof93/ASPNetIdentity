using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Client
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config => {
                // Cookie is checked to confirm Authentication
                config.DefaultAuthenticateScheme = "ClientCookie";
                // Cookie is dealt out when User signs in
                config.DefaultSignInScheme = "ClientCookie";
                // Use this to check if User is allowed to perform an action
                config.DefaultChallengeScheme = "Server";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("Server", config =>
                {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    // Callback for Authorization
                    config.CallbackPath = "/oauth/callback";
                    // Redirect-path where to Authorize
                    config.AuthorizationEndpoint = "https://localhost:5001/oauth/authorize";
                    // Redirect-path where to validate Token
                    config.TokenEndpoint = "https://localhost:5001/oauth/token";

                    config.SaveTokens = true;

                    config.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = context =>
                        {
                            var accessToken = context.AccessToken;
                            var payload = accessToken.Split('.')[1]; // Base64
                            var bytes = Convert.FromBase64String(payload);
                            var jsonPayload = Encoding.UTF8.GetString(bytes);

                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                            foreach (var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
