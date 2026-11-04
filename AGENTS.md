# Agent instructions

Same rules as `CLAUDE.md`; the source of truth is `docs/`:
- `docs/architecture.md`
- `docs/coding-standards.md`
- `docs/testing-guidelines.md`
- `docs/review-checklist.md`

Commands: `dotnet build -warnaserror`, `dotnet test`, `dotnet format --verify-no-changes`.
Never commit, push, merge or approve PRs. Never read `.env*` or `*.local.json` files.
