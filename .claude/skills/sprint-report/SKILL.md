---
name: sprint-report
description: Builds a sprint status report from Jira for the PM and stand-up. Use when asked for a sprint report, sprint status or burn-down summary.
disable-model-invocation: true
allowed-tools: mcp__jira__get_active_sprint, mcp__jira__search_issues, mcp__jira__get_issue
---

Build a report for the active sprint using only the `jira` MCP server. Never change issues.

1. Fetch the active sprint and all its issues.
2. For every Blocked or flagged issue, fetch details and comments to find the blocker.
3. Use the sprint end date and today's date to compute days left.

Output exactly these sections, in Markdown, short and scannable:

## <Sprint name> — <days left> days left
Goal: <goal>
Points: <done> / <committed> done, <added mid-sprint> added after start

## Completed
Key, summary, assignee.

## In progress / In review
Key, summary, assignee, days since last update. Mark items not updated for 5+ days as stale.

## Blocked
Key, summary, assignee, blocked by, last comment.

## Risks
Max 5 bullets: blocked high-point items, stale work, unassigned items, scope added mid-sprint, points left vs days left. Say plainly if the sprint goal is at risk.

Do not invent data. If a field is missing, say so.
