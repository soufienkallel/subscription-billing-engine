# Architecture & Design

## Domain

Customers subscribe to plans and get billed on a recurring cycle. They can change plans mid-cycle (requiring proration), consume metered usage add-ons, and — when a payment fails — enter a retry/dunning process that ends in either recovery or involuntary cancellation.

This single bounded context is deliberately scoped tightly: **Billing**. It does not try to also model a full CRM, a support-ticket system, or a marketing/growth platform. Those would be separate bounded contexts in a real company and are explicitly out of scope here.

## Core entities

`Customer`, `Plan`, `Subscription`, `Invoice`, `LineItem`, `PaymentAttempt`, `DunningCampaign`, `UsageRecord`, `Coupon`, `AuditLogEntry`.

## Aggregate boundaries (deliberate decision, revisited in Phase 6)

- **`Subscription`** is an aggregate root owning its own lifecycle state (`Trialing → Active → PastDue → Canceled`) and plan-change history.
- **`Invoice`** is a *separate* aggregate root, referencing its `Subscription` by ID rather than by object reference. Reasoning: once an invoice is finalized, it must not be mutated by later subscription changes — keeping them separate aggregates prevents one bloated object that both a live plan-change request and a nightly billing run are trying to lock at once.
- **`DunningCampaign`** is owned by `Invoice` (a failed invoice starts exactly one active campaign at a time).

## Why the DDD concepts exist here (not just theory)

| Concept | Problem it solves in this codebase |
|---|---|
| **Value Object** (`Money`, `BillingPeriod`, `ProrationCredit`, `TaxRate`) | Billing math is where silent bugs cost real money. A `Money` type that always carries currency and enforces correct rounding, and a `BillingPeriod` that can't represent an inverted date range, make entire bug classes unrepresentable. |
| **Entity** (`Invoice`, `PaymentAttempt`) | Has identity and a lifecycle tracked precisely — the same invoice persists through draft → finalized → paid, and every transition must be attributable. |
| **Aggregate Root** | Enforces "exactly one active plan at a time" and "an invoice's line items must sum to its total" atomically. |
| **Domain Service** (`ProrationCalculator`, `TaxCalculationService`) | Proration spans two plans plus a time boundary — doesn't belong to either `Plan` or `Subscription` alone. |
| **Domain Event** (`InvoicePaid`, `PaymentFailed`, `SubscriptionCanceled`) | Decouples "the payment succeeded" from its side effects (receipt, MRR reporting update, closing the dunning campaign). |
| **Repository** | Keeps the domain persistence-ignorant — proration and dunning logic are unit tested with zero database involvement. |
| **Specification** (`IsEligibleForRetrySpec`, `CouponApplicabilitySpec`) | Dunning-retry eligibility and coupon-stacking rules are composable, independently testable rules. |

## Pattern map (used only where they solve a real problem — not forced)

- **Strategy** — proration calculation per plan type; dunning retry-interval policy.
- **State** — `SubscriptionStatus` and `DunningCampaignStatus` transitions.
- **Decorator** — coupon/discount application over base pricing.
- **Chain of Responsibility** — dunning retry schedule (attempt 1 → 2 → 3 → give up).
- **Adapter** — isolating the Stripe SDK and FX-rate API client behind our own interfaces.
- **Factory** — building an `Invoice` with line items from a `Subscription` + `BillingPeriod`.
- **Unit of Work** — committing invoice + line items + domain event atomically.

## Deliberate non-defaults (and why)

- **No MediatR** — it moved to a commercial license in 2025. We hand-roll a thin `ICommandHandler<TCommand,TResult>` / `IQueryHandler<TQuery,TResult>` dispatcher instead. This is a feature, not a compromise: writing it ourselves is a better CQRS lesson than importing it.
- **No FluentAssertions** for the same reason (commercial since v8) — using **Shouldly** instead.
- **No .NET Aspire for local orchestration** (chosen deliberately) — plain Docker Compose instead, for portability of the skill outside the .NET ecosystem.
- **No real tax API** — genuinely free ones are rare. A currency-exchange-rate API stands in as the second real external integration; a simplified jurisdiction-rate-table service covers tax logic as an in-house domain service.

## Two concurrency problems (by design, not accident)

1. A nightly billing run and a customer's mid-cycle plan change racing on the same `Subscription` — resolved with EF Core optimistic concurrency (rowversion).
2. Two payment webhooks arriving out of order for the same `Invoice`/`PaymentAttempt` — resolved with idempotency keys, not "last write wins."

## Two state machines

- `SubscriptionStatus`: `Trialing → Active → PastDue → Canceled`
- `DunningCampaignStatus`: `Active → Recovered` / `Active → Exhausted → SubscriptionCanceled`

## Phase roadmap

See GitHub [milestones](../../milestones) for the authoritative, evolving breakdown. High-level shape:

1. Project setup & architecture
2. Basic domain model (Value Objects, Entities)
3. CRUD foundations
4. Auth & authorization
5. Complex business rules — proration engine (TDD)
6. DDD deepening — aggregates, domain events, repository/UoW
7. State machines — subscription & dunning lifecycles
8. External integrations — Stripe, FX-rate API
9. Background processing — Hangfire billing run & dunning scheduler
10. Caching & concurrency — Redis, `HybridCache`, the two concurrency problems
11. Testing/TDD consolidation — full pyramid
12. Docker
13. CI/CD
14. Production hardening — logging, OpenTelemetry, reporting, audit trail
15. Frontend — React customer portal + admin dashboard

Issues within each milestone are intentionally created close to when we reach them, not all upfront — requirements sharpen as earlier phases teach us things.
