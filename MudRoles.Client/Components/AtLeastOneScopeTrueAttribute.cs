using System.ComponentModel.DataAnnotations;
using static MudRoles.Client.Pages.ApiMgt;

namespace MudRoles.Client.Components
{
    /// <summary>
    /// Validation attribute to ensure that at least one scope is selected.
    /// </summary>
    public class AtLeastOneScopeTrueAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates whether at least one scope is selected.
        /// </summary>
        /// <param name="value">The value to validate, expected to be a list of <see cref="Scope"/>.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>A <see cref="ValidationResult"/> indicating whether validation succeeded or failed.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is List<Scope> scopes && scopes.Any(s => s.IsChecked))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("At least one scope must be selected.");
        }
    }
}