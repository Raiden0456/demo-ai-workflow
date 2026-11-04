using System.ComponentModel;
using Demo.JiraMock.Models;
using ModelContextProtocol.Server;

namespace Demo.JiraMock.Tools;

[McpServerToolType]
public sealed class JiraTools(SprintData data)
{
    [McpServerTool(Name = "get_active_sprint", ReadOnly = true)]
    [Description("Returns the active sprint with its goal, dates, committed story points and a one-line summary of every issue.")]
    public object GetActiveSprint() => new
    {
        data.Sprint,
        Issues = data.Issues.Select(i => new { i.Key, i.Summary, i.Type, i.Status, i.Assignee, i.StoryPoints, i.Flagged, i.AddedAfterSprintStart }),
    };

    [McpServerTool(Name = "search_issues", ReadOnly = true)]
    [Description("Searches issues of the active sprint. All filters are optional and case-insensitive.")]
    public IEnumerable<Issue> SearchIssues(
        [Description("Status: To Do, In Progress, In Review, Blocked, Done")] string? status = null,
        [Description("Assignee display name")] string? assignee = null) =>
        data.Issues.Where(i =>
            (status is null || i.Status.Equals(status, StringComparison.OrdinalIgnoreCase)) &&
            (assignee is null || string.Equals(i.Assignee, assignee, StringComparison.OrdinalIgnoreCase)));

    [McpServerTool(Name = "get_issue", ReadOnly = true)]
    [Description("Returns a single issue with description, acceptance criteria and comments.")]
    public Issue GetIssue([Description("Issue key, e.g. DEMO-112")] string key) =>
        data.Issues.FirstOrDefault(i => i.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
        ?? throw new ArgumentException($"Issue {key} not found");
}
