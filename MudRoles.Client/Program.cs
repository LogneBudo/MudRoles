using MudRoles.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudRoles.Client.Infrastructure.Settings;
using MudBlazor.Extensions;
using MudRoles.Client.Components;
using FluentValidation;
using MudExtensions.Services;
using MudBlazor;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServicesWithExtensions(options =>
{
    options.PopoverOptions.ThrowOnDuplicateProvider = false;
    options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    options.SnackbarConfiguration.PreventDuplicates = true;
});

builder.Services.AddMudExtensions();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddSingleton<ConsentService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<KeyInputModelValidator>();
await builder.Build().RunAsync();
