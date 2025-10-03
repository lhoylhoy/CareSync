# Coding conventions for CareSync

This document contains short, actionable conventions to keep the codebase consistent.

Core principles
- Prefer clarity and ownership. Put types where they belong (Application, Domain, Infrastructure, API, Web.Admin).
- File names should match public type names (PascalCase) and one public type per file when reasonable.
- Use PascalCase for types, methods, properties, and enums. Prefix interfaces with `I`.
- Async methods must end with `Async`.
- Use `_camelCase` for private fields and `_httpClient`, `_logger` for dependencies.

Project / namespace layout
- Projects: `CareSync.Domain`, `CareSync.Application`, `CareSync.Infrastructure`, `CareSync.API`, `CareSync.Web.Admin`.
- Namespaces mirror folder paths, e.g. `CareSync.Application.Common.Geographics`.

DTOs / Commands / Handlers
- DTO suffixes: `Dto`, `CreateXxxDto`, `UpdateXxxDto`, `UpsertXxxDto`.
- Commands/Queries: `CreatePatientCommand`, `GetPatientByIdQuery`.
- Handlers: `CreatePatientCommandHandler`.

API and routes
- Controllers: plural + `Controller` (e.g. `PatientsController`).
- Routes: lowercase, plural nouns (e.g. `/api/patients`, `/api/geographic-data/provinces`).

Blazor
- Components and pages: PascalCase filenames that match the component name (e.g. `PatientForm.razor`).

Tests
- Test class: `<ClassUnderTest>Tests` or `<Feature>Tests`.
- Test methods: `MethodName_StateUnderTest_ExpectedBehavior`.

Tools & enforcement
- `.editorconfig` is present at the repo root to enforce basic naming and formatting rules.
- CI (`.github/workflows/ci.yml`) runs `dotnet format --verify-no-changes`, `dotnet build`, and `dotnet test`.

If you'd like, I can follow up to enable StyleCop analyzers centrally in a staged PR (with a baseline) so the build won't be broken by a large number of style warnings.
