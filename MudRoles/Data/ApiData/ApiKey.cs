using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MudRoles.Data.ApiData
{
    public class ApiKey
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You must set a name for your key set.")]
        [MinLength(5, ErrorMessage = "Name is too short.")]
        [StringLength(250, ErrorMessage = "Name is too long.")]
        public string? Name { get; set; }

        [Required]
        public string? KeyPrefix { get; set; }
        [Required]
        public string? Key { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        [Required]
        public string? UserId { get; set; }

        public KeyStatus Status { get; set; }

        [Required]
        public string ScopesJson { get; set; } = string.Empty;

        public List<Scope> Scopes
        {
            get => JsonSerializer.Deserialize<List<Scope>>(ScopesJson) ?? [];
            set => ScopesJson = JsonSerializer.Serialize(value);
        }
    }
}