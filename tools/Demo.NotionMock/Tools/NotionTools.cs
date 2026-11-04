using ModelContextProtocol;
using System.ComponentModel;
using Demo.NotionMock.Models;
using Demo.NotionMock.Services;
using ModelContextProtocol.Server;

namespace Demo.NotionMock.Tools;

[McpServerToolType]
public sealed class NotionTools(PageStore store)
{
    [McpServerTool(Name = "search_pages", ReadOnly = true)]
    [Description("Searches pages by title or text. Returns id, title, status and URL. Empty query lists all pages.")]
    public IEnumerable<PageSummary> SearchPages([Description("Words from the title or body")] string? query = null) =>
        store.Search(query).Select(p => new PageSummary(p.Id, p.Title, p.Status, p.LastEditedBy, p.LastEdited, p.Url));

    [McpServerTool(Name = "get_page", ReadOnly = true)]
    [Description("Returns a page with its full Markdown content.")]
    public Page GetPage([Description("Page id or URL")] string id) =>
        store.Find(id) ?? throw new McpException($"Page {id} not found");

    [McpServerTool(Name = "create_page", ReadOnly = false, Destructive = false, Idempotent = false)]
    [Description("Creates a page with Markdown content and returns it with its URL. Only call after the user approved the draft.")]
    public Page CreatePage(
        [Description("Page title, e.g. PRD: Invoice CSV export")] string title,
        [Description("Full page body in Markdown")] string content,
        [Description("Draft, In review or Ready for tickets")] string status = "Draft")
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new McpException("Title is required");
        }

        return store.Create(title.Trim(), content, status);
    }

    [McpServerTool(Name = "append_to_page", ReadOnly = false, Destructive = false, Idempotent = false)]
    [Description("Appends Markdown to the end of an existing page and returns the updated page.")]
    public Page AppendToPage(
        [Description("Page id or URL")] string id,
        [Description("Markdown to append")] string content) =>
        store.Append(id, content);
}
