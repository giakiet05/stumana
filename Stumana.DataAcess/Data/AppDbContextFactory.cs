using System;
using Microsoft.EntityFrameworkCore;

namespace Stumana.DataAcess.Data
{
    public class AppDbContextFactory
    {
        private readonly string _connectionString = "Data Source=stumana.db"; // SQLite file-based DB
        private static readonly Lazy<AppDbContextFactory> _instance = new Lazy<AppDbContextFactory>(() => new AppDbContextFactory());
        public static AppDbContextFactory Instance => _instance.Value;

        public AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(_connectionString);
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
