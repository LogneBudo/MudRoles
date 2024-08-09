
using Microsoft.EntityFrameworkCore;
using MudRoles.Data.ApiData;

namespace MudRoles.Data
{
    /// <summary>  
    /// Represents the database context for API keys.  
    /// </summary>  
    public class ApiKeyDbContext : DbContext
    {
        /// <summary>  
        /// Initializes a new instance of the <see cref="ApiKeyDbContext"/> class.  
        /// </summary>  
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>  
        public ApiKeyDbContext(DbContextOptions<ApiKeyDbContext> options)
            : base(options)
        {
        }

        /// <summary>  
        /// Configures the model that was discovered by convention from the entity types  
        /// exposed in <see cref="DbSet{TEntity}"/> properties on your derived context.  
        /// </summary>  
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ignore the Scopes property when creating the model  
            modelBuilder.Entity<ApiKey>().Ignore(e => e.Scopes);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>  
        /// Gets or sets the DbSet for API keys.  
        /// </summary>  
        public DbSet<ApiKey> ApiKeys { get; set; }

        /// <summary>  
        /// Configures the database (and other options) to be used for this context.  
        /// </summary>  
        /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>  
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use SQLite database if no configuration is provided  
                optionsBuilder.UseSqlite("Data Source=Data\\apikeys.db");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
