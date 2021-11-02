using EllieGlöggli;
using EllieGlöggli.Common;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAlarmService, AlarmService>();

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var url = configuration.GetValue<string>("ServerBaseAddress") ?? "https://localhost:7287";
    var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
    var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions { HttpHandler = httpHandler });
    var client = new EllieGloeggeli.EllieGloeggeliClient(channel);
    return client;
});


await builder.Build().RunAsync();
