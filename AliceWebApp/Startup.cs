using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AliceWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Diagnostics;
using OpenIddict.Models;
using AliceWebApp.Lib;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AliceWebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)

                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Db 
            services.AddDbContext<AliceContext>(opt => opt.UseInMemoryDatabase());
            services.AddDbContext<DbContext>(options =>
            {
                // Configure the context to use an in-memory store.
                options.UseInMemoryDatabase();
                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });

            services.AddCors();

            services.AddScoped<IAliceRepository, AliceCharacterRepository>();      
            
            // Add authorization
            //services.AddOpenIddict(options =>
            //            {
            //                // Register the Entity Framework stores.
            //                options.AddEntityFrameworkCoreStores<DbContext>();
            //                // Register the ASP.NET Core MVC binder used by OpenIddict.
            //                // Note: if you don't call this method, you won't be able to
            //                // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
            //                options.AddMvcBinders();
            //                // Enable the token endpoint.
            //                options.EnableTokenEndpoint("/connect/token");
            //                // Enable the password flow.
            //                options.AllowPasswordFlow();
            //                // During development, you can disable the HTTPS requirement.
            //                options.DisableHttpsRequirement();
            //            });

            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen(gen =>
            {
                gen.SwaggerDoc("v1", new Info { Title = "Alice Characters API", Version = "v1" });
                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                Debug.WriteLine("========= " + basePath);
                var xmlPath = Path.Combine(basePath, "AliceWebApp.xml");

                Debug.WriteLine("========= " + xmlPath);
                gen.IncludeXmlComments(xmlPath);
            }

            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder =>
                // This will allow any request from any server. Tweak to fit your needs!
                // The fluent API is pretty pleasant to work with.
                builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
             );

            //app.UseOpenIddict();

            //app.UseOAuthValidation();



            var auth = new Authorization();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = auth.tokenValidationParameters,
                AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme,
            });

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alice Characters API v1");
            });

        }
    }
}
