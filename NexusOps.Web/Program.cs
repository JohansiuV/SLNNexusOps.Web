using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.WebEncoders;
using NexusOps.Web.Middleware;
using NexusOps.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Todas las páginas requieren sesión, salvo el login y las páginas de error.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Error");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");
});

// Autenticación por cookies. El token JWT de la API se guarda dentro de la cookie (cifrada).
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".NexusOps.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.SlidingExpiration = false;
    });

// Permite tildes y ñ sin escapar en el HTML (sigue escapando <, >, & y comillas).
builder.Services.Configure<WebEncoderOptions>(o =>
    o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement,
        UnicodeRanges.LatinExtendedA, UnicodeRanges.GeneralPunctuation, UnicodeRanges.Arrows, UnicodeRanges.Dingbats));

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// URL base de la API (NexusOps.Api). Si no existe en appsettings.json se usa https://localhost:7150/
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7150/";

builder.Services.AddTransient<TokenHandler>();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<TokenHandler>()
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();

    // Solo en desarrollo: acepta el certificado HTTPS de localhost de la API.
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }

    return handler;
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<CatalogosService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Cultura invariante: los números decimales llegan con punto (<input type="number">) y las fechas en formato ISO.
var cultura = CultureInfo.InvariantCulture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new List<CultureInfo> { cultura },
    SupportedUICultures = new List<CultureInfo> { cultura }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Si el token de la API expiró o fue rechazado, se cierra la sesión y se vuelve al login.
app.UseMiddleware<SesionExpiradaMiddleware>();

app.MapRazorPages();

app.Run();
