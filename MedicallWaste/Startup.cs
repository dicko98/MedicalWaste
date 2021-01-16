using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Microsoft.OpenApi.Models;
using Neo4j.Driver;
using Microsoft.AspNetCore.Identity;
using MedicallWaste.Models;
using Neo4jClient;

namespace MedicallWaste
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MediCall Waste API",
                    Description = "Official CoDi Web API",
                    Contact = new OpenApiContact
                    {
                        Name = "Danilo Markovic",
                        Url = new Uri("https://twitter.com/danilo_sb34"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Milan Stojanovic",
                        Url = new Uri("https://twitter.com/dicko_98"),
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.ResolveConflictingActions(x => x.First());
            });
            services.AddControllers();
            var neo4jClient = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "medicalwaste");
            neo4jClient.ConnectAsync().Wait();
            services.AddSingleton<IGraphClient>(neo4jClient);

            services.AddSingleton(GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "medicalwaste")));

            services.AddScoped(s => s.GetService<IDriver>().AsyncSession());
   
            //services.AddIdentity<ApplicationUser, Neo4jIdentityRole>()
            //    .AddNeo4jDataStores()
            //    .AddDefaultTokenProviders();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                                  builder =>
                                  {
                                      builder.WithOrigins("https://localhost:5001",
                                                          "https://localhost:5000",
                                                          "https://localhost:44383"
                                                          ).WithMethods("PUT", "DELETE", "GET"); ;
                                  });
            });
            //services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseSwaggerUi3(settings =>
            {
                settings.Path = "/api";
                settings.DocumentPath = "/api/specification.json";
            });

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseStaticFiles();

            app.UseSwagger(c =>
            { 
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Medicall Waste API v1");
                c.RoutePrefix = string.Empty;
                c.InjectStylesheet("/swagger-ui/custom.css");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
