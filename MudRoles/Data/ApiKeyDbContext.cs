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
