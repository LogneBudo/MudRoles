using FluentValidation;
namespace MudRoles.Client.Components
{
    public class KeyInputModelValidator : AbstractValidator<KeyInputModel>
    {
        public KeyInputModelValidator()
        {
            RuleFor(x => x.ApiKeyName)
                .NotEmpty().WithMessage("API Key Name is required.")
                .MinimumLength(5).WithMessage("API Key Name must be at least 5 characters long.");

            RuleFor(x => x.Scopes).Must(scopes => scopes != null && scopes.Any(s => s.IsChecked))
            .WithMessage("At least one endpoint is required.");
        }

    }
}
