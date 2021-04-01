using MB.Common;
using MB.Data.Access;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minded.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;

namespace MB.Application.Api
{
    public class Startup
    {
        public IWebHostEnvironment HostingEnvironment { get; }
        public IConfigurationRoot Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddConfiguration(configuration);

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(Configuration.GetSection("Logging"));

                if (HostingEnvironment.IsDevelopment())
                {
                    logging.AddConsole();
                    logging.AddDebug();
                }
            });

            RegisterContext(services);

            services.AddMinded(assembly => assembly.Name.StartsWith("MB.Business."));

            services.AddCors(options => {
                options.AddDefaultPolicy(
                builder =>
                {
                    builder
                    .WithOrigins("http://localhost:4200", "https://localhost:4200")
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader();
                });
            });
            services.AddOData();

            services.AddMvc(
                options => options.EnableEndpointRouting = false
            )
            .AddApplicationPart(typeof(BaseController).Assembly)
            .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseMvc(routeBuilder =>
            {
                routeBuilder
                    .Expand()
                    .Filter()
                    .OrderBy(QueryOptionSetting.Allowed)
                    .MaxTop(100)
                    .Count();
                routeBuilder.EnableDependencyInjection();
            });

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void RegisterContext(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString(Constants.ConfigConnectionStringName);

            services.AddDbContext<MindedBankingContext>(o => o.UseSqlServer(connectionString, b=>b.MigrationsAssembly(typeof(MindedBankingContext).Assembly.FullName)));
            //services.AddDbContextPool<MyMindedBankingContext>(options =>
            //{
            //    options.UseSqlServer(connectionString);
            //}, poolSize: 5);

            services.AddTransient<IMindedBankingContext>(service =>
                services.BuildServiceProvider().GetService<MindedBankingContext>());
        }
    }
}
