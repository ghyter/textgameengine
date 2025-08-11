using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameEditor.Client;


// var builder = WebAssemblyHostBuilder.CreateDefault(args);
// builder.RootComponents.Add<App>("#app");
// builder.RootComponents.Add<HeadOutlet>("head::after"); 

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// await builder.Build().RunAsync();


using MudBlazor.Services;
using GameEditor.Client.Services;
using Blazor.IndexedDB;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped<ThemeService>();

builder.Services.AddMudServices();
builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();
builder.Services.AddScoped<IGamePackService, GamePackService>();


var host = builder.Build();

var gamePackService = host.Services.GetRequiredService<IGamePackService>();
await gamePackService.InitializeAsync();


await host.RunAsync();