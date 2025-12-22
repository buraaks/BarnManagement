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
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Server=JEFT;Database=BarnManagementDb;Trusted_Connection=True;TrustServerCertificate=True;",
                    ["JwtSettings:Secret"] = "integration_test_secret_key_very_long_123!",
                    ["JwtSettings:Issuer"] = "IntegrationIssuer",
                    ["JwtSettings:Audience"] = "IntegrationAudience",
                    ["JwtSettings:ExpiryMinutes"] = "60"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Mevcut DbContext'i kaldır
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbDescriptor != null) services.Remove(dbDescriptor);

                // BackgroundWorker'ları kaldır (Testleri bozabilirler)
                var workerDescriptors = services.Where(
                    d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService)).ToList();
                foreach (var descriptor in workerDescriptors)
                {
                    services.Remove(descriptor);
                }

                // appsettings.json ile birebir aynı bağlantı dizesini kullan (Kullanıcı Talebi)
                var connectionString = "Server=JEFT;Database=BarnManagementDb;Trusted_Connection=True;TrustServerCertificate=True;";

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
            }
        }
    }
}
