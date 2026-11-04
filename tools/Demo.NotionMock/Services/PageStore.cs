using ModelContextProtocol;
using System.Text.RegularExpressions;
using Demo.NotionMock.Models;

namespace Demo.NotionMock.Services;

public sealed partial class PageStore(IReadOnlyList<Page> seed, TimeProvider clock)
{
    private const string Editor = "Claude";
    private readonly Lock _lock = new();
    private readonly List<Page> _pages = [.. seed];

    public IReadOnlyList<Page> Search(string? query)
    {
        lock (_lock)
        {
            return _pages
                .Where(p => string.IsNullOrWhiteSpace(query)
                    || p.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || p.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public Page? Find(string idOrUrl)
    {
        var id = idOrUrl.Split('/').Last();
        lock (_lock)
        {
            return _pages.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }

    public Page Create(string title, string content, string status)
    {
        lock (_lock)
        {
            var baseId = Slug().Replace(title.ToLowerInvariant(), "-").Trim('-');
            var id = baseId;
            for (var n = 2; _pages.Any(p => p.Id == id); n++)
            {
                id = $"{baseId}-{n}";
            }

            var page = new Page(id, title, status, Editor, Today(), content);
            _pages.Add(page);
            return page;
        }
    }

    public Page Append(string idOrUrl, string content)
    {
        lock (_lock)
        {
            var page = Find(idOrUrl) ?? throw new McpException($"Page {idOrUrl} not found");
            var updated = page with
            {
                Content = $"{page.Content.TrimEnd()}\n\n{content.Trim()}\n",
                LastEditedBy = Editor,
                LastEdited = Today(),
            };
            _pages[_pages.IndexOf(page)] = updated;
            return updated;
        }
    }

    private DateOnly Today() => DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex Slug();
}
