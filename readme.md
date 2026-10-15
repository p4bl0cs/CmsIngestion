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
* **Consumer & Admin REST API (`GET /api/entities`):**
  * Consumers receive only active, published entities (`IsDisabled == false`).
  * Admins can view all records via query parameters (`?isAdmin=true`) and explicitly override/disable entities (`POST /api/entities/{id}/disable`).
* **Zero-Config Database Startup:** Automatically provisions the SQLite database file (`cms.db`) and tables on application startup using EF Core `EnsureCreated()`.

---

## 🛠️ Getting Started & Running Locally

### Prerequisites
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed on your machine.

### Clone and Run
1. Clone the repository:
   ```bash
   git clone <your-repository-url>
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
  * `Authorization`: `Basic <base64-encoded-credentials>` (Username must be 10–20 chars; Password must be a GUID, e.g., `adminuser123:d3b07384-d113-4ec2-849e-270edbbe37d3`)
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