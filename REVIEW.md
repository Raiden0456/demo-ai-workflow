# Review checklist

Single source of review rules for every reviewer: `/review-pr` locally, Claude Review Action, GitHub Copilot code review (reads this file natively).

## Security
- [ ] New endpoints are inside a group with `RequireAuthorization()` and mutating ones have an explicit policy.
- [ ] Data access is tenant-scoped; no new store method reads by id alone.
- [ ] No secrets, connection strings or tokens in code, config or logs.
- [ ] Input is validated before it reaches the domain.

## Correctness
- [ ] State changes go through domain methods with transition guards.
- [ ] Error mapping follows the standards: 400 / 403 / 404 / 409.
- [ ] Time comes from `TimeProvider`, stored as UTC.

## Tests
- [ ] Each acceptance criterion of the linked ticket is covered, including negative cases.
- [ ] No test relies on execution order or shared mutable state.

## Scope
- [ ] The diff matches the ticket; unrelated changes are split out.
