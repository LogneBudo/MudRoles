using Microsoft.AspNetCore.Identity;

namespace MudRoles.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Title { get; set; }
    public bool IsDarkTheme { get; set; }
    public string? AvatarId { get; set; }
}

