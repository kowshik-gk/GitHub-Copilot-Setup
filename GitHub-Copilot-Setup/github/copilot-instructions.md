# Copilot Instructions

## 1. Project Overview
Build maintainable, secure, and scalable full-stack applications using .NET and Angular. Follow Clean Architecture principles, separation of concerns, SOLID principles, and consistent coding standards. Keep implementations concise, reusable, and production-ready.

## 2. Technology Stack
- **Backend:** .NET 8, C#, ASP.NET Core Web API, Minimal APIs, REST APIs, LINQ, asynchronous programming.
- **Architecture & Data Access:** Clean Architecture, Entity Framework Core, Repository Pattern, Dependency Injection, SQL Server, PostgreSQL, Azure Cosmos DB.
- **Frontend:** Angular 16+, TypeScript, HTML, CSS.
- **Cloud & DevOps:** Microsoft Azure, Azure Functions, Service Bus, App Services, Event Grid, Entra ID, Azure DevOps CI/CD.
- **Authentication:** OAuth 2.0, JWT claims-based authentication, SSO, secure username/password authentication.
- **Testing:** xUnit, EF Core In-Memory Database, Playwright.
- **Tools:** Git, Azure Repos, Postman, Swagger, Visual Studio.

## 3. Architecture and Design
- Follow Clean Architecture with clear separation between Domain, Application, Infrastructure, and API/Presentation layers.
- Keep business rules in the Application/Domain layers, not in controllers or endpoints.
- Use thin controllers or Minimal API endpoints; delegate business operations to services.
- Apply the Repository Pattern for data access where appropriate. Keep EF Core queries and persistence logic in the Infrastructure layer.
- Use interfaces and constructor-based Dependency Injection throughout the application.
- Register services, repositories, and database contexts in the dependency injection container.
- Use Entity Framework Core migrations and configurations to manage database schemas and relationships.
- Use async/await for I/O-bound operations and support cancellation tokens where appropriate.
- Avoid unnecessary abstractions, duplicated logic, and over-engineering.

## 4. Coding Standards
- Follow C# naming conventions, SOLID principles, and clean, readable coding practices.
- Use DTOs for API requests and responses; avoid exposing database entities directly.
- Apply appropriate validation, nullable reference types, and consistent error handling.
- Use LINQ and EF Core efficiently; avoid N+1 queries and unnecessary database calls.
- Use Angular services for API communication and keep components focused on UI behavior.
- Use TypeScript types and interfaces instead of `any` wherever possible.
- Follow Angular best practices for components, dependency injection, routing, and state management.
- Keep methods focused, reusable, and easy to test.
- AsNoTracking should be used for read-only queries to improve performance.
- async/await should be used for I/O-bound operations, and cancellation tokens should be supported where appropriate.

## 5. Security
- Never hardcode credentials, API keys, connection strings, or tokens.
- Store secrets in Azure Key Vault or AWS Secrets Manager, as appropriate to the deployment environment.
- Validate and sanitize untrusted input; enforce server-side validation.
- Apply authentication and role/claims-based authorization to protected endpoints.
- Follow least-privilege access for Azure/AWS identities, IAM roles, and database permissions.
- Use HTTPS and secure token-handling practices.
- For Amazon Bedrock integrations, apply Bedrock Guardrails where appropriate and validate model inputs and outputs.
- Never log passwords, access tokens, secrets, or sensitive personal data.

## 6. Testing and Quality
- Use xUnit for unit and integration tests.
- Use EF Core In-Memory Database for suitable isolated unit tests; use a realistic database provider for tests requiring relational behavior.
- Mock external dependencies when appropriate and test business logic independently.
- Use Playwright for Angular end-to-end tests covering critical user workflows.
- Test success cases, validation failures, authorization failures, and error scenarios.
- Keep tests deterministic, readable, and independent of execution order.
- Ensure builds, tests, and relevant quality checks pass before considering work complete.

## 7. Output Preferences
- Provide concise, actionable solutions with minimal unnecessary explanation.
- Prefer complete, working code examples that fit the existing project structure and conventions.
- When modifying code, explain which files need changes and why.
- Follow existing patterns unless there is a clear reason to improve them.
- Include required dependency registrations, configuration changes, DTOs, and tests when relevant.
- Avoid unnecessary packages, abstractions, boilerplate, and unrelated refactoring.
- State assumptions or trade-offs briefly; ask questions only when essential information is missing.
- Prioritize maintainability, security, performance, and production readiness over clever or overly complex solutions.