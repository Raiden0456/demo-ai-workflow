---
name: implement-ticket
description: Implements a Jira ticket end to end following the repo standards. Use when asked to implement, build or pick up a ticket by key, e.g. DEMO-112.
argument-hint: <TICKET-KEY>
disable-model-invocation: true
effort: high
---

Implement Jira ticket $ARGUMENTS.

Start every reply with the stage and what you are waiting for, e.g. `Stage 2/5 Plan: waiting for your go`. Keep the to-do list in step: one item per stage.

1. **Intake.** Fetch the ticket with `mcp__jira__get_issue`. List the acceptance criteria with IDs (AC1, AC2, ...). If there are none, stop and ask.
2. **Plan.** Read the code it touches and `docs/architecture.md`. Post the plan: files to change, tests to add, one question at a time with the answer you recommend. If the plan changes `InvoiceStore`, a shared contract, an auth policy or tenant handling, ask the `advisor` agent first and show its recommendation. Wait for approval.
3. **Build.** Implement following `docs/coding-standards.md`. Reuse existing patterns before adding new ones.
4. **Test.** One test per acceptance criterion per `docs/testing-guidelines.md`, negative cases included. Run `dotnet build -warnaserror` and `dotnet test`. Fix until green.
5. **Hand back.** Acceptance criteria with ✅/❌, files changed in the order to read them, test results.

Do not commit, push or open a PR unless the developer asks for it after reviewing the diff.
