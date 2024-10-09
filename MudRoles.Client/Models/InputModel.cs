using System.ComponentModel.DataAnnotations;

namespace MudRoles.Client.Models
{
    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";

        [Display(Name = "Your choice of Avatar")]
        public string AvatarChoice { get; set; } = "";

        [Display(Name = "Set the color of the Avatar")]
        public string AvatarColor { get; set; } = "";

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "";

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        [Display(Name = "Title")]
        public string Title { get; set; } = "";

        [Display(Name = "Theme Preference")]
        public bool IsDarkTheme { get; set; } = false;

        [Display(Name = "Avatar")]
        public string AvatarId { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public byte[]? AvatarPhoto { get; set; }
    }
}
