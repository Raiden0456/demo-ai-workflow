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
- Tickets live in Jira (`jira` MCP server). Start work with `/implement-ticket <KEY>`.
- Before a PR run `/review-pr`. Every AI reviewer in CI (Claude or Copilot) uses the same `REVIEW.md`.
- Work on feature branches. Push, PR creation and merge ask for confirmation; GitHub still requires a human approval and green CI to merge.
- Pushing to master, force pushes, `--admin` merges, PR approvals and secrets are blocked by `.claude/settings.json` and `.claude/hooks/`.

## Team setup
- Shared, committed: `CLAUDE.md`, `.claude/settings.json`, `.claude/skills/`, `.claude/hooks/`, `.mcp.json`.
- Personal, not committed: `CLAUDE.local.md`, `.claude/settings.local.json`.
