namespace Demo.Api.Invoices;

public enum InvoiceStatus
{
    Draft,
    Issued,
    Paid,
    Void,
}

public sealed class Invoice
{
    public required Guid Id { get; init; }
    public required string TenantId { get; init; }
    public required string Number { get; init; }
    public required string Customer { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;
    public DateTimeOffset? IssuedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    public void Issue(DateTimeOffset now)
    {
        if (Status != InvoiceStatus.Draft)
        {
            throw new InvalidInvoiceTransitionException(Status, InvoiceStatus.Issued);
        }

        Status = InvoiceStatus.Issued;
        IssuedAt = now;
    }

    public void Pay(DateTimeOffset now)
    {
        if (Status != InvoiceStatus.Issued)
        {
            throw new InvalidInvoiceTransitionException(Status, InvoiceStatus.Paid);
        }

        Status = InvoiceStatus.Paid;
        PaidAt = now.ToUniversalTime();
    }

    public void Void()
    {
        if (Status is InvoiceStatus.Paid or InvoiceStatus.Void)
        {
            throw new InvalidInvoiceTransitionException(Status, InvoiceStatus.Void);
        }

        Status = InvoiceStatus.Void;
    }
}

public sealed class InvalidInvoiceTransitionException(InvoiceStatus from, InvoiceStatus to)
    : InvalidOperationException($"Cannot move invoice from {from} to {to}");
