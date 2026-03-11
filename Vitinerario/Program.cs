using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
                new CultureInfo("en-US"),
                new CultureInfo("it-IT")
            };
    options.DefaultRequestCulture = new RequestCulture("it-IT", "it-IT");

    // You must explicitly state which cultures your application supports.
    // These are the cultures the app supports for formatting 
    // numbers, dates, etc.

    options.SupportedCultures = supportedCultures;

    // These are the cultures the app supports for UI strings, 
    // i.e. we have localized resources for.
    // Ordine dei provider (IMPORTANTE!)
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(), // ?culture=en-US
        new CookieRequestCultureProvider(),      // cookie salvato dopo scelta lingua
        new AcceptLanguageHeaderRequestCultureProvider() // fallback browser
    };
    options.SupportedUICultures = supportedCultures;
});
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor(); // Aggiunto per LanguageHelper

builder.Services.Configure<Vitinerario.Models.Settings.ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient<Vitinerario.Services.IAuthService, Vitinerario.Services.AuthService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "The userAgent field is required.");
});

builder.Services.AddHttpClient("VitinerarioApi", client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");
    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
    // AGGIUNGI QUESTA RIGA:
    client.DefaultRequestHeaders.Add("User-Agent", "VitinerarioWebClient/1.0");
    client.DefaultRequestHeaders.Add("User-Agent", "The userAgent field is required.");
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

// Configura la localizzazione all'avvio
var supportedCultures = new[] { "it-IT" ,"en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Aggiungi un provider personalizzato per leggere il TUO cookie "UserLanguage"
localizationOptions.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
{
    var cookie = context.Request.Cookies["UserLanguage"];
    var culture = (cookie == "2") ? "it-IT" : "en";
    return Task.FromResult(new ProviderCultureResult(culture));
}));

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
