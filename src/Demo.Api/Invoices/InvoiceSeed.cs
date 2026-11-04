namespace Demo.Api.Invoices;

public static class InvoiceSeed
{
    public static void Apply(InvoiceStore store, TimeProvider clock)
    {
        store.Create("acme", "Northwind Traders", 1200m, "EUR");
        store.Create("acme", "Contoso Ltd", 860.50m, "EUR").Issue(clock.GetUtcNow());
        store.Create("globex", "Initech", 4300m, "USD").Issue(clock.GetUtcNow());
    }
}
