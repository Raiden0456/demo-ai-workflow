---
name: review-pr
description: First-pass code review of the current branch or a PR against the team checklist. Use when asked to review changes, a branch or a PR before a human reviewer.
argument-hint: "[PR number | base branch, default master]"
allowed-tools: Read, Grep, Glob, Bash(git diff:*), Bash(git log:*), Bash(gh pr view:*), Bash(gh pr diff:*), mcp__jira__get_issue
---

Review target: $ARGUMENTS (empty → current branch vs `master`; a number → that PR via `gh pr diff`).

1. Get the diff and the list of changed files. Read each changed file in full, not just the hunks.
2. Check every item of `REVIEW.md`. Pay most attention to tenant isolation and authorization.
3. If the change references a ticket, fetch it with `mcp__jira__get_issue` and verify the acceptance criteria are met and tested.

Output a table: severity (Critical / High / Medium / Low), file:line, problem, concrete fix.
Then one line verdict: "Ready for human review" or "Changes requested".

Only report real problems with a concrete failure scenario. No style nitpicks the formatter already enforces.
Never approve, merge or push. The final decision belongs to a human reviewer.
