
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor(); // Aggiunto per LanguageHelper

builder.Services.Configure<Vitinerario.Models.Settings.ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient<Vitinerario.Services.IAuthService, Vitinerario.Services.AuthService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "VitinerarioWebClient/1.0");
});

builder.Services.AddHttpClient("VitinerarioApi", client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");
    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
    client.DefaultRequestHeaders.Add("User-Agent", "VitinerarioWebClient/1.0");
});

builder.Services.AddScoped<Vitinerario.Services.IApiService, Vitinerario.Services.ApiService>();
builder.Services.AddScoped<Vitinerario.Services.IAuthService, Vitinerario.Services.AuthService>();

builder.Services.Configure<Vitinerario.Models.Settings.MailSettings>(
    builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<Vitinerario.Services.IEmailService, Vitinerario.Services.EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// =========================================================================
// CONFIGURAZIONE LOCALIZZAZIONE PULITA (Forzata su IT di default)
// =========================================================================
var supportedCultures = new[] { "it-IT", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0]) // Imposta l'italiano come default assoluto
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// IL TRUCCO È QUI: Svuotiamo i provider predefiniti (che leggono la lingua del browser)
localizationOptions.RequestCultureProviders.Clear();

// Aggiungiamo SOLO il nostro provider personalizzato per leggere il TUO cookie "UserLanguage"
localizationOptions.RequestCultureProviders.Add(new CustomRequestCultureProvider(context =>
{
    var cookie = context.Request.Cookies["UserLanguage"];

    // SE IL COOKIE NON ESISTE (Nuovo utente): restituisci null. 
    // Poiché abbiamo svuotato gli altri provider, scatterà matematicamente il DefaultCulture (it-IT)!
    if (string.IsNullOrEmpty(cookie))
    {
        return Task.FromResult<ProviderCultureResult>(null);
    }

    // Se il cookie esiste, applica la tua logica: 2 = italiano, altrimenti inglese
    var culture = (cookie == "2") ? "it-IT" : "en";
    return Task.FromResult(new ProviderCultureResult(culture));
}));

app.UseRequestLocalization(localizationOptions);
// =========================================================================

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();