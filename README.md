# C# Limit Order Book & Matching Engine

A pet project built from scratch on **.NET 10** using C# to understand the fundamentals of quantitative development (quant-dev) workflows, order matching logic, and electronic trading architectures.

---

## Repository Architecture

The solution is divided into three distinct modules to separate concerns:

```text
TradingEngineServer/ (Solution Root)
├── Logging/            # Custom lock-free text logging library
├── Orders/             # Domain models (order types, rejections, comparers)
└── TradingEngineServer/# Main application executable & Hosted Services shell
```

### 1. `TradingEngineServer` (Main Host)
*   **Purpose:** The application entry point and hosting lifecycle manager.
*   **Key Responsibilities:**
    *   Manages server startup and shutdown hooks using Microsoft Generic Host.
    *   Loads configurations via the Options pattern (`appsettings.json`).
    *   Sets up Dependency Injection (DI) providers.

### 2. `Logging` (Log Library)
*   **Purpose:** An asynchronous text logger designed to offload file writing from execution threads.
*   **Key Responsibilities:**
    *   Uses a thread-safe `BufferBlock` queue to store log items asynchronously.
    *   Maintains a background writer task to log output to dynamically named local files.
    *   Drains all remaining queued logs during process disposal/cancellation.

### 3. `Orders` (Domain Models)
*   **Purpose:** Implements the core domain models for orders, rejections, and limit level indexing.
*   **Key Responsibilities:**
    *   **Immutability:** Identity fields are kept strictly read-only (`IOrderCore` & `OrderCore`) to prevent state corruption.
    *   **Memory Efficiency:** `OrderbookEntry` directly inherits from `Order` to behave as a doubly-linked list node, eliminating double allocations and pointer dereferencing cache jumps.
    *   **Cents-Based Pricing:** To avoid floating-point rounding errors, all prices are stored as integers (`long`) in **cents/ticks**, and quantities are stored as `uint`.
    *   **Strict Sorting:** Uses `BidLimitComparer` (descending) and `AskLimitComparer` (ascending) with early-return null checks to maintain strict weak ordering without tree corruption.

---

## Tech Stack

*   **.NET 10.0** Target Framework.

---

##  How to Run the Project

Ensure you have the **.NET 10 SDK** installed.

### 1. Build the solution
Run the following from the root directory:
```bash
dotnet build
```

### 2. Run the server
Start the trading engine:
```bash
dotnet run --project TradingEngineServer/TradingEngineServer.csproj
```

### 3. View Logs
Logs are written to the root directory under folders named with the current date (e.g. `2026-07-06/`). You can stream them in real time using PowerShell:
```powershell
Get-Content -Path (Get-ChildItem -Path "2026-*/*.log" | Sort-Object LastWriteTime -Descending | Select-Object -First 1) -Wait
```
