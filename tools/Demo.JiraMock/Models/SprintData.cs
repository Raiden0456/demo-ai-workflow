namespace Demo.JiraMock.Models;

public sealed record SprintData(Sprint Sprint, IReadOnlyList<Issue> Issues);

public sealed record Sprint(int Id, string Name, string Goal, DateOnly StartDate, DateOnly EndDate, int CommittedPoints);

public sealed record Issue(
    string Key,
    string Summary,
    string Type,
    string Status,
    string? Assignee,
    int StoryPoints,
    DateOnly Updated,
    string Description,
    IReadOnlyList<Comment> Comments,
    bool Flagged = false,
    string? BlockedBy = null,
    bool AddedAfterSprintStart = false,
    IReadOnlyList<string>? Labels = null,
    bool InActiveSprint = true);

public sealed record Comment(string Author, DateOnly Created, string Body);
