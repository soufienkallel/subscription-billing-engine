# Subscription Billing Engine

A production-realistic SaaS subscription billing & dunning platform, built as a structured, incremental learning project for modern .NET (backend) and React/TypeScript (frontend).

This is **not** a tutorial clone. It's a from-scratch system that handles the kind of problems real billing platforms deal with: mid-cycle plan changes with proration, failed-payment retry/dunning workflows, metered usage billing, and the concurrency bugs that show up when a nightly billing run and a live user action touch the same data at once.

## Status

🚧 Early setup — see [open issues](../../issues) and [milestones](../../milestones) for current progress. Built one issue at a time, not generated in bulk.

## Stack

| Layer | Choice |
|---|---|
| Backend | C#, .NET 10, ASP.NET Core Minimal APIs |
| Database | PostgreSQL, EF Core 10 |
| Architecture | Clean Architecture + DDD, CQRS introduced progressively |
| Frontend | React 19 + TypeScript, Vite, TanStack Query |
| Testing | xUnit, Shouldly, NSubstitute, Testcontainers |
| Background jobs | Hangfire |
| Caching | Redis + `HybridCache` |
| Containerization | Docker Compose |
| CI/CD | GitHub Actions |
| Later phases | messaging, observability (Serilog + OpenTelemetry), .NET Aspire |

Full design rationale: [`ARCHITECTURE.md`](./ARCHITECTURE.md)
Git workflow & contribution process: [`WORKFLOW.md`](./WORKFLOW.md)

## Running locally

### Running Postgres locally

1. Copy `.env.example` to `.env` and set your own `POSTGRES_PASSWORD` (and adjust `POSTGRES_USER` / `POSTGRES_DB` if you want).
2. Start the database:
   ```bash
   docker compose up -d
   ```
3. Check it's healthy:
   ```bash
   docker compose ps
   ```
4. Connect with `psql` using the credentials from your `.env`:
   ```bash
   psql -h localhost -U <POSTGRES_USER> -d <POSTGRES_DB>
   ```

The rest of the local setup (running the API, etc.) is populated as Phase 1 (project setup) completes.
