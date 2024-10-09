using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MudRoles.Client.Components
{
    public class RoleInputModel
    {
        [Required]
        public string RoleName { get; set; } = "";

        public bool IsUserRole { get; set; } // Indicates if this is the mandatory "User" role

        public List<RoleItem> Roles { get; set; } = new List<RoleItem>();
    }

    public class RoleItem
    {
        public string Id { get; set; } = "";
        public string NormalizedName { get; set; } = "";
        [Required]
        [MinLength(4, ErrorMessage = "For readability purposes don't create a role with less than 4 letters!")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Role name must contain only alphabetic characters from the regular Latin script.")]
        public string RoleName { get; set; } = "";
        public bool IsAssigned { get; set; }
    }

}
