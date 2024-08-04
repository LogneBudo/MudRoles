using Microsoft.EntityFrameworkCore;
using MudRoles.Data.ApiData;

namespace MudRoles.Data
{
    public class ApiKeyDbContext : DbContext
    {
        public ApiKeyDbContext(DbContextOptions<ApiKeyDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiKey>().Ignore(e => e.Scopes);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<ApiKey> ApiKeys { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Data\\apikeys.db");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
