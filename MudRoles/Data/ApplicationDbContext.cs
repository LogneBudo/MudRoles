
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MudRoles.Data
{
    /// <summary>  
    /// Represents the application's database context, inheriting from IdentityDbContext.  
    /// </summary>  
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>  
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.  
        /// </summary>  
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>  
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
