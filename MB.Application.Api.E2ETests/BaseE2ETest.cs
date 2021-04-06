using AnonymousData;
using Builder;
using MB.Common;
using MB.Data.Access;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;

namespace MB.Application.Api.E2ETests
{
    [TestClass]
    public abstract class BaseE2ETest
    {
        protected const int MaxPageItemNumber = 100;
        protected HttpClient _sutClient;
        private TestServer _server;
        private IMindedBankingContext _context;        
        private IConfigurationRoot _configuration;

        [TestInitialize]
        public void BaseTestTestInitialize()
        {
            _sutClient = CreateServer().CreateClient();
            ResetDb();
        }

        protected T SeedOne<T>(Expression<Func<T, int>> id, Action<T, int> buildAction = default) where T : class, new()
        {
            return Seed<T>(id, 1, buildAction).First(); ;
        }

        protected IEnumerable<T> Seed<T>(Expression<Func<T, int>> id) where T : class, new()
        {
            return Seed<T>(id, 100, default);
        }

        protected IEnumerable<T> Seed<T>(Expression<Func<T, int>> id, int quantity = 100) where T : class, new()
        {
            return Seed<T>(id, quantity, default);
        }

        protected IEnumerable<T> Seed<T>(Expression<Func<T, int>> id, int quantity = 100, Action<T, int> buildAction = default) where T : class, new()
        {
            List<T> entities = null;          
            entities = Builder<T>.New().BuildMany(quantity, (e,i) => {
                if(buildAction != null)
                    buildAction(e, i);

                ResetPrimaryKey(id, e);
            });           

            var property = _context.GetType().GetProperties()
                .First(p =>
                    p.PropertyType.IsGenericType &&
                    p.PropertyType == typeof(DbSet<T>));

            DbSet<T> dbSet = (DbSet<T>)property.GetValue(_context);
            dbSet.AddRange(entities);
            _context.SaveChanges();
          
            return entities;
        }

        private static void ResetPrimaryKey<T>(Expression<Func<T, int>> id, T e) where T : class, new()
        {
            var parameter1 = Expression.Parameter(typeof(T));
            var parameter2 = Expression.Parameter(typeof(int));

            var member = (MemberExpression)id.Body;
            var propertyInfo = (PropertyInfo)member.Member;

            var property = Expression.Property(parameter1, propertyInfo);
            var assignment = Expression.Assign(property, parameter2);

            var setter = Expression.Lambda<Action<T, int>>(assignment, parameter1, parameter2);

            setter.Compile()(e, 0);
        }

        /// <summary>
        /// Create the Server with the possibility to customize the service collection setup and custom configuration override
        /// </summary>
        /// <param name="resetDd">Allow to specify if the database must be reset, default is true</param>
        /// <param name="serviceCollectionSetup">The service collection action to customize</param>
        /// <param name="configurationOverride">Custom configuration which will override the config file</param>
        /// <returns>TestServer</returns>
        protected TestServer CreateServer(Action<IServiceCollection> serviceCollectionSetup = null,
            Dictionary<string, string> configurationOverride = null)
        {
            // Load application configuration from the test folder
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testappsettings.json", optional: false)
                .AddInMemoryCollection(configurationOverride)
                .Build();

            // Setup mocked environment object
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.SetupProperty(p => p.EnvironmentName, "Test");
            mockEnv.SetupProperty(p => p.ApplicationName, GetType().Assembly.FullName);
            mockEnv.SetupProperty(p => p.ContentRootPath, AppContext.BaseDirectory);
            mockEnv.SetupProperty(p => p.WebRootPath, AppContext.BaseDirectory);
            var env = mockEnv.Object;

            ServiceProvider serviceProvider = null;
            
            var applicationStartup = new Startup(_configuration, env);
            var builder = new WebHostBuilder().UseConfiguration(_configuration);

            builder.Configure(app =>
            {
                // Application configuration
                applicationStartup.Configure(app, env);
            });
            builder.ConfigureServices(services =>
            {
                // Application service configuration
                applicationStartup.ConfigureServices(services);

                ConfigureContext(services);

                // Execute overrides as passed in the test
                serviceCollectionSetup?.Invoke(services);

                serviceProvider = services.BuildServiceProvider();
            });

            _server = new TestServer(builder);

            _context = serviceProvider.GetService<IMindedBankingContext>();

            return _server;
        }

        private void ConfigureContext(IServiceCollection services)
        {            
            services.AddDbContext<MindedBankingContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString(Constants.ConfigConnectionStringName));                
            });

            services.AddTransient<IMindedBankingContext>(s =>
            {
                var context = s.GetService<MindedBankingContext>();
                context.Database.EnsureCreated();
                return context;
            });
        }

        /// <summary>
        /// Drop and create the testing database, this does not have effect for UnitTesting
        /// </summary>
        public void ResetDb()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
    }
}
