# AspireTodoApp

A production-ready Todo application built with **.NET Aspire**, **ASP.NET Core Minimal APIs**, and **React + Vite**. This project demonstrates how to build, orchestrate, and observe a modern distributed application using the full Aspire stack.

## What This Project Is About

Most tutorials show you how to build a simple API. This project goes further  it shows how to wire up a real distributed application where services discover each other automatically, health is monitored out of the box, telemetry flows without extra configuration, and everything runs with a single command.

The goal is **production-ready from day one**: structured observability, integration tests against a real in-memory database, central package management, and a containerizable frontend served alongside the API.

## Architecture

```
┌─────────────────────────────────────────┐
│           .NET Aspire AppHost           │
│         (orchestrates everything)       │
└────────────────┬────────────────────────┘
                 │
    ┌────────────┴────────────┐
    │                         │
    ▼                         ▼
┌──────────────┐     ┌─────────────────┐
│  React + Vite│────▶│ ASP.NET Core    │
│   Frontend   │     │ Minimal API     │
│  (port 5173) │     │  (port 5000)    │
└──────────────┘     └────────┬────────┘
                              │
                     ┌────────▼────────┐
                     │  PostgreSQL +   │
                     │  EF Core 10     │
                     └─────────────────┘
```

The React frontend proxies all `/api` calls to the backend. Aspire injects the correct URL automatically — no hardcoded ports anywhere.

## Tech Stack

| Layer | Technology |
|---|---|
| Orchestration | .NET Aspire 13 |
| Backend | ASP.NET Core 10 Minimal APIs |
| Database | PostgreSQL + Entity Framework Core 10 |
| Auth | ASP.NET Core Identity |
| Frontend | React 19 + Vite 8 + TypeScript |
| Observability | OpenTelemetry (traces, metrics, logs) |
| Testing | xUnit + WebApplicationFactory (integration tests) |
| Package Management | Central Package Management (`Directory.Packages.props`) |

## Project Structure

```
AspireTodoApp/
├── AspireTodoApp.AppHost/        # Aspire orchestrator — defines what runs and how
├── AspireTodoApp.Server/         # Minimal API backend — all endpoints live here
├── AspireTodoApp.Server.Tests/   # Integration tests — hits real in-memory database
├── TodoApp.ServiceDefaults/      # Shared observability and resilience config
├── frontend/                     # React + Vite frontend (TypeScript)
│   └── src/
│       ├── App.tsx
│       └── main.tsx
├── Directory.Build.props         # Shared build properties for all .NET projects
├── Directory.Packages.props      # Central NuGet package version management
├── global.json                   # Pins the .NET SDK version
└── .devcontainer/                # One-click dev environment (VS Code / Codespaces)
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+](https://nodejs.org)
- [Aspire CLI](https://aspire.dev/get-started/install-cli/)

### Run the App

```bash
aspire run
```

That single command:
1. Builds the .NET backend
2. Starts the Vite dev server for React
3. Opens the Aspire dashboard (logs, traces, health) in your browser
4. Wires up service discovery between frontend and API automatically

### Run Tests

```bash
dotnet test
```

Integration tests use `WebApplicationFactory` with an in-memory database — no mocking, no fakes, real HTTP calls against a real in-memory server.

## Why Minimal APIs

ASP.NET Core Minimal APIs keep endpoint definitions lean and explicit. No controllers, no attribute routing sprawl — just functions mapped to routes:

```csharp
var api = app.MapGroup("/api");

api.MapGet("/todos", async (TodoDbContext db) =>
    await db.Todos.ToListAsync());

api.MapPost("/todos", async (Todo todo, TodoDbContext db) => {
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});
```

## Why Aspire

Aspire solves the hardest part of distributed development: running everything together locally the same way it runs in production. Service discovery, health checks, structured logs and traces — all configured once in `AppHost.cs`, available everywhere.

## Dev Container

Open this project in VS Code and click **"Reopen in Container"** — Docker will set up .NET 10, Node 22, and the Aspire CLI automatically. No manual installation required.

Works with **GitHub Codespaces** too — open the repo and start coding instantly from your browser.

## Inspired By

[davidfowl/TodoApp](https://github.com/davidfowl/TodoApp) — the reference architecture for production-ready ASP.NET Core applications.
