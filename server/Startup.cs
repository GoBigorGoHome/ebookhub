using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ebookhub.Calibre;
using ebookhub.Controllers;
using ebookhub.Data;

namespace ebookhub
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds services required for using options.
            services.AddOptions();

            // Register the IConfiguration instance which MyOptions binds against.
            services.Configure<CalibreOptions>(Configuration.GetSection("CalibreOptions"));
            services.Configure<DatabaseOptions>(Configuration.GetSection("DatabaseOptions"));
            services.Configure<SmtpOptions>(Configuration.GetSection("SmtpOptions"));

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Add framework services.
            services.AddHangfire(config =>
            {
                // Read DefaultConnection string from appsettings.json
                var connectionString = Configuration.GetSection("DatabaseOptions").GetValue<string>("ConnectionString");
                var dbName = Configuration.GetSection("DatabaseOptions").GetValue<string>("Database");

                var migrationOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        Strategy = MongoMigrationStrategy.Migrate,
                    }
                };
                config.UseMongoStorage(connectionString, dbName, migrationOptions);
            });
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters = new List<JsonConverter> {new ObjectIdConverter()};
            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseCors(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            }

            app.UseCors(builder =>
                builder
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                );
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
