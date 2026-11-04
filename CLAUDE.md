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
- Before a PR run `/review-pr`; CI runs the same checklist (`docs/review-checklist.md`).
- Never commit, push, merge or approve PRs. Humans own those steps; settings and hooks enforce it.
- Never read `.env*` or `*.local.json` files.

## Team setup
- Shared, committed: `CLAUDE.md`, `.claude/settings.json`, `.claude/skills/`, `.claude/hooks/`, `.mcp.json`.
- Personal, not committed: `CLAUDE.local.md`, `.claude/settings.local.json`.
