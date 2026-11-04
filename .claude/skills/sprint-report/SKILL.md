---
name: sprint-report
description: Builds a visual HTML sprint report from Jira for PMs and stand-ups. Use when asked for a sprint report, sprint status, sprint health or what is blocked in the current sprint.
allowed-tools: mcp__jira__get_active_sprint, mcp__jira__search_issues, mcp__jira__get_issue, Read, Write
---

Build a report for the active sprint from Jira. Read-only: never change issues.

## 1. Collect
- Fetch the active sprint and all its issues (Jira MCP or the Atlassian connector, whichever is available).
- For every Blocked or flagged issue fetch details and comments to find the blocker and the last comment.

## 2. Build the data JSON
Map Jira fields to exactly this shape. Do not invent values; use `null` when a field is missing.

```json
{
  "generatedAt": "YYYY-MM-DD (today)",
  "sprint": { "name": "", "goal": "", "startDate": "YYYY-MM-DD", "endDate": "YYYY-MM-DD", "committedPoints": 0 },
  "health": "on-track | at-risk | off-track",
  "headline": "One sentence for a PM: where the sprint stands and the single biggest risk.",
  "issues": [
    { "key": "", "summary": "", "status": "Done | In Review | In Progress | Blocked | To Do", "assignee": null,
      "storyPoints": 0, "updated": "YYYY-MM-DD", "flagged": false, "addedAfterSprintStart": false,
      "blockedBy": null, "lastComment": null }
  ],
  "risks": [ { "severity": "critical | serious | warning", "text": "Concrete risk with issue keys and who should act." } ]
}
```

- `health`: off-track if done + in-review points cannot realistically reach committed points in the days left or a goal-critical item is blocked; at-risk if anything is blocked, stale (no update for 5+ days) or scope was added mid-sprint; otherwise on-track.
- `risks`: at most 5, most severe first. Cover blocked high-point items, stale work, unassigned items, mid-sprint scope, points left vs days left.
- The template computes all numbers, stale flags and charts itself. Only provide raw issue data.

## 3. Render
Take `template.html` from this skill's folder and replace the `__REPORT_DATA__` placeholder with the JSON (escape every `</` as `<\/`). Change nothing else in the template.

- **Claude app (claude.ai / Desktop):** return the filled template as a single HTML artifact titled "<sprint name> report".
- **Claude Code:** write it to `reports/sprint-<sprint id or name>.html`, then reply with the file path, the headline and the risks as a short list.
