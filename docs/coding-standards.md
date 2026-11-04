# Coding standards

- .NET 10, C# latest, nullable enabled, warnings are errors in CI.
- Minimal APIs. One `Map<Feature>()` extension per feature folder, request/response records in `<Feature>Contracts.cs`.
- Every endpoint group calls `RequireAuthorization()`. Mutating endpoints require an explicit policy.
- Tenant is always taken from `user.TenantId()`. Never add store methods that read across tenants.
- State changes go through domain methods on the entity, not setters from endpoints.
- Validation errors → `Results.ValidationProblem`, missing or foreign entity → `404`, invalid state transition → `409`.
- Inject `TimeProvider`; never call `DateTime.Now` / `DateTimeOffset.UtcNow` directly. Timestamps are UTC `DateTimeOffset`.
- No comments that restate the code. A one-line comment is fine for a non-obvious why.
- Formatting is owned by `dotnet format`; do not argue with it in review.
