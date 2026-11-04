---
name: implement-ticket
description: Implements a Jira ticket end to end following the repo standards. Use when asked to implement, build or pick up a ticket by key, e.g. DEMO-112.
argument-hint: <TICKET-KEY>
disable-model-invocation: true
---

Implement Jira ticket $ARGUMENTS.

1. Fetch the ticket with `mcp__jira__get_issue`. Extract acceptance criteria as a checklist. If there are none, stop and ask.
2. Read the code it touches and `docs/architecture.md`. Post a short plan: files to change, tests to add, open questions. Wait for approval.
3. Implement following `docs/coding-standards.md`. Reuse existing patterns before adding new ones.
4. Add tests per `docs/testing-guidelines.md`: one test per acceptance criterion, including the negative cases.
5. Run `dotnet build -warnaserror` and `dotnet test`. Fix until green.
6. Report: acceptance criteria with ✅/❌, files changed, test results.

Do not commit, push or open a PR unless the developer asks for it after reviewing the diff.
