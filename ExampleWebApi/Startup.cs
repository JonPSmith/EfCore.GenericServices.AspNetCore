using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using ExampleDatabase;
using ExampleWebApi.BusinessLogic;
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

            //GenericServices configuration
            services.GenericServicesSimpleSetup<ExampleDbContext>(
                new GenericServicesConfig
                {
                    DtoAccessValidateOnSave = true,  //This causes validation to happen on create/update via DTOs
                    DirectAccessValidateOnSave = true, //This causes validation to happen on direct create/update and delete
                    NoErrorOnReadSingleNull = true //When working with WebAPI you should set this flag. Responce then sends 404 on null result
                },Assembly.GetAssembly(typeof(ChangeNameDto)));

            //GenericBizRunner configuration
            services.RegisterGenericBizRunnerBasic<ExampleDbContext>();
            services.AddAutoMapper(Assembly.GetAssembly(typeof (CreateTodoBizLogic)));
            services.AddTransient(typeof(ICreateTodoBizLogic), typeof(CreateTodoBizLogic));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API V1", Version = "v1" });

                //see https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1&tabs=visual-studio%2Cvisual-studio-xml#xml-comments
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (!File.Exists(xmlPath))
                    throw new InvalidOperationException("The XML file does not exist for Swagger - see link above for more info.");
                c.IncludeXmlComments(xmlPath);
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

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
