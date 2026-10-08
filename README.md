# Candidate Assessment

Blank full-stack starter for the Senior Full-Stack .NET Modernization Developer take-home assignment. The React UI and the ASP.NET Core API are separate projects in one Visual Studio solution. No assignment behavior is implemented.

## Assignment overview

The assignment is the first phase of the technical interview. It asks you to design and build a small system that manages providers and their licenses, including soft deletion, and to connect a React UI to a C# API and a SQLite database.

The starter is intentionally empty:

- The React application renders a blank page.
- The API is an ASP.NET Core Web API shell with controller support and no business endpoints.
- No database, Entity Framework Core model, authentication, dashboard, or business rules are included.

You own the design. The official assignment is the requirements source.

[Take-Home Technical Assignment](assignment/Take-Home_Technical_Assignment_Formatted.pdf)

## Technology stack

Supplied in this starter:

| Layer | Technology |
| --- | --- |
| UI | React 19.3.0 and React DOM 19.3.0 (JavaScript) |
| UI tooling | Vite 8.3.4 and `@vitejs/plugin-react` 6.1.2 |
| API | ASP.NET Core Web API, C#, `net8.0` |
| IDE | Visual Studio 2022 solution (`CandidateAssessment.sln`) |

Declared in `frontend/React.UI/package.json` as `react` `^19.2.8`, `react-dom` `^19.2.8`, `vite` `^8.3.0`, and `@vitejs/plugin-react` `^6.1.1`. The versions above are the ones installed from `package-lock.json`.

Required by the assignment and left for you to add:

- ASP.NET Core Web API on .NET 8 or .NET 10
- Entity Framework Core with the SQLite provider
- SQLite

This starter targets `net8.0`. The assignment also allows .NET 10. You may retarget the API if you choose that option. Document the choice.

## Prerequisites

- [Node.js](https://nodejs.org/) LTS and npm
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or a newer SDK that can build `net8.0` (the .NET 8 targeting pack must be installed)
- Git
- One of:
  - Visual Studio 2022 (17.7 or later) with the **ASP.NET and web development** workload and the **Node.js development** workload
  - Visual Studio Code or Cursor

## Visual Studio 2022 setup

1. Open `CandidateAssessment.sln`.
2. Confirm Solution Explorer shows:
   - `frontend` / `React.UI`
   - `backend` / `Candidate.API`
3. In a terminal, install frontend dependencies:

   ```bash
   cd frontend/React.UI
   npm ci
   ```

4. Run both projects together:
   - Right-click the solution and choose **Configure Startup Projects**.
   - Select **Multiple startup projects**.
   - Set **React.UI** and **Candidate.API** to **Start**.
   - Start debugging.

`React.UI` starts with `npm run dev`. `Candidate.API` starts with the profile selected in Visual Studio. The Development environment serves the OpenAPI document through Swagger UI. That document contains no business operations.

The UI does not call the API. Each project prints its own address when it starts.

## VS Code and Cursor setup

Open the repository folder. Use two terminals.

Frontend:

```bash
cd frontend/React.UI
npm ci
npm run dev
```

API:

```bash
dotnet run --project backend/Candidate.API
```

Use the addresses printed by Vite and by `dotnet run`. No proxy or shared base URL is configured.

## Running the blank React frontend

From `frontend/React.UI`:

| Command | Purpose |
| --- | --- |
| `npm ci` | Install the locked dependencies |
| `npm run dev` | Start the Vite development server |
| `npm run build` | Create the production build in `dist/` |
| `npm run preview` | Preview the production build |

`src/App.jsx` returns `null`, so the running app is a blank page. Application entry is `src/main.jsx`.

## Running the blank ASP.NET Core API

From the repository root:

```bash
dotnet restore
dotnet build CandidateAssessment.sln
dotnet run --project backend/Candidate.API
```

`Program.cs` registers controllers and maps controller endpoints. There are no controllers, routes, models, or database services. A browser request to the site root returns 404 until you add endpoints. In Development, Swagger UI confirms the host started.

## Candidate responsibilities

Implement the system described in the assignment PDF. This repository does not prescribe endpoint names or a database schema.

You are responsible for:

- Designing the SQLite schema and adding Entity Framework Core
- Implementing provider and license behavior, including soft deletion
- Exposing that behavior through the ASP.NET Core Web API
- Building the React workflows and loading application data from the API
- Keeping business rules in the API, not only in the UI
- Adding any sample data or repeatable setup the assignment requires
- Documenting design decisions, assumptions, and trade-offs

Optional dashboard work, if you choose to do it, is described only in the assignment PDF.

## GitHub branching and submission

Starter repository: <https://github.com/balthapa2023/provider-technical-assignment>

1. Fork the repository.
2. Clone your fork.
3. Add the original repository as `upstream`:

   ```bash
   git remote add upstream https://github.com/balthapa2023/provider-technical-assignment.git
   ```

4. Create a branch from `main`. `your-assignment-copy` is an example name:

   ```bash
   git checkout -b your-assignment-copy
   ```

5. Do all work on that branch. Do not commit assignment changes directly to `main`.
6. Commit and push the branch to your fork:

   ```bash
   git push origin your-assignment-copy
   ```

7. Open a pull request from your branch to `main` on the original repository.

You may submit a link to your fork, or a link to the branch or pull request that contains the work. A ZIP file is not required.

## Required documentation

Keep this starter guide accurate if you change how the projects run. Your submission README must also explain:

- How to run the API and the React application locally
- Database design decisions
- How soft deletion is implemented
- How the React UI communicates with the C# API
- Assumptions and trade-offs
- What you would improve with more time

Include the SQLite database file and any SQL scripts you use to create or modify tables, views, or queries.

## Verification steps

From a clean checkout, with Node.js and the .NET SDK on `PATH`:

```bash
dotnet restore
dotnet build CandidateAssessment.sln
cd frontend/React.UI
npm ci
npm run build
```

Then confirm both applications start:

```bash
dotnet run --project backend/Candidate.API
```

```bash
cd frontend/React.UI
npm run dev
```

Expected results:

- The solution build succeeds for `Candidate.API` and `React.UI`.
- The API process stays running and reports no startup exception. Swagger UI in Development lists no business operations.
- The React development server stays running and the page is blank.
- Solution Explorer shows both projects.

## Project layout

```text
CandidateAssessment.sln
frontend/React.UI/          React + Vite application
backend/Candidate.API/      ASP.NET Core Web API (net8.0)
assignment/                 Official assignment PDF
README.md
.gitignore
```
