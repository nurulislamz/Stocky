# Stocky API Documentation

This document provides an overview of the Stocky API, its architecture, and the core principles guiding its development.

## 1. Architectural Overview

The Stocky API follows a clean, multi-layered architecture designed to separate concerns, improve maintainability, and enhance scalability. The flow of a request is typically handled by three distinct layers:

**Controllers → Application Services → Repositories**

### a. Controllers (Thin Layer)

Controllers are the entry point for all HTTP requests. They are designed to be "thin," meaning they contain minimal business logic. Their primary responsibilities are:

-   Receiving and validating incoming HTTP requests.
-   Invoking the appropriate method in an Application Service.
-   Translating the result from the service layer into an HTTP response (e.g., `200 OK`, `404 Not Found`, `409 Conflict`).

This is handled by a `BaseController` that processes success and failure results from the service layer, ensuring consistent responses across the API.

### b. Application Services (Business Logic)

The Application Services (or "APIs" in this codebase, e.g., `PortfolioApi`) contain the core business logic. They orchestrate the application's features and use cases. Key responsibilities include:

-   Executing business rules and complex operations.
-   Coordinating with one or more repositories to fetch and persist data.
-   Returning a `Result` object to the controller, indicating the outcome of the operation.

For example, the `PortfolioApi` handles the logic for buying a ticker by first validating if the user has sufficient funds and then instructing the repository to execute the transaction.

### c. Repositories (Data Access)

The repository layer abstracts the database and data access logic. It provides a clean, strongly-typed interface for querying and manipulating data, completely hiding the underlying data source (Entity Framework Core) from the business logic layer.

Key characteristics:

-   Exposes methods for data operations (e.g., `GetPortfolioFromUserIdAsync`, `BuyHoldingAsync`).
-   Contains all the `DbContext` interactions and LINQ queries.
-   Ensures that the business logic is not directly tied to a specific database technology.

## 2. Core Principles

### a. Clean Code

We strive to follow clean code principles to ensure the codebase is readable, understandable, and easy to maintain. This includes using meaningful names, keeping methods short and focused, and adhering to the SOLID principles of object-oriented design.

#### SOLID Principles

-   **S - Single Responsibility Principle (SRP)**: A class should have only one reason to change. In our architecture, this is exemplified by the clear separation of concerns: Controllers handle HTTP traffic, Application Services contain business logic, and Repositories manage data access.

-   **O - Open/Closed Principle (OCP)**: Software entities (classes, modules, functions) should be open for extension but closed for modification. We achieve this by using interfaces (e.g., `IPortfolioApi`, `IPortfolioRepository`) and dependency injection, allowing new functionality to be added by implementing new classes rather than changing existing ones.

-   **L - Liskov Substitution Principle (LSP)**: Subtypes must be substitutable for their base types. We ensure that derived classes can be used in place of their base classes without causing issues, maintaining a predictable and reliable class hierarchy.

-   **I - Interface Segregation Principle (ISP)**: Clients should not be forced to depend on interfaces they do not use. We define small, cohesive interfaces (e.g., `IUserContext`, `IFundsApi`) that are specific to the client's needs, preventing "fat" interfaces that lead to unnecessary dependencies.

-   **D - Dependency Inversion Principle (DIP)**: High-level modules should not depend on low-level modules. Both should depend on abstractions. This is a cornerstone of our architecture. Application Services depend on repository *interfaces*, not concrete repository classes. The specific implementation is provided at runtime by the dependency injection container, which "inverts" the control of dependency creation.

### b. The Result Pattern

For handling operation outcomes, the API uses a **Result Pattern** instead of relying heavily on exceptions for control flow.

-   **How it Works**: Application service methods return a `Result<T>` object. This object wraps either a successful value (`T`) or a `Failure` object.
-   **Benefits**:
    -   **Explicit Outcomes**: It makes the success and failure paths of a method explicit. You know exactly what kind of errors a method can return just by its signature.
    -   **No Exceptions for Flow Control**: Predictable errors, such as "ticker not found" or "insufficient funds," are handled as normal return values rather than exceptions. This avoids the performance overhead of exceptions and leads to cleaner code in the controllers.
    -   **Rich Error Information**: The `Failure` object contains detailed information about what went wrong, including a status code, title, and detail, which is then used to generate a consistent `ProblemDetails` response.

This pattern is implemented in the `Result<T>` and `Failure` classes within the `stockyapi.Middleware` namespace.

## 3. API Testing and Coverage Strategy

To ensure the robustness, reliability, and maintainability of the Stocky API, a comprehensive testing strategy is employed, aiming for high test coverage across all layers.

### a. Unit Tests

Unit tests focus on individual components (e.g., methods within Application Services, Repository methods, utility functions) in isolation. They are designed to verify that each unit of code performs as expected, covering various input scenarios and edge cases.

-   **Goal**: Achieve high statement and branch coverage for core business logic within Application Services and critical utility functions.
-   **Implementation**: Mocks and stubs are heavily utilized to isolate the unit under test from its dependencies (e.g., repositories, external services).
-   **Benefits**: Fast execution, easy to pinpoint failures, and provides immediate feedback during development.

### b. Integration Tests

Integration tests verify the interactions between different components or layers of the API. This includes testing the flow from a Controller through an Application Service to a Repository, ensuring that these layers work together correctly.

-   **Goal**: Validate the correct interaction between Controllers, Application Services, and Repositories, and ensure proper data persistence and retrieval.
-   **Implementation**: These tests often involve a test database or an in-memory database to simulate real data interactions without relying on external systems.
-   **Benefits**: Catches issues related to component integration and data flow.

### c. End-to-End (E2E) Tests

E2E tests simulate real user scenarios, testing the entire system from the client (e.g., UI or another service) to the database and back. While primarily focused on the UI, a subset of E2E tests can validate critical API endpoints from an external perspective.

-   **Goal**: Ensure that critical business workflows function correctly across the entire system.
-   **Implementation**: These tests interact with the deployed API endpoints and verify the overall system behavior.

### d. Test Coverage

Maintaining high test coverage is a key objective to minimize bugs and ensure code quality.

-   **Measurement**: Code coverage tools are integrated into the CI/CD pipeline to track statement, branch, and line coverage.
-   **Targets**:
    -   **Application Services**: Aim for 90%+ line coverage due to their critical business logic.
    -   **Repositories**: Aim for 80%+ line coverage to ensure data access operations are thoroughly tested.
    -   **Controllers**: Focus on integration tests that cover controller actions, as unit testing controllers in isolation often provides less value than testing the full request-response cycle.
-   **Process**: Code reviews include scrutiny of test coverage, and new features or bug fixes require corresponding tests to maintain or improve coverage metrics.
