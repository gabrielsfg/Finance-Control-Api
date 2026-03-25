# Finance Control API — Personal Finance Management Backend

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13.0-239120?style=flat&logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-EF_Core_9-336791?style=flat&logo=postgresql)
![JWT](https://img.shields.io/badge/Auth-JWT_Bearer-000000?style=flat&logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/Docs-Swagger-85EA2D?style=flat&logo=swagger)
![Status](https://img.shields.io/badge/Status-Moved_to_Monorepo-blue?style=flat)

> ## ⚠️ This repository is no longer maintained
>
> Development has moved to a **monorepo** that combines the .NET API and the Flutter app in one place.
>
> ### 👉 [github.com/gabrielsfg/Finance-Control](https://github.com/gabrielsfg/Finance-Control)
>
> This repository is kept as a reference only. All new code, issues, and pull requests should go to the monorepo above.

---

## About the Project

Finance Control API is the backend of a full-stack personal finance system. The goal of this project is to give users full visibility and control over their financial life: tracking income and expenses, organizing spending into budgets, and surfacing insights about where money is going.

The API supports multiple financial accounts per user, a flexible multi-level category system (Category → SubCategory → Area), and three types of transactions: one-time, installment-based, and recurring. It also provides a dashboard summary endpoint that powers the app's home screen with balance analytics, budget performance, and top spending categories.

This is a personal project built to deepen expertise in .NET backend development, clean architecture, and real-world API design patterns.

---

## Features

| Feature | Status |
|---|---|
| User registration & JWT authentication | ✅ Done |
| Refresh token rotation (single-use, 7-day expiry) | ✅ Done |
| Email verification on registration | ✅ Done |
| Forgot password & reset password via email | ✅ Done |
| Logout (refresh token revocation) | ✅ Done |
| User profile update (name, currency, language, country) | ✅ Done |
| Account deletion with password confirmation (LGPD) | ✅ Done |
| Financial data reset with password confirmation | ✅ Done |
| Multi-account management (balance + savings goal) | ✅ Done |
| Hierarchical categories (Category → SubCategory → Area) | ✅ Done |
| One-time, installment & recurring transactions | ✅ Done |
| Payment method field on transactions | ✅ Done |
| Country-based payment method validation | ✅ Done |
| Budget management with sub-category allocations | ✅ Done |
| Dashboard analytics (balance, top categories, budget performance) | ✅ Done |
| Wishlist item management (CRUD + pagination) | ✅ Done |
| Wishlist price tracking & price history | ✅ Done |
| Wishlist item purchase (creates linked expense transaction) | ✅ Done |
| Wishlist priority & status (Active / Purchased / Archived) | ✅ Done |
| Swagger UI with Bearer token support | ✅ Done |

---

## Tech Stack

| Technology | Purpose | Version |
|---|---|---|
| ASP.NET Core | Web API framework | .NET 9.0 |
| Entity Framework Core | ORM & database migrations | 9.0 |
| PostgreSQL | Relational database | — |
| JWT Bearer | Authentication | — |
| FluentValidation | Request DTO validation | 12.x |
| Swagger / Swashbuckle | API documentation | 10.x |
| SmtpClient | Email (verification & password reset) | — |

---

## Architecture

The solution follows a **Layered (N-Tier) Architecture** split into 5 projects with clear separation of concerns:

```
FinanceControl.sln
├── FinanceControl.Domain        # Entities, interfaces, enums — no dependencies
├── FinanceControl.Data          # EF Core DbContext, migrations, entity mappings
├── FinanceControl.Services      # Business logic, FluentValidation validators
├── FinanceControl.Shared        # DTOs (request/response), helpers, shared models
└── FinanceControl.WebApi        # Controllers, DI setup, Swagger, entry point
```

**Key patterns used:**
- **Result\<T\> pattern** — service methods return `Result<T>` instead of throwing exceptions, making error handling explicit and predictable
- **OwnedEntity base class** — all user-owned entities inherit `UserId`, enforcing multi-user data isolation at the model level
- **BaseController** — shared `GetUserId()` method extracts the authenticated user's ID from JWT claims, used across all controllers
- **Repository-less services** — business logic lives in services that directly use the EF Core DbContext, keeping the stack pragmatic and lean
- **SHA256 token hashing** — refresh tokens and password reset tokens are stored hashed, never in plain text

---

## API Endpoints Overview

All endpoints except `/api/user/register` and `/api/user/login` require an `Authorization: Bearer <token>` header.

| Group | Endpoints |
|---|---|
| Auth | `POST /api/user/register`, `POST /api/user/login`, `POST /api/user/refresh`, `POST /api/user/logout` |
| Email | `GET /api/user/verify-email` |
| Password | `POST /api/user/forgot-password`, `POST /api/user/reset-password` |
| User Profile | `GET /api/user/me`, `PATCH /api/user/me`, `DELETE /api/user/me`, `POST /api/user/me/reset-data` |
| Accounts | CRUD `/api/account` |
| Categories | CRUD `/api/category`, `/api/subcategory` |
| Areas | CRUD `/api/area`, `/api/areacategory` |
| Budgets | CRUD `/api/budget`, `/api/budgetsubcategoryallocation` |
| Transactions | CRUD `/api/transaction` + recurring management |
| Dashboard | `GET /api/mainpage/summary?startDate=&finishDate=&budgetId=` |
| Wishlist | CRUD `/api/wishlist`, `POST /api/wishlist/{id}/price`, `POST /api/wishlist/{id}/purchase`, `GET /api/wishlist/{id}/price-history` |

Full interactive documentation is available via Swagger UI after running the project.

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) (v12+)

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/gabrielsfg/Finance-Control-Api.git
cd Finance-Control-Api

# 2. Restore dependencies
dotnet restore
```

### Configuration

Edit `FinanceControl.WebApi/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FinanceControlDb;Username=postgres;Password=YOUR_PASSWORD"
  },
  "AppSettings": {
    "Token": "your-secret-key-minimum-32-characters",
    "Issuer": "http://localhost:5112",
    "Audience": "http://localhost:5112",
    "TokenValidityMins": 60,
    "BackendUrl": "http://localhost:5112"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUser": "your@email.com",
    "SmtpPass": "your-password",
    "FromAddress": "your@email.com"
  }
}
```

### Run Migrations & Start

```bash
# Apply database migrations
dotnet ef database update --project FinanceControl.Data --startup-project FinanceControl.WebApi

# Run the API
cd FinanceControl.WebApi
dotnet run
```

The API will be available at `http://localhost:5112`.
Swagger UI: `http://localhost:5112/swagger/index.html`

---

## Monorepo

This repository has been archived in favour of a monorepo that contains both the API and the Flutter mobile client:

**[github.com/gabrielsfg/Finance-Control](https://github.com/gabrielsfg/Finance-Control)**
