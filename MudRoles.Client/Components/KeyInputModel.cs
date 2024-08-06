using System.ComponentModel.DataAnnotations;
using static MudRoles.Client.Pages.ApiMgt;

namespace MudRoles.Client.Components
{
    public class KeyInputModel
    {
        [Required(ErrorMessage = "API Key Name is required.")]
        [MinLength(5, ErrorMessage = "API Key Name must be at least 5 characters long.")]
        public string? ApiKeyName { get; set; }
        [AtLeastOneScopeTrue(ErrorMessage = "At least one scope must be selected.")]
        public List<Scope> Scopes { get; set; } = new List<Scope>();
    }

}
