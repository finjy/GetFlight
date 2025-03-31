using GetFlight.WebClient;
using GetFlight.WebClient.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7079") });

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FlightService>();

await builder.Build().RunAsync();
