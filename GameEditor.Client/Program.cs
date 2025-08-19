using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameEditor.Client;
using GameEditor.Client.Services;
using Radzen;
using Blazor.IndexedDB;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();
builder.Services.AddScoped<IGamePackService, GamePackService>();

var host = builder.Build();

var gamePackService = host.Services.GetRequiredService<IGamePackService>();
await gamePackService.InitializeAsync();

await host.RunAsync();
