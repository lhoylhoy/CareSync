# CareSync - Healthcare Management System

![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![Build Status](https://github.com/your-org/caresync/workflows/CI%2FCD%20Pipeline/badge.svg)
![Code Coverage](https://codecov.io/gh/your-org/caresync/branch/main/graph/badge.svg)

A modern, clean healthcare management system built with .NET 9 and Clean Architecture principles. This system is specifically designed for healthcare facilities in the Philippines with integrated geographic data support.

## 🎯 Strategic Overhaul Initiative

> **October 2025**: We're overhauling CareSync to make everything intentional and simplified for its core purpose.

**📖 Start Here:**
- **[GETTING_STARTED.md](./GETTING_STARTED.md)** - Your entry point to the overhaul initiative
- **[VISUAL_ROADMAP.md](./VISUAL_ROADMAP.md)** - Quick visual overview of the transformation
- **[STRATEGIC_OVERHAUL_PLAN.md](./STRATEGIC_OVERHAUL_PLAN.md)** - Complete vision and strategy
- **[REFACTORING_TASKS.md](./REFACTORING_TASKS.md)** - Detailed implementation tasks
- **[QUICK_WINS.md](./QUICK_WINS.md)** - Start improving today (1-2 days)

**Goal:** Transform from a complex, over-engineered system to an intentionally simple, user-focused healthcare management platform.

## 🏗️ Architecture

This solution follows **Clean Architecture** principles with clear separation of concerns:

```
src/
├── CareSync.Domain/          # Core Domain Layer - Business entities and logic
├── CareSync.Application/     # Application Layer - Use cases and orchestration
├── CareSync.Infrastructure/  # Infrastructure Layer - Data persistence and external services
├── CareSync.API/             # Presentation Layer - RESTful API endpoints
└── CareSync.Web.Admin/       # Admin UI - Blazor WebAssembly interface

tests/
├── CareSync.Domain.UnitTests/
├── CareSync.Application.UnitTests/
└── CareSync.API.IntegrationTests/
```

## 🚀 Features

### Core Functionality
- **Patient Management**: Full CRUD operations for patient records with Philippine address format
- **Philippine Identification**: Support for PhilHealth, SSS, and TIN numbers
- **Philippine Geographic Data**: Official province, city/municipality, and barangay data
- **Doctor Management**: Healthcare provider profiles and specialties
- **Appointment Scheduling**: Complete appointment lifecycle management
- **Medical Records**: Secure medical history and documentation
- **Billing & Payments**: Integrated billing and payment processing
- **Domain-Driven Design**: Rich domain models with business logic
- **CQRS Pattern**: Separate command and query operations
- **Azure Integration**: Cloud-based data persistence and authentication

### 🎨 Modern UI/UX (v2.0 - October 2025)
- **Consistent Design System**: Healthcare-focused color palette with design tokens
- **Smooth Animations**: Polished transitions and micro-interactions
- **Responsive Layout**: Mobile-first design for all devices
- **Enhanced Components**: Gradient buttons, cards, and status badges
- **Accessibility**: WCAG AA compliant with keyboard navigation
- **Professional Dashboard**: Color-coded metrics with hover effects
- **Style Guide**: Comprehensive component showcase (`/style-guide`)
- **Loading States**: Skeleton screens and animated spinners
- **Empty States**: Helpful placeholders with call-to-actions

📚 **See full UI/UX improvements:** [ENHANCEMENT_SUMMARY.md](./ENHANCEMENT_SUMMARY.md) | [VISUAL_GUIDE.md](./VISUAL_GUIDE.md)

## 🛠️ Technology Stack

- **.NET 9.0** - Latest .NET framework
- **Entity Framework Core 9.0** - Database access
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **Blazor WebAssembly** - Admin interface
- **Azure SQL Database** - Data persistence
- **Azure Entra ID** - Authentication and authorization
- **Docker** - Containerization

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (Local or Azure)
- [Docker](https://www.docker.com/) (Optional)

## 🚦 Getting Started

# CareSync

CareSync is a healthcare management system built with .NET 9 using Clean Architecture principles. It includes patient management, appointments, medical records, billing, and integrated Philippine geographic data.

This repository contains:

- src/CareSync.API — ASP.NET Core Web API (presentation)
- src/CareSync.Application — Application layer (CQRS handlers)
- src/CareSync.Domain — Domain entities and value objects
- src/CareSync.Infrastructure — EF Core and repositories
- src/CareSync.Web.Admin — Blazor WebAssembly admin UI

## Quick start

Prerequisites:

- .NET 9 SDK
- SQL Server (local or Docker)
- Docker (optional)

Local development (recommended):

1. Restore packages and build the solution:

   dotnet restore
   dotnet build

2. Start a local SQL Server (optional):

   docker-compose -f docker-compose.local.yml up -d

3. Update connection string in `src/CareSync.API/appsettings.Development.json` if needed.

4. Apply EF migrations:

   cd src/CareSync.API
   dotnet ef database update

5. Run the API:

   dotnet run

The API will be available at https://localhost:7262 (or http://localhost:5262). The Blazor admin UI runs separately from `src/CareSync.Web.Admin`.

## Running tests

Run all tests from the repo root:

  dotnet test

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit and push changes
4. Open a pull request

## License

MIT

---

If you want me to expand any section (detailed API docs, environment variables, deploy instructions, or a developer checklist), tell me which parts to include and I'll extend it.
```bash
