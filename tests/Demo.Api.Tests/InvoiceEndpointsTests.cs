using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Demo.Api.Auth;
using Demo.Api.Invoices;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Demo.Api.Tests;

public sealed class InvoiceEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private HttpClient ClientFor(string? user)
    {
        var client = factory.CreateClient();
        if (user is not null)
        {
            client.DefaultRequestHeaders.Add(DemoHeaderAuthHandler.Header, user);
        }

        return client;
    }

    private async Task<InvoiceResponse> CreateDraft(string user = "alice")
    {
        var response = await ClientFor(user).PostAsJsonAsync("/invoices", new CreateInvoiceRequest("Fabrikam", 100m, "EUR"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<InvoiceResponse>(Json))!;
    }

    [Fact]
    public async Task Anonymous_request_is_rejected()
    {
        var response = await ClientFor(null).GetAsync("/invoices");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Unknown_user_is_rejected()
    {
        var response = await ClientFor("mallory").GetAsync("/invoices");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task List_returns_only_own_tenant_invoices()
    {
        var invoices = await ClientFor("carol").GetFromJsonAsync<List<InvoiceResponse>>("/invoices", Json);

        Assert.NotNull(invoices);
        Assert.NotEmpty(invoices);
        Assert.All(invoices, i => Assert.StartsWith("GLOBEX-", i.Number));
    }

    [Fact]
    public async Task Invoice_from_another_tenant_is_not_found()
    {
        var invoice = await CreateDraft("alice");

        var response = await ClientFor("carol").GetAsync($"/invoices/{invoice.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Viewer_cannot_create_invoice()
    {
        var response = await ClientFor("bob").PostAsJsonAsync("/invoices", new CreateInvoiceRequest("Fabrikam", 100m, "EUR"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("", 100, "EUR")]
    [InlineData("Fabrikam", 0, "EUR")]
    [InlineData("Fabrikam", 100, "BTC")]
    public async Task Invalid_create_request_returns_validation_problem(string customer, decimal amount, string currency)
    {
        var response = await ClientFor("alice").PostAsJsonAsync("/invoices", new CreateInvoiceRequest(customer, amount, currency));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Billing_user_can_issue_draft()
    {
        var invoice = await CreateDraft();

        var response = await ClientFor("alice").PostAsync($"/invoices/{invoice.Id}/issue", null);
        var issued = await response.Content.ReadFromJsonAsync<InvoiceResponse>(Json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(InvoiceStatus.Issued, issued!.Status);
        Assert.NotNull(issued.IssuedAt);
    }

    [Fact]
    public async Task Issuing_twice_returns_conflict()
    {
        var invoice = await CreateDraft();
        await ClientFor("alice").PostAsync($"/invoices/{invoice.Id}/issue", null);

        var response = await ClientFor("alice").PostAsync($"/invoices/{invoice.Id}/issue", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Billing_user_cannot_void()
    {
        var invoice = await CreateDraft();

        var response = await ClientFor("alice").PostAsync($"/invoices/{invoice.Id}/void", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_cannot_void_invoice_of_another_tenant()
    {
        var invoice = await CreateDraft("alice");

        var response = await ClientFor("carol").PostAsync($"/invoices/{invoice.Id}/void", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
