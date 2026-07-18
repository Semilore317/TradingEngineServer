# TradingEngineServer

A limit order book and matching engine written from scratch in C# on **.NET 10**. FIFO and pro-rata matching, strict price-time priority, and an async logging pipeline. 
There are no exchange libraries: the book, the matching loop, and the allocation math are all custom.

---

## What is it?

An in-memory trading engine that maintains a live bid/ask book per instrument and matches incoming orders against it. I built it to work through the fundamentals of electronic trading infrastructure: order-book data structures, matching algorithms, and the low-latency-minded design choices that come with them.

Two matching algorithms are present today, selectable from config:

- **FIFO (price-time priority)**: the oldest resting order at a price level fills first.
- **Pro-rata**: incoming volume is split across resting orders relative to their size.

---

## Matching in action

**Pro-rata** allocates an incoming order across resting orders by *size*, not arrival time, the same way many futures markets fill:

```
Resting asks @ $1.00 :  #2 = 100    #3 = 200    #4 = 300      ==> 600 total
Incoming buy         :  300 @ $1.00

Fills                :  #2 → 50     #3 → 100    #4 → 150
                        each resting order gets 50% of its size; leftover lots are
                        handed out by largest fractional remainder, tie-broken by
                        time priority (FIFO)
```

Flip `MatchingEngineConfiguration.Algorithm` to `Fifo` and that same incoming 300 fills the oldest resting order to completion first, then the next, no proportional split whatsoever.

---

## Design decisions

The choices that shaped the engine, and why:

- **Prices as `long` cents, not `decimal`/`double`.** No floating-point drift anywhere in the matching path... comparisons and fills are exact integer math.
- **`OrderbookEntry : Order` *is* the linked-list node.** The order carries its own `Next`/`Previous` pointers, so cancels are O(1) pointer splices with zero wrapper allocations.
- **`SortedSet<Limit>` + `Dictionary<orderId, entry>`.** O(log n) to reach the best bid/ask price level, O(1) to find or cancel any order by id.
- **Pro-rata uses largest-remainder allocation.** Fractional lots are distributed deterministically instead of silently dropped, and ties fall back to time priority.
- **Async logging off the hot path.** Log calls just enqueue onto a thread-safe `BufferBlock`; a background writer drains it to disk and flushes cleanly on shutdown, keeping file I/O out of the execution threads.

---

## Architecture

The solution is split into focused projects so instrument data, order state, book mechanics, and matching never bleed into each other:

```
TradingEngineServer/ (solution root)
├── Instruments/          Security reference data (id + ticker) e.g (1, AAPL)
├── Orders/               Order / limit domain models, comparers, linked-list node
├── OrderBook/            Per-instrument bid/ask book + tiered interfaces
├── MatchingEngine/       Multi-book orchestrator + FIFO / pro-rata algorithms
├── Logging/              Async logger (only text is supported for now in .log files)
├── UnitTests/            xUnit + FluentAssertions
└── TradingEngineServer/  DI wiring
```

**Order book.** Price levels live in a `SortedSet<Limit>` ordered by `BidLimitComparer` (descending) and `AskLimitComparer` (ascending), so the best bid/ask is always `Min`. Every order is additionally indexed in a `Dictionary<long, OrderbookEntry>` for O(1) lookup and cancel.

**Interface tiers.** `IReadonlyOrderBook → IOrderEntryOrderBook → IRetrievalOrderBook → IMatchingOrderBook` widen access one step at a time. A caller takes the narrowest interface it needs: read-only consumers can't mutate, and the fully-mutable book stays internal to the matching path.

**Matching engine.** Holds a dictionary of order books keyed by `SecurityId`, so a single engine serves the whole venue. Algorithms implement `IMatchingAlgorithm` as stateless singletons and are resolved from config via DI: swapping matching algorithms is a one-line settings change in `appsettings.json`

---

## Running it

Requires the **.NET 10 SDK**.

```bash
dotnet build
dotnet test          
dotnet run --project TradingEngineServer/TradingEngineServer.csproj
```

Pick the matching algorithm in `TradingEngineServer/appsettings.json`:

```json
"MatchingEngineConfiguration": {
  "Algorithm": "Fifo"      // or "ProRata"
}
```

The host currently wires up the engine, DI, and logging; order intake over REST/WebSocket is on the roadmap, so for now the matching logic is exercised through the test suite.

### Logs

Written to date-stamped folders under `logs/`. Tail the most recent one live in PowerShell:

```powershell
Get-Content -Path (Get-ChildItem "logs/*/*.log" | Sort-Object LastWriteTime -Descending | Select-Object -First 1) -Wait
```

---

## Tech stack

| Layer     | Technology                |
|-----------|---------------------------|
| Runtime   | .NET 10.0                 |
| Language  | C# 13                     |
| Testing   | xUnit + FluentAssertions  |
| Hosting   | Microsoft Generic Host    |

---

## Roadmap

- [x] Domain models (orders, limit levels, comparers)
- [x] Order-book structural layer (interfaces, sorted sets, linked lists)
- [x] Matching engine (FIFO + pro-rata)
- [x] Unit test suite
- [ ] REST gateway (`POST /orders`, `DELETE /orders/{id}`, `GET /book`)
- [ ] WebSocket / SignalR layer (live book + trade broadcast)
- [ ] Browser dashboard (order entry form + live book table)
- [ ] Cloud deployment (backend on Render, frontend on Vercel)

---
