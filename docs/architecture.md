# Architecture

## Solution
- `src/Demo.Api` — ASP.NET Core minimal API (.NET 10), feature folders: `Auth/`, `Invoices/`.
- `tests/Demo.Api.Tests` — xUnit, endpoint tests via `WebApplicationFactory<Program>` plus domain unit tests.
- `tools/Demo.JiraMock` — stdio MCP server with mock sprint data and in-memory ticket creation, registered as `jira` in `.mcp.json`.
- `tools/Demo.NotionMock` — stdio MCP server with mock PRDs and notes, registered as `notion` in `.mcp.json`. Created pages live in memory until the server restarts.

## Multi-tenancy
Every invoice belongs to a tenant. The tenant comes only from the authenticated user (`ClaimsPrincipal.TenantId()`), never from the route, body or query.
`InvoiceStore` exposes tenant-scoped methods only. An invoice of another tenant is indistinguishable from a missing one: respond `404`, not `403`.

## Auth
Demo scheme: `X-Demo-User` header → users in `Auth/DemoUsers.cs`. Production target is Entra ID with app roles; the policies stay the same.

| Policy | Roles |
|---|---|
| authenticated | any |
| `CanBill` | billing, admin |
| `CanAdmin` | admin |

## Invoice lifecycle
`Draft → Issued → Paid`, `Draft|Issued → Void`. Transitions live on `Invoice` and throw `InvalidInvoiceTransitionException`; endpoints map it to `409`.

## Storage
In-memory, single instance. Swappable for EF Core + Azure SQL behind the same store methods.
