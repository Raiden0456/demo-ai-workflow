namespace Demo.NotionMock.Models;

public sealed record Page(
    string Id,
    string Title,
    string Status,
    string LastEditedBy,
    DateOnly LastEdited,
    string Content)
{
    public string Url => $"https://notion.so/demo/{Id}";
}

public sealed record PageSummary(string Id, string Title, string Status, string LastEditedBy, DateOnly LastEdited, string Url);
