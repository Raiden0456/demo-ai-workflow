using System.Text.Json;
using Demo.NotionMock.Models;
using Demo.NotionMock.Services;
using Demo.NotionMock.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var dataPath = Path.Combine(AppContext.BaseDirectory, "data", "pages.json");
var pages = JsonSerializer.Deserialize<List<Page>>(File.ReadAllText(dataPath), JsonSerializerOptions.Web)
    ?? throw new InvalidOperationException($"Cannot load {dataPath}");

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Services.AddSingleton<IReadOnlyList<Page>>(pages);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PageStore>();
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<NotionTools>();

await builder.Build().RunAsync();
