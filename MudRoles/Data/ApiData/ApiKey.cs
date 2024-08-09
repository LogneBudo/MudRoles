
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MudRoles.Data.ApiData
{
    /// <summary>  
    /// Represents an API key with associated metadata and scopes.  
    /// </summary>  
    public class ApiKey
    {
        /// <summary>  
        /// Gets or sets the unique identifier for the API key.  
        /// </summary>  
        public int Id { get; set; }

        /// <summary>  
        /// Gets or sets the name of the API key set.  
        /// </summary>  
        [Required(ErrorMessage = "You must set a name for your key set.")]
        [MinLength(5, ErrorMessage = "Name is too short.")]
        [StringLength(250, ErrorMessage = "Name is too long.")]
        public string? Name { get; set; }

        /// <summary>  
        /// Gets or sets the prefix of the API key.  
        /// </summary>  
        [Required]
        public string? KeyPrefix { get; set; }

        /// <summary>  
        /// Gets or sets the API key value.  
        /// </summary>  
        [Required]
        public string? Key { get; set; }

        /// <summary>  
        /// Gets or sets the creation date of the API key.  
        /// </summary>  
        public DateTime CreationDate { get; set; }

        /// <summary>  
        /// Gets or sets the expiration date of the API key.  
        /// </summary>  
        public DateTime ExpirationDate { get; set; }

        /// <summary>  
        /// Gets or sets the user ID associated with the API key.  
        /// </summary>  
        [Required]
        public string? UserId { get; set; }

        /// <summary>  
        /// Gets or sets the status of the API key.  
        /// </summary>  
        public KeyStatus Status { get; set; }

        /// <summary>  
        /// Gets or sets the JSON representation of the scopes associated with the API key.  
        /// </summary>  
        [Required]
        public string ScopesJson { get; set; } = string.Empty;

        /// <summary>  
        /// Gets or sets the list of scopes associated with the API key.  
        /// </summary>  
        public List<Scope> Scopes
        {
            get => JsonSerializer.Deserialize<List<Scope>>(ScopesJson) ?? new List<Scope>();
            set => ScopesJson = JsonSerializer.Serialize(value);
        }
    }
}
