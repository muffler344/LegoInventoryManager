using LegoInventoryHelper.DatabaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LegoInventoryHelper.DatabaseContext
{
    public class LegoInventoryContext : DbContext
    {
        private readonly string sqlitePath;
        public LegoInventoryContext(IConfiguration configuration)
        {
            sqlitePath = configuration["SQLite:Path"];
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={sqlitePath}");

        public DbSet<LegoInventoryItem> LegoInventoryItems => Set<LegoInventoryItem>();
        public DbSet<Theme> Themes => Set<Theme>(); 
        public DbSet<Price> Prices => Set<Price>();
    }
}
