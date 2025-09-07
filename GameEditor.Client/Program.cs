using Blazor.IndexedDB;
using GameEditor.Client;
using GameEditor.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

//builder.Services.AddScoped<ThemeService>();

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();
builder.Services.AddScoped<IGamePackService, GamePackService>();


var host = builder.Build();

var gamePackService = host.Services.GetRequiredService<IGamePackService>();
await gamePackService.InitializeAsync();


await host.RunAsync();