using System.Security.Claims;
using Demo.Api.Auth;

namespace Demo.Api.Invoices;

public static class InvoiceEndpoints
{
    private static readonly HashSet<string> SupportedCurrencies = ["EUR", "USD", "GBP"];

    public static IEndpointRouteBuilder MapInvoices(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/invoices").RequireAuthorization();

        group.MapGet("/", (ClaimsPrincipal user, InvoiceStore store) =>
            store.List(user.TenantId()).Select(InvoiceResponse.From));

        group.MapGet("/{id:guid}", (Guid id, ClaimsPrincipal user, InvoiceStore store) =>
            store.Find(user.TenantId(), id) is { } invoice
                ? Results.Ok(InvoiceResponse.From(invoice))
                : Results.NotFound());

        group.MapPost("/", (CreateInvoiceRequest request, ClaimsPrincipal user, InvoiceStore store) =>
        {
            var errors = Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var invoice = store.Create(user.TenantId(), request.Customer.Trim(), request.Amount, request.Currency);
            return Results.Created($"/invoices/{invoice.Id}", InvoiceResponse.From(invoice));
        }).RequireAuthorization(Policies.CanBill);

        group.MapPost("/{id:guid}/issue", (Guid id, ClaimsPrincipal user, InvoiceStore store, TimeProvider clock) =>
            Transition(store.Find(user.TenantId(), id), invoice => invoice.Issue(clock.GetUtcNow())))
            .RequireAuthorization(Policies.CanBill);

        group.MapPost("/{id:guid}/void", (Guid id, ClaimsPrincipal user, InvoiceStore store) =>
            Transition(store.Find(user.TenantId(), id), invoice => invoice.Void()))
            .RequireAuthorization(Policies.CanAdmin);

        return app;
    }

    private static IResult Transition(Invoice? invoice, Action<Invoice> apply)
    {
        if (invoice is null)
        {
            return Results.NotFound();
        }

        try
        {
            apply(invoice);
        }
        catch (InvalidInvoiceTransitionException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }

        return Results.Ok(InvoiceResponse.From(invoice));
    }

    private static Dictionary<string, string[]> Validate(CreateInvoiceRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Customer))
        {
            errors[nameof(request.Customer)] = ["Customer is required"];
        }

        if (request.Amount <= 0)
        {
            errors[nameof(request.Amount)] = ["Amount must be positive"];
        }

        if (!SupportedCurrencies.Contains(request.Currency))
        {
            errors[nameof(request.Currency)] = [$"Currency must be one of {string.Join(", ", SupportedCurrencies)}"];
        }

        return errors;
    }
}
