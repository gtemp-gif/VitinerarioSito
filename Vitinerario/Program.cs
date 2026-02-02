using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Vitinerario.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IContentService, MockContentService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var app = builder.Build();

// Configure the HTTP request pipeline.
var supportedCultures = new[] { "it", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("it")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRequestLocalization(localizationOptions);

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{culture=it}/{controller=Home}/{action=Index}/{id?}");

app.Run();
