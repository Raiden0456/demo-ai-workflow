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
    public void Pay_moves_issued_to_paid()
    {
        var invoice = Draft();
        invoice.Issue(Now);

        invoice.Pay(Now);

        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(Now, invoice.PaidAt);
    }

    [Fact]
    public void Pay_with_local_offset_stores_utc()
    {
        var invoice = Draft();
        invoice.Issue(Now);

        invoice.Pay(Now.ToOffset(TimeSpan.FromHours(2)));

        Assert.Equal(TimeSpan.Zero, invoice.PaidAt!.Value.Offset);
        Assert.Equal(Now, invoice.PaidAt);
    }

    [Fact]
    public void Pay_from_draft_throws()
    {
        var invoice = Draft();

        Assert.Throws<InvalidInvoiceTransitionException>(() => invoice.Pay(Now));
    }

    [Fact]
    public void Pay_twice_throws()
    {
        var invoice = Draft();
        invoice.Issue(Now);
        invoice.Pay(Now);

        Assert.Throws<InvalidInvoiceTransitionException>(() => invoice.Pay(Now));
    }

    [Fact]
    public void Pay_from_void_throws()
    {
        var invoice = Draft();
        invoice.Issue(Now);
        invoice.Void();

        Assert.Throws<InvalidInvoiceTransitionException>(() => invoice.Pay(Now));
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
