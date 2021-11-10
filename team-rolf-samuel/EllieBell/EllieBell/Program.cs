using EllieBell;
using EllieBell.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new GloeggliService());
builder.Services.AddSingleton(sp =>
{
    var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
    var channel = GrpcChannel.ForAddress("https://localhost:7287", new GrpcChannelOptions { HttpHandler = httpHandler });
    var client = new EllieGloeggeli.EllieGloeggeliClient(channel);
    return client;
});

await builder.Build().RunAsync();
