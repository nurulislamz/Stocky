# GEMINI.md

## Project Overview

This is a full-stack application for stock portfolio tracking. It consists of a .NET backend API (`stockyapi`) and a React frontend (`stockyui`).

**Backend:**
*   **.NET 10** ASP.NET Core Web API
*   **Database:** PostgreSQL (production) / SQLite (development) with Entity Framework Core
*   **Authentication:** JWT
*   **Testing:** NUnit
*   **API Documentation:** Swagger

**Frontend:**
*   **React** with TypeScript
*   **UI Library:** Material-UI
*   **API Client Generation:** NSwag

## Building and Running

### Backend

To run the backend API:

```bash
dotnet run --project stockyapi
```

The API will be available at `https://localhost:5001`.

### Frontend

To run the frontend React application:

```bash
cd stockyui
npm install
npm start
```

The frontend will be available at `http://localhost:3000`.

### Testing

To run the .NET tests:

```bash
dotnet test
```

To run the React tests:

```bash
cd stockyui
npm test
```

## Development Conventions

### Backend Architecture

The backend follows a clean, multi-layered architecture:

*   **Controllers:** Thin layer for handling HTTP requests.
*   **Application Services:** Contain the core business logic.
*   **Repositories:** Abstract data access logic using Entity Framework Core.

This architecture is guided by **SOLID principles** and uses a **Result Pattern** for handling operation outcomes, which avoids using exceptions for control flow.

### API Client Generation

The frontend uses **NSwag** to generate a TypeScript client from the backend's Swagger/OpenAPI specification. To regenerate the client:

```bash
cd stockyui
npm run generate-api
```

### Database

The application uses **PostgreSQL** in production and **SQLite** for development. The connection string is configured in `stockyapi/appsettings.json`. Entity Framework Core is used for data access, and migrations are managed with `dotnet ef`.
