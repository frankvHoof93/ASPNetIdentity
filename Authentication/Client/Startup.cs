using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                });


            services.AddControllersWithViews();
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
