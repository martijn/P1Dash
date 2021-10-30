using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using P1Dash;
using P1Dash.Dsmr;
using P1Dash.Models;
using P1Dash.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

Directory.CreateDirectory("./Storage");
builder.Configuration.AddJsonFile("./Storage/settings.json", true);

builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("AppOptions"));

var appOptions = builder.Configuration.GetSection("AppOptions").Get<AppOptions>() ?? new AppOptions();

#if DEBUG
builder.Services.AddSingleton<IDsmrProvider, DummyDsmrProvider>();
#else
switch (appOptions.Provider)
{
    case AppOptions.ProviderType.Serial:
        builder.Services.AddSingleton<IDsmrProvider, SerialDsmrProvider>();
        break;
    case AppOptions.ProviderType.Tcp:
        builder.Services.AddSingleton<IDsmrProvider, TcpDsmrProvider>();
        break;
}

;
#endif

builder.Services.AddSingleton<DsmrService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Prometheus-compatible metrics endpoint
app.MapGet("/metrics",
    (DsmrService dsmrService) =>
        Util.FormatMetric("p1dash_electricity_balance_kwh", "gauge",
            dsmrService.History.Last().ElectricityBalance,
            "Electricity consumed and/or delivered") +
        Util.FormatMetric("p1dash_gas_delivered_m3", "counter",
            dsmrService.History.Last().GasDelivered,
            "Gas delivered to client in m3 (5 minute interval)")
        );

app.Run();
