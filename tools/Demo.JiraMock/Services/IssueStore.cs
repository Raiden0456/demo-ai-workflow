using Demo.JiraMock.Models;

namespace Demo.JiraMock.Services;

public sealed class IssueStore(SprintData data, TimeProvider clock)
{
    private readonly Lock _lock = new();
    private readonly List<Issue> _issues = [.. data.Issues];

    public Sprint Sprint => data.Sprint;

    public IReadOnlyList<Issue> All()
    {
        lock (_lock)
        {
            return [.. _issues];
        }
    }

    public Issue? Find(string key)
    {
        lock (_lock)
        {
            return _issues.FirstOrDefault(i => i.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }
    }

    public Issue Create(string summary, string description, string type, int storyPoints, IReadOnlyList<string> labels)
    {
        lock (_lock)
        {
            var next = _issues.Max(i => int.Parse(i.Key.Split('-')[1])) + 1;
            var issue = new Issue(
                Key: $"DEMO-{next}",
                Summary: summary,
                Type: type,
                Status: "To Do",
                Assignee: null,
                StoryPoints: storyPoints,
                Updated: DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime),
                Description: description,
                Comments: [],
                Labels: labels,
                InActiveSprint: false);
            _issues.Add(issue);
            return issue;
        }
    }
}
