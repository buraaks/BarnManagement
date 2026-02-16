using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BarnManagement.Tests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        private readonly string _connectionString =
            Environment.GetEnvironmentVariable("TEST_SQLSERVER_CONNECTION")
            ?? "Server=JEFT;Database=BarnManagementDb;Trusted_Connection=True;TrustServerCertificate=True;";

        public AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            ClearDatabase(context);
            return context;
        }

        private void ClearDatabase(AppDbContext context)
        {
            context.Database.ExecuteSqlRaw("DELETE FROM Products; DELETE FROM Animals; DELETE FROM Farms; DELETE FROM Users;");
        }

        public void Dispose()
        {
        }
    }
}
