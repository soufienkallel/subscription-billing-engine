# Git Workflow

## Branches

- **`main`** — stable/production. Never committed to directly. Only updated via PR from `develop` at milestone boundaries.
- **`develop`** — integration branch. All feature branches merge here first.
- **`issue-<number>-<short-description>`** — one branch per GitHub issue (e.g. `issue-3-value-object-money`).

## Flow per issue

1. Branch from `develop`: `git checkout develop && git pull && git checkout -b issue-<n>-<slug>`
2. Small, meaningful commits as the issue progresses (see commit convention below).
3. Open a PR into `develop` when the issue's acceptance criteria are met.
4. Merge into `develop` (regular merge, not squash — keeps the incremental learning commits visible).
5. `develop` merges into `main` via PR at the end of a milestone (a working, demoable state), not after every issue.

## Commit convention

Conventional Commits, kept small and scoped to one idea:

```
feat(domain): add Money value object with currency-safe arithmetic
test(domain): cover Money arithmetic across mismatched currencies
docs(architecture): record aggregate boundary decision for Invoice
refactor(billing): extract proration calc into domain service
fix(concurrency): resolve lost-update on subscription plan change
```

Prefixes used: `feat`, `fix`, `test`, `docs`, `refactor`, `chore`, `ci`.

## Issue labels

- Which phase an issue belongs to is tracked via its **GitHub milestone** (Phase 1–15), not a separate label — avoids the two ever disagreeing.
- `concept:ddd`, `concept:testing`, `concept:concurrency`, `concept:ef-core`, `concept:auth`, `concept:cqrs`, `concept:background-jobs`, `concept:frontend` — primary concept(s) the issue teaches.
- `size:xs` (~1-2h), `size:s` (~3-5h), `size:m` (~6-10h), `size:l` (~10h+, should probably be split further).

## Pull request checklist

- [ ] Acceptance criteria from the linked issue are met
- [ ] Tests added/updated for the behavior changed
- [ ] No unrelated changes bundled in
- [ ] Commit messages follow the convention above
