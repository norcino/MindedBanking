using MB.Common;
using MB.Data.Access;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minded.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            services.AddMinded(assembly => assembly.Name.StartsWith("Service."));            
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

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
            });

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
        }

        public static void RegisterContext(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(Constants.ConfigConnectionStringName);

            services.AddDbContextPool<MindedBankingContext>(options =>
            {
                options.UseSqlServer(connectionString);
            }, poolSize: 5);

            services.AddTransient<IMindedBankingContext>(service =>
                services.BuildServiceProvider().GetService<MindedBankingContext>());
        }
    }
}
