# CMS Ingestion & Event Processing API

A robust, enterprise-grade .NET 9 Web API built with **Clean Architecture** principles to handle asynchronous batch content management system (CMS) webhooks, version tracking, out-of-order revision safety, and admin overrides.

---

## 🏛️ Architectural Overview

The solution follows strict Clean Architecture and SOLID principles, split across four distinct projects to ensure complete separation of concerns:

1. **`CmsIngestion.Core`** (Domain Layer)
   - Zero dependencies. Contains core domain entities (`CmsEntity`) and repository contracts (`ICmsRepository`).
2. **`CmsIngestion.Application`** (Application Layer)
   - References `Core`. Contains DTOs (`CmsEventDto`) and core event processing service logic (`CmsEventService`) implementing version tracking and safety rules.
3. **`CmsIngestion.Infrastructure`** (Infrastructure Layer)
   - References `Core`. Implements data persistence using Entity Framework Core with SQLite, complete with dependency injection extensions.
4. **`CmsIngestion.Api`** (Presentation Layer)
   - References `Application` and `Infrastructure`. Exposes endpoints, coordinates dependency injection, and includes custom Basic Authentication middleware.

---

## 🚀 Features & Requirements Fulfilled

* **.NET 9 Solution:** Built natively for .NET 9.0.
* **Secured Webhook (`POST /cms/events`):** Protected by a custom **Basic Authentication Middleware** validating requirements:
  * **Username:** 10 to 20 characters length.
  * **Password:** Valid GUID format.
* **Batch Event Processing:** Supports processing multiple events per payload with safe handling for:
  * `publish`: Creates or updates entities, respecting version numbers to prevent out-of-order race conditions.
  * `unPublish`: Sets the `IsDisabled` flag to true while maintaining version safety.
  * `delete`: Soft/hard deletes records safely.
* **Consumer & Admin REST API (`GET /cms/entities`):**
  * Consumers receive only active, published entities (`IsDisabled == false`).
  * Admins can view all records via the `X-User-Role: admin` header and explicitly override/disable entities (`PATCH /cms/entities/{id}/disable`).
* **Zero-Config Database Startup:** Automatically provisions the SQLite database file (`cms.db`) and tables on application startup using EF Core `EnsureCreated()`.

---

## 🛠️ Getting Started & Running Locally

### Prerequisites
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed on your machine.

### Clone and Run
1. Clone the repository:
   ```bash
   git clone https://github.com/p4bl0cs/CmsIngestion.git
   cd CmsIngestion
   ```
2. Build the solution:
   ```bash
   dotnet build
   ```
3. Run the API project:
   ```bash
   dotnet run --project CmsIngestion.Api
   ```

The application will start locally (typically on `https://localhost:5001` or `http://localhost:5000`). If development mode is enabled, Swagger UI is available at `/swagger` for interactive testing.

---

## 🧪 Testing the Webhook

You can test the webhook endpoint using `curl` or Postman. 

**Request Example (`POST /cms/events`):**
* **Headers:**
  * `Authorization`: `Basic <base64-encoded-credentials>` (Must match the default `appsettings.json` credentials, e.g., `webhookadmin:a1b2c3d4-e5f6-7890-abcd-ef1234567890`)
  * `Content-Type`: `application/json`
* **Body:**
  ```json
  [
    {
      "id": "entity-001",
      "type": "publish",
      "version": 1,
      "timestamp": "2026-10-14T12:00:00Z",
      "payload": {
        "title": "Hello World",
        "content": "This is a test CMS entity."
      }
    }
  ]
  ```
  
---

## ?? REST API Endpoints & Testing

### 1. Consumer Listing (Public View)
- **GET `/cms/entities`**
  - **Description:** Returns only active, published entities (automatically hides disabled or unpublished records).

### 2. Admin Listing (All Records)
- **GET `/cms/entities`**
  - **Headers:** `X-User-Role: admin`
  - **Description:** Returns all entities, including disabled/unpublished ones.

### 3. Admin Disable Override
- **PATCH `/cms/entities/{id}/disable`**
  - **Headers:** `X-User-Role: admin`
  - **Description:** Performs a local API overwrite to disable an entity without altering the upstream CMS data stream.

---

## ?? Performance & Observability Highlights
- **EF Core Optimization:** Read operations utilize `.AsNoTracking()` to reduce memory overhead and maximize query performance under high load.
- **Global Error Handling:** RFC 7807 compliant `ProblemDetails` middleware catches unhandled exceptions and returns standardized JSON error payloads with unique TraceIDs.