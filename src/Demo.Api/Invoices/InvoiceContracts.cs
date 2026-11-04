namespace Demo.Api.Invoices;

public sealed record CreateInvoiceRequest(string Customer, decimal Amount, string Currency);

public sealed record InvoiceResponse(
    Guid Id,
    string Number,
    string Customer,
    decimal Amount,
    string Currency,
    InvoiceStatus Status,
    DateTimeOffset? IssuedAt)
{
    public static InvoiceResponse From(Invoice invoice) => new(
        invoice.Id,
        invoice.Number,
        invoice.Customer,
        invoice.Amount,
        invoice.Currency,
        invoice.Status,
        invoice.IssuedAt);
}
