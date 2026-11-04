namespace Demo.Api.Invoices;

public sealed class InvoiceStore
{
    private readonly Lock _lock = new();
    private readonly Dictionary<Guid, Invoice> _invoices = [];

    public IReadOnlyList<Invoice> List(string tenantId)
    {
        lock (_lock)
        {
            return _invoices.Values.Where(i => i.TenantId == tenantId).OrderBy(i => i.Number).ToList();
        }
    }

    public Invoice? Find(string tenantId, Guid id)
    {
        lock (_lock)
        {
            return _invoices.TryGetValue(id, out var invoice) && invoice.TenantId == tenantId ? invoice : null;
        }
    }

    public Invoice? GetById(Guid id)
    {
        lock (_lock)
        {
            return _invoices.GetValueOrDefault(id);
        }
    }

    public Invoice Create(string tenantId, string customer, decimal amount, string currency)
    {
        lock (_lock)
        {
            var sequence = _invoices.Values.Count(i => i.TenantId == tenantId) + 1;
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Number = $"{tenantId.ToUpperInvariant()}-{sequence:D4}",
                Customer = customer,
                Amount = amount,
                Currency = currency,
            };
            _invoices.Add(invoice.Id, invoice);
            return invoice;
        }
    }
}
