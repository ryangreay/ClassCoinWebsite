using CoinSite.Data;
using CoinSite.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using GoogleCaptchaComponent;
using GoogleCaptchaComponent.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<CoinInfoService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddGoogleCaptcha(configuration =>
{
    configuration.ServerSideValidationRequired = false;
    //configuration.SiteKey = "6LfsLKcpAAAAAMiRgBWqEiNjdGSbVh65FDBaL6BL"; //localhost debug key
    configuration.SiteKey = "6LcnmqYpAAAAAM8XLC2fxjNkvbRR4n_SbUZ_UV0H"; //classcoin.org key
    configuration.CaptchaVersion = CaptchaConfiguration.Version.V2; // V3 is also the option now
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
