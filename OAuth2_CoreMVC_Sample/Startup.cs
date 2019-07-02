using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using OAuth2_CoreMVC_Sample.Helper;
using OAuth2_CoreMVC_Sample.Models;

namespace OAuth2_CoreMVC_Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            builder.AddUserSecrets<Startup>();
            Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<TokensContext>(options =>
        options.UseSqlServer(Configuration["DBConnectionString"]));
           
            services.AddTransient<IServices, Services>();
            services.Configure<OAuth2Keys>(Configuration.GetSection("OAuth2Keys"));

            services.AddMvc();
            services.AddSingleton<IConfiguration>(provider => Configuration);

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultChallengeScheme = "QBO";
            })
               .AddCookie()
               .AddOAuth("QBO",options =>
               {
                   options.ClientId = OAuth2Keys.ClientId;
                   options.ClientSecret = OAuth2Keys.ClientSecret;
                   options.CallbackPath = new PathString("/");
                   options.AuthorizationEndpoint = OAuth2Keys.AuthURL;
                   options.TokenEndpoint = OAuth2Keys.AuthURL;
               
                   options.SaveTokens = true;
                   options.Events = new OAuthEvents
                   {
                       
                       OnRedirectToAuthorizationEndpoint =  context =>
                       {
                           context.HttpContext.Response.Redirect(context.RedirectUri);
                           return Task.CompletedTask;
                       }

                   };
               });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        private void CheckCookies(CookieAuthenticationOptions o)
        {
            throw new NotImplementedException();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
  

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Connect}/{action=Index}/{id?}");
            });
        }
    }
}
