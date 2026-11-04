using ModelContextProtocol;
using System.ComponentModel;
using Demo.JiraMock.Models;
using Demo.JiraMock.Services;
using ModelContextProtocol.Server;

namespace Demo.JiraMock.Tools;

[McpServerToolType]
public sealed class JiraTools(IssueStore store)
{
    private static readonly HashSet<string> IssueTypes = new(StringComparer.OrdinalIgnoreCase) { "Story", "Task", "Bug" };
    private static readonly HashSet<int> Points = [1, 2, 3, 5, 8];

    [McpServerTool(Name = "get_active_sprint", ReadOnly = true)]
    [Description("Returns the active sprint with its goal, dates, committed story points and a one-line summary of every issue in it.")]
    public object GetActiveSprint() => new
    {
        store.Sprint,
        Issues = store.All()
            .Where(i => i.InActiveSprint)
            .Select(i => new { i.Key, i.Summary, i.Type, i.Status, i.Assignee, i.StoryPoints, i.Flagged, i.AddedAfterSprintStart }),
    };

    [McpServerTool(Name = "search_issues", ReadOnly = true)]
    [Description("Searches issues. All filters are optional and case-insensitive. Backlog issues are included only when includeBacklog is true or a label is given.")]
    public IEnumerable<Issue> SearchIssues(
        [Description("Status: To Do, In Progress, In Review, Blocked, Done")] string? status = null,
        [Description("Assignee display name")] string? assignee = null,
        [Description("Exact label, e.g. prd:prd-csv-export")] string? label = null,
        [Description("Include issues that are not in the active sprint")] bool includeBacklog = false) =>
        store.All().Where(i =>
            (i.InActiveSprint || includeBacklog || label is not null) &&
            (status is null || i.Status.Equals(status, StringComparison.OrdinalIgnoreCase)) &&
            (assignee is null || string.Equals(i.Assignee, assignee, StringComparison.OrdinalIgnoreCase)) &&
            (label is null || (i.Labels?.Contains(label, StringComparer.OrdinalIgnoreCase) ?? false)));

    [McpServerTool(Name = "get_issue", ReadOnly = true)]
    [Description("Returns a single issue with description, acceptance criteria and comments.")]
    public Issue GetIssue([Description("Issue key, e.g. DEMO-112")] string key) =>
        store.Find(key) ?? throw new McpException($"Issue {key} not found");

    [McpServerTool(Name = "create_issue", ReadOnly = false, Destructive = false, Idempotent = false)]
    [Description("Creates a new issue in the backlog and returns it with its key. Only call after the user approved the exact ticket.")]
    public Issue CreateIssue(
        [Description("Short imperative title")] string summary,
        [Description("Context, acceptance criteria as a list and a link to the source document")] string description,
        [Description("Story, Task or Bug")] string type = "Story",
        [Description("Estimate: 1, 2, 3, 5 or 8")] int storyPoints = 3,
        [Description("Labels, e.g. prd:prd-csv-export")] string[]? labels = null)
    {
        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new McpException("Summary is required");
        }

        if (!IssueTypes.Contains(type))
        {
            throw new McpException($"Type must be one of {string.Join(", ", IssueTypes)}");
        }

        if (!Points.Contains(storyPoints))
        {
            throw new McpException($"Story points must be one of {string.Join(", ", Points)}");
        }

        return store.Create(summary.Trim(), description, type, storyPoints, labels ?? []);
    }
}
