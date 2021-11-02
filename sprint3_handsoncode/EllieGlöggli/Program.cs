using EllieGlöggli;
using EllieGlöggli.Common;
using EllieGlöggli.Common.Admin;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddEllieAdmin();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAlarmService, AlarmService>();

var configuration = new ConfigurationBuilder()
    //.AddJsonFile($"appsettings.json", optional: false)
    //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json".Replace("..", "."), optional: false)
    .Build();

builder.Services.AddSingleton(sp =>
{
    var url = configuration?.GetValue<string>("ServerBaseAddress") ?? "https://localhost:7287";
    var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
    var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions { HttpHandler = httpHandler });
    var client = new EllieGloeggeli.EllieGloeggeliClient(channel);
    return client;
});


await builder.Build().RunAsync();
