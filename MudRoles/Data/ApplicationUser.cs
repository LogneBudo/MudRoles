using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MudRoles.Client.Components;

namespace MudRoles.Data;

/// <summary>  
/// Represents an application user with additional profile data.  
/// </summary>  
public class ApplicationUser : IdentityUser
{
    /// <summary>  
    /// Gets or sets the first name of the user.  
    /// </summary>  
    public required string FirstName { get; set; }

    /// <summary>  
    /// Gets or sets the last name of the user.  
    /// </summary>  
    public required string LastName { get; set; }

    /// <summary>  
    /// Gets or sets the title of the user.  
    /// </summary>  
    public string? Title { get; set; }

    /// <summary>  
    /// Gets or sets a value indicating whether the user prefers a dark theme.  
    /// </summary>  
    public bool IsDarkTheme { get; set; }

    /// <summary>  
    /// Gets or sets the avatar ID of the user.  
    /// </summary>  
    public string? AvatarId { get; set; }

    /// <summary>  
    /// Gets or sets the avatar choice of the user.  
    /// </summary>  
    public string? AvatarChoice { get; set; }

    /// <summary>  
    /// Gets or sets the uploaded avatar image of the user.  
    /// </summary>  
    public byte[]? UploadedAvatar { get; set; }

    [NotMapped]
    public List<RoleItem> Roles { get; set; } = [];
}
