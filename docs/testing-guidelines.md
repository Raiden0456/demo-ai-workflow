# Testing guidelines

- Every acceptance criterion has at least one test. Negative cases are mandatory: wrong role (`403`), foreign tenant (`404`), invalid transition (`409`), invalid input (`400`).
- Endpoint behaviour → `InvoiceEndpointsTests` style: real pipeline via `WebApplicationFactory<Program>`, users via the `X-Demo-User` header.
- Domain rules → plain unit tests on the entity, fixed `DateTimeOffset`, no web host.
- Tests create their own data; never depend on seed records except for read-only list checks.
- Test names: `Subject_condition_expected_result`.
- `dotnet test` must pass locally before a PR; CI runs build with `-warnaserror`, `dotnet format --verify-no-changes` and tests.
