using Demo.Api.Invoices;

namespace Demo.Api.Tests;

public sealed class InvoiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 4, 10, 0, 0, TimeSpan.Zero);

    private static Invoice Draft() => new()
    {
        Id = Guid.NewGuid(),
        TenantId = "acme",
        Number = "ACME-0001",
        Customer = "Fabrikam",
        Amount = 100m,
        Currency = "EUR",
    };

    [Fact]
    public void Issue_moves_draft_to_issued()
    {
        var invoice = Draft();

        invoice.Issue(Now);

        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(Now, invoice.IssuedAt);
    }

    [Fact]
    public void Void_is_allowed_for_issued_invoice()
    {
        var invoice = Draft();
        invoice.Issue(Now);

        invoice.Void();

        Assert.Equal(InvoiceStatus.Void, invoice.Status);
    }

    [Fact]
    public void Void_twice_throws()
    {
        var invoice = Draft();
        invoice.Void();

        Assert.Throws<InvalidInvoiceTransitionException>(invoice.Void);
    }
}
