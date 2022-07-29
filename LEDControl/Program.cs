using System.Net;
using System.Net.Sockets;
using FastEndpoints;
using FastEndpoints.Swagger;
using LEDControl;

var builder = WebApplication.CreateBuilder(args);

// ConfigureServices
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddHostedService<LEDBackgroundService>();
builder.Services.AddSingleton<ILEDState, LEDState>();
builder.Services.AddScoped<ILEDEffects, LEDEffects>();

builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();

var app = builder.Build();

// Configure
var localIP = LocalIPAddress();
app.Urls.Add("https://localhost:5001");
app.Urls.Add($"https://{localIP}:5001");

app.UseAuthorization();
app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUi3(c => c.ConfigureDefaults());
app.Run();

string LocalIPAddress()
{
    string localIP;
    using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
    
    socket.Connect("8.8.8.8", 65530);
    var endPoint = socket.LocalEndPoint as IPEndPoint;
    localIP = endPoint?.Address.ToString() ?? "127.0.0.1";

    return localIP;
}