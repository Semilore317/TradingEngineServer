
A pet project built from scratch on **.NET 10** using C# to understand the fundamentals of quantitative development (quant-dev) workflows, order matching logic, and electronic trading architectures.

---

## Repository Architecture

The solution is divided into distinct modules to separate concerns:

```text
TradingEngineServer/ (Solution Root)
├── Logging/              # Custom logging library (capabilites for db logging scaffolded but to be fully implemented later)
├── Instruments/          # Financial instrument reference data (Security)
├── Orders/               # Domain models: order types, limit levels, comparers
├── OrderBook/            # Order book engine: interfaces, matching book, spread
├── Tests/                # xUnit unit test suite (Fluent Assertions)
└── TradingEngineServer/  # Main application host & DI shell
```

### 1. `TradingEngineServer` (Main Host)
*   **Purpose:** Application entry point and hosting lifecycle manager.
*   Uses Microsoft Generic Host, Options pattern (`appsettings.json`), and Dependency Injection.

### 2. `Logging`
*   **Purpose:** Asynchronous text logger that offloads file writes from hot execution threads.
*   Uses a thread-safe `BufferBlock` queue and a background writer task.
*   Drains all queued logs cleanly on process disposal/cancellation.

### 3. `Instruments`
*   **Purpose:** Reference data library for tradable financial instruments.
*   Defines `Security` — a simple, immutable model carrying a `SecurityId` and ticker `Symbol`.
*   Kept separate from execution logic so instrument metadata never bleeds into the matching path.

### 4. `Orders` (Domain Models)
*   **Purpose:** Core domain models for orders, limit levels, and sorting rules.
*   **Immutability:** Identity fields are read-only via `IOrderCore` / `OrderCore` — prevents state corruption during matching.
*   **Memory Efficiency:** `OrderbookEntry` inherits directly from `Order` to act as a doubly-linked list node, eliminating redundant allocations.
*   **Cents-Based Pricing:** All prices are `long` (cents/ticks) to avoid floating-point rounding errors. Quantities are `uint`.
*   **Strict Sorting:** `BidLimitComparer` (descending) and `AskLimitComparer` (ascending) with null guards for safe use inside `SortedSet<T>`.

### 5. `OrderBook` (Order Book Engine)
*   **Purpose:** Maintains the live bid/ask book, enforces price-time priority, and exposes a tiered interface hierarchy.
*   **Interface Hierarchy:** `IReadonlyOrderBook` ← `IOrderEntryOrderBook` ← `IRetrievalOrderBook` ← `IMatchingOrderBook` — each layer restricts mutation access to the appropriate consumer.
*   **Data Structures:** `SortedSet<Limit>` for O(log n) price level insertion; `Dictionary<long, OrderbookEntry>` for O(1) order lookup.
*   **Memory Safety:** Cancelling the last order on a price level automatically removes the empty `Limit` from the sorted set, preventing unbounded memory growth.

### 6. `Tests`
*   **Purpose:** xUnit unit test suite using Fluent Assertions.
*   **Coverage:** Comparer ordering, limit level aggregation, order quantity mutations, modify/cancel mappings, linked-list pointer integrity, and full order book lifecycle (add → modify → cancel).

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10.0 |
| Language | C# 13 |
| Testing | xUnit + Fluent Assertions |
| Hosting | Microsoft Generic Host |

---

## Roadmap

- [x] Domain models (Orders, Limit Levels, Comparers)
- [x] Order Book structural layer (interfaces, sorted sets, linked lists)
- [x] Unit test suite
- [ ] Matching engine execution logic (price-time priority fills)
- [ ] REST gateway (`POST /orders`, `DELETE /orders/{id}`, `GET /book`)
- [ ] WebSocket/SignalR layer (live book + trade event broadcast)
- [ ] Browser dashboard (order entry form + live book table)
- [ ] Cloud deployment (backend on Render, frontend on Vercel)

---

## How to Run

Ensure you have the **.NET 10 SDK** installed.

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run --project TradingEngineServer/TradingEngineServer.csproj
```

### Test
```bash
dotnet test
```

### View Logs
Logs are written to date-stamped folders in the root directory. Stream them live in PowerShell:
```powershell
Get-Content -Path (Get-ChildItem -Path "2026-*/*.log" | Sort-Object LastWriteTime -Descending | Select-Object -First 1) -Wait
```
