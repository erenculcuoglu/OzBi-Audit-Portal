using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzBiPortalCRM.Components;
using OzBiPortalCRM.Data;
using OzBiPortalCRM.Services;

var builder = WebApplication.CreateBuilder(args);
var prodJsonPath = Path.Combine(builder.Environment.ContentRootPath, "appsettings.Production.json");
builder.Configuration.AddJsonFile(prodJsonPath, optional: true, reloadOnChange: true);

var localJsonPath = Path.Combine(builder.Environment.ContentRootPath, "appsettings.Local.json");
builder.Configuration.AddJsonFile(localJsonPath, optional: true, reloadOnChange: true);

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
builder.Services.AddSingleton<OzBiLoginMonitorService>();
builder.Services.AddSingleton<IOzBiLoginMonitorService>(sp => sp.GetRequiredService<OzBiLoginMonitorService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<OzBiLoginMonitorService>());

var app = builder.Build();

// Seed Default User (eren@ozbiapp.com.tr / 123456)
using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    await userService.SeedDefaultUserAsync();
}

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

    // Slack push bildirimini tetikle
    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
    if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "::1")
    {
        ipAddress = "127.0.0.1 (Yerel)";
    }
    var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
    await slackService.SendLoginNotificationAsync(user.FullName, user.Email, user.Role, ipAddress, userAgent);

    return Results.Redirect("/");
}).DisableAntiforgery();


app.MapGet("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapGet("/api/cron/check-logins", async (IOzBiLoginMonitorService monitor) =>
{
    await monitor.CheckForNewLoginsAsync();
    return Results.Ok(new { status = "success", message = "MariaDB login check triggered successfully." });
}).AllowAnonymous();

app.MapGet("/api/trigger-login-check", async (IOzBiLoginMonitorService monitor) =>
{
    try
    {
        await monitor.CheckForNewLoginsAsync();
        return Results.Ok(new { success = true, message = "Check executed cleanly" });
    }
    catch (Exception ex)
    {
        return Results.Json(new { success = false, error = ex.Message, stack = ex.StackTrace }, statusCode: 500);
    }
}).AllowAnonymous();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
