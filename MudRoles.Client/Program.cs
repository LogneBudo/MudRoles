using MudRoles.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudRoles.Client.Infrastructure.Settings;
using MudBlazor.Extensions;
using MudRoles.Client.Components;
using FluentValidation;
using MudExtensions.Services;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
var movieApiKey = builder.Configuration["ApiKey"];
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServicesWithExtensions(options =>
{
    options.PopoverOptions.ThrowOnDuplicateProvider = false;
});
builder.Services.AddMudExtensions();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddValidatorsFromAssemblyContaining<KeyInputModelValidator>();
await builder.Build().RunAsync();
