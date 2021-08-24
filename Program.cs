using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using P1Dash;
using P1Dash.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// TODO
// Determine what source to use and add corresponding IDsmrProvider
// DsmrService gets IDsmrProvider from DI container
// No more temp serviceProvider for logging necessary

using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    var dsmrService = new DsmrService(serviceProvider.GetService<ILogger<DsmrService>>()!);
    builder.Services.AddSingleton<DsmrService>(dsmrService);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
