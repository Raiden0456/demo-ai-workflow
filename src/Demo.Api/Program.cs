using System.Text.Json.Serialization;
using Demo.Api.Auth;
using Demo.Api.Invoices;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(DemoHeaderAuthHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, DemoHeaderAuthHandler>(DemoHeaderAuthHandler.SchemeName, null);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.CanBill, p => p.RequireRole(Roles.Billing, Roles.Admin))
    .AddPolicy(Policies.CanAdmin, p => p.RequireRole(Roles.Admin));

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<InvoiceStore>();

var app = builder.Build();

InvoiceSeed.Apply(app.Services.GetRequiredService<InvoiceStore>(), app.Services.GetRequiredService<TimeProvider>());

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapInvoices();

app.Run();
