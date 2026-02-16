using BarnManagement.DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarnManagement.Tests.Integration
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private static readonly string TestConnectionString =
            Environment.GetEnvironmentVariable("TEST_SQLSERVER_CONNECTION")
            ?? "Server=JEFT;Database=BarnManagementDb;Trusted_Connection=True;TrustServerCertificate=True;";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Database:Provider"] = "SqlServer",
                    ["ConnectionStrings:DefaultConnection"] = TestConnectionString,
                    ["JwtSettings:Secret"] = "integration_test_secret_key_very_long_123!",
                    ["JwtSettings:Issuer"] = "IntegrationIssuer",
                    ["JwtSettings:Audience"] = "IntegrationAudience",
                    ["JwtSettings:ExpiryMinutes"] = "60"
                });
            });

            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbDescriptor != null) services.Remove(dbDescriptor);

                var workerDescriptors = services.Where(
                    d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService)).ToList();
                foreach (var descriptor in workerDescriptors)
                {
                    services.Remove(descriptor);
                }

                var connectionString = TestConnectionString;

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            });
        }

        public void InitializeDatabase()
        {
            using (var scope = Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
                db.Database.ExecuteSqlRaw("DELETE FROM Products; DELETE FROM Animals; DELETE FROM Farms; DELETE FROM Users;");
            }
        }
    }
}
