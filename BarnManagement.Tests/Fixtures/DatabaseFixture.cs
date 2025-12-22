using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

// Testlerin birbirini etkilememesi ve SQL Server üzerinde güvenli işlem yapmak için paralelliği kapatıyoruz.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BarnManagement.Tests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        private readonly List<string> _createdDatabases = new();

        public DatabaseFixture()
        {
        }

        public AppDbContext CreateContext()
        {
            var dbName = $"BarnTest_{Guid.NewGuid():N}";
            var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True;MultipleActiveResultSets=true;Connect Timeout=30";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            var context = new AppDbContext(options);
            
            context.Database.EnsureCreated();
            _createdDatabases.Add(dbName);
            
            return context;
        }

        public void Dispose()
        {
            // Testler bittiğinde veritabanlarını temizlemek için opsiyonel olarak kod eklenebilir.
            // Ancak LocalDB'de dosya kilitleri nedeniyle Dispose anında silmek bazen sorun çıkarabilir.
            // Genellikle CI/CD ortamlarında bu veritabanları otomatik temizlenir.
            foreach (var dbName in _createdDatabases)
            {
                try
                {
                    // Clean up logic if needed
                }
                catch { /* Ignore cleanup errors in tests */ }
            }
        }
    }
}
