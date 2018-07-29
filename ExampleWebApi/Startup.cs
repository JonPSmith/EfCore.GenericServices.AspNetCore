using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExampleDatabase;
using ExampleWebApi.Dtos;
using ExampleWebApi.Helpers;
using GenericBizRunner.Configuration;
using GenericServices.Configuration;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace ExampleWebApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //--------------------------------------------------------------------
            //var connection = Configuration.GetConnectionString("DefaultConnection");
            //Swapped over to sqlite in-memory database
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();  //see https://github.com/aspnet/EntityFramework/issues/6968
            services.AddDbContext<ExampleDbContext>(options => options.UseSqlite(connection));

            services.GenericServicesSimpleSetup<ExampleDbContext>(
                new GenericServicesConfig
                {
                    NoErrorOnReadSingleNull = true //When working with WebAPI you should set this flag. Responce then sends 404 on null result
                },
                Assembly.GetAssembly(typeof(ChangeNameDto)));
            services.RegisterGenericBizRunnerBasic<ExampleDbContext>();
            //No need to register AutoMapper as the business logic doesn't map from a DTO to another class

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API V1", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
