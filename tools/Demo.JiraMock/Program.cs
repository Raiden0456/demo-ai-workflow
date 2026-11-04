using System.Text.Json;
using Demo.JiraMock.Models;
using Demo.JiraMock.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var dataPath = Path.Combine(AppContext.BaseDirectory, "data", "sprint.json");
var data = JsonSerializer.Deserialize<SprintData>(File.ReadAllText(dataPath), JsonSerializerOptions.Web)
    ?? throw new InvalidOperationException($"Cannot load {dataPath}");

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Services.AddSingleton(data);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<JiraTools>();

await builder.Build().RunAsync();
