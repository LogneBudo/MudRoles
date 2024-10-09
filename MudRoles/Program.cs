using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudRoles;
using MudRoles.Client.Components;
using MudRoles.Client.Infrastructure.Settings;
using MudRoles.Client.Pages;
using MudRoles.Components;
using MudRoles.Components.Account;
using MudRoles.Data;
using MudRoles.Infrastructure.Api;
using MudExtensions.Services;
using MudRoles.Infrastructure.MiddleWare;
using MudRoles.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.OpenApi.Models;

/// <summary>
/// Configures the web application builder, adds user secrets, and registers services.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CookiePolicyOptions>(opt =>
{
    opt.CheckConsentNeeded = context => true;
    opt.MinimumSameSitePolicy = SameSiteMode.None;
});
// Add user secrets to the configuration
builder.Configuration.AddUserSecrets<Program>();

// Retrieve the API key from the configuration
//var movieApiKey = builder.Configuration["ApiKey"];

// Register an HttpClient with a base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:7140") });

builder.Services.AddHttpContextAccessor();
/// <summary>
/// Add MudBlazor services with extensions and options.
/// </summary>
builder.Services.AddMudServicesWithExtensions(options =>
{
    options.PopoverOptions.ThrowOnDuplicateProvider = false;
});

// Add MudExtensions service
builder.Services.AddMudExtensions();

// Register the ThemeService
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddSingleton<ConsentService>();

/// <summary>
/// Add Razor components and interactive components for server and WebAssembly.
/// </summary>
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add cascading authentication state
builder.Services.AddCascadingAuthenticationState();

// Register IdentityUserAccessor and IdentityRedirectManager
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

// Register the custom authentication state provider
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();


/// <summary>
/// Configure authentication and identity services.
/// </summary>
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

/// <summary>
/// Configure database contexts and identity services.
/// </summary>
// Get the connection string from the configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure the main application database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add a filter to show detailed database errors during development
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure identity services with custom options
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Configure the API key database context
builder.Services.AddDbContext<ApiKeyDbContext>(options =>
           options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

// Add MVC controllers
builder.Services.AddControllers();

// Register a no-op email sender for identity services
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Add FluentValidation validators from the specified assembly
builder.Services.AddValidatorsFromAssemblyContaining<KeyInputModelValidator>();

// Add in-memory caching services
//builder.Services.AddMemoryCache();

// Register the API key service
//builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "My API",
        Description = "ASP.NET Core Web API"
    });
    // Custom schema ID generation to avoid conflicts
    c.CustomSchemaIds(type => type.FullName);
    // Add security definition for Identity
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Enter your authentication token in the text input below.\r\n\r\nExample: \"12345abcdef\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

/// <summary>
/// Configure the middleware for serving static files and antiforgery protection.
/// </summary>
app.UseStaticFiles();
app.UseAntiforgery();
app.UseCookiePolicy();

/// <summary>
/// Map the controllers and configure Razor components with interactive render modes.
/// </summary>
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MudRoles.Client._Imports).Assembly);

/// <summary>
/// Use MudExtensions middleware.
/// </summary>
app.UseMudExtensions();

/// <summary>
/// Add additional endpoints required by the Identity /Account Razor components.
/// </summary>
app.MapAdditionalIdentityEndpoints();
/// <summary>
/// Initialize scope fetching for action descriptors.
/// </summary>
var actionDescriptorCollectionProvider = app.Services.GetRequiredService<IActionDescriptorCollectionProvider>();
ScopeFetcher.Initialize(actionDescriptorCollectionProvider);

/// <summary>
/// Use custom middleware for API key validation and rate limiting.
/// </summary>
//app.UseMiddleware<ApiKeyMiddleware>();
//app.UseMiddleware<RateLimitingMiddleware>();

// Seed the database with default roles and users 
// Uncomment this code to seed the database with default roles and users

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//        await SeedData.Initialize(services, userManager);
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while seeding the database.");
//    }
//}
app.Run();
