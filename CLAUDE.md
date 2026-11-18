# Demo AI Workflow

Multi-tenant invoicing API used to demo a team-wide Claude Code setup. Stack: .NET 10, ASP.NET Core minimal APIs, xUnit, GitHub Actions.

## Commands

- Build: `dotnet build -warnaserror`
- Test: `dotnet test`
- Format check: `dotnet format --verify-no-changes`
- Run API: `dotnet run --project src/Demo.Api` then `curl -H "X-Demo-User: alice" localhost:5141/invoices`

## Rules

@docs/architecture.md
@docs/coding-standards.md
@docs/testing-guidelines.md

## Workflow

- PRDs live in Notion (`notion` MCP server). `/draft-prd` writes a draft, `/prd-to-tickets` turns an approved PRD into Jira tickets.
- Tickets live in Jira (`jira` MCP server). Start work with `/implement-ticket <KEY>`.
- Creating Jira tickets or Notion pages always needs explicit approval of the exact content first.
- In this repo Jira and Notion mean only the demo `jira` and `notion` MCP servers. Never use any other Atlassian or Notion connector; if those servers are missing, stop and say so.
- Before a PR run `/review-pr`. Every AI reviewer in CI (Claude or Copilot) uses the same `REVIEW.md`.
- Work on feature branches. Push, PR creation and merge ask for confirmation; GitHub still requires a human approval and green CI to merge.
- Pushing to master, force pushes, `--admin` merges, PR approvals and secrets are blocked by `.claude/settings.json` and `.claude/hooks/`.

## Advisor

`.claude/agents/advisor.md` is a read-only second opinion from a fresh context on the strongest model. Ask it before changing `InvoiceStore`, a shared contract, an auth policy or tenant handling, and after two failed attempts at the same problem. The brief states the decision, what must not change and which files to read, not the answer you prefer. Check its references before acting on them.

## How to work with the developer

- In a multi-step workflow, start each reply with the stage and what you are waiting for: `Stage 2/5 Plan: waiting for your go`.
- Ask only when the answer changes behaviour. One question at a time, with options and your recommendation.
- Summaries list files changed, one line each, in the order to read them.

## Team setup

- Shared, committed: `CLAUDE.md`, `.claude/settings.json`, `.claude/skills/`, `.claude/hooks/`, `.claude/agents/`, `.mcp.json`.
- Personal, not committed: `CLAUDE.local.md`, `.claude/settings.local.json`.
