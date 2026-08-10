using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzBiPortalCRM.Components;
using OzBiPortalCRM.Data;
using OzBiPortalCRM.Services;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var prodJsonPath = Path.Combine(builder.Environment.ContentRootPath, "appsettings.Production.json");
builder.Configuration.AddJsonFile(prodJsonPath, optional: true, reloadOnChange: true);

var localJsonPath = Path.Combine(builder.Environment.ContentRootPath, "appsettings.Local.json");
builder.Configuration.AddJsonFile(localJsonPath, optional: true, reloadOnChange: true);

// Configure Forwarded Headers for Cloud Run SSL Proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Configure Data Protection Key Persistence for Cloud Run instances
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "app", "keys");
Directory.CreateDirectory(keysFolder);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("OzBiAuditPortal");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "OzBiAuditPortal.Auth";
        options.LoginPath = "/login";
        options.LogoutPath = "/api/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// Configure MariaDB / MySQL ReadOnly DbContext Factory with verified DB 'ozbiappc_app' and GuidFormat=None
var ozBiConnStr = builder.Configuration.GetConnectionString("OzBiDatabase");
var serverVersion = new MariaDbServerVersion(new Version(10, 11, 8));

builder.Services.AddDbContextFactory<OzBiDbContext>(options =>
{
    options.UseMySql(ozBiConnStr, serverVersion, mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(2);
        mysqlOptions.CommandTimeout(15);
    });
});

// Configure SQLite DbContext Factory for Local App Database
var appConnStr = builder.Configuration.GetConnectionString("AppDatabase");
var absoluteDbPath = Path.Combine(builder.Environment.ContentRootPath, "app", "ozbi_audit.db");
Directory.CreateDirectory(Path.GetDirectoryName(absoluteDbPath)!);
appConnStr = $"Data Source={absoluteDbPath}";

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlite(appConnStr);
});

// Register Memory Cache & Multi-ERP Business Services
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ISlackNotificationService, SlackNotificationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOzBiAuditService, OzBiAuditService>();
builder.Services.AddScoped<ITenantSchemaProvider, TenantSchemaProvider>();
builder.Services.AddScoped<IErpAuditEngine, ErpAuditEngine>();
builder.Services.AddScoped<IMikroAuditEngine, ErpAuditEngine>();
// OzBiLoginMonitorService is handled by GCP Cloud Function - disabled in Web App startup to keep container lightweight
// builder.Services.AddSingleton<OzBiLoginMonitorService>();
// builder.Services.AddSingleton<IOzBiLoginMonitorService>(sp => sp.GetRequiredService<OzBiLoginMonitorService>());
// builder.Services.AddHostedService(sp => sp.GetRequiredService<OzBiLoginMonitorService>());

var app = builder.Build();

app.UseForwardedHeaders();

// Seed Default User in background without blocking port binding startup
_ = Task.Run(async () =>
{
    try
    {
        using var scope = app.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        await userService.SeedDefaultUserAsync();
    }
    catch { }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Native Cookie Auth Endpoints
app.MapPost("/api/auth/login", async (HttpContext httpContext, IUserService userService, ISlackNotificationService slackService, [FromForm] string email, [FromForm] string password) =>
{
    var user = await userService.AuthenticateAsync(email, password);
    if (user == null)
    {
        return Results.Redirect("/login?error=1");
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties
    {
        IsPersistent = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
    };

    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

    return Results.Redirect("/");
}).DisableAntiforgery();


app.MapGet("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
