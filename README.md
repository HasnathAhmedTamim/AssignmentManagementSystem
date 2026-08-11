# Assignment & Submission Management System

Role-based full-stack web application for schools/colleges. Teachers create and grade assignments; students submit work and view feedback; admins manage users, classes, subjects, teacher assignments, and enrollments.

## Main features

- JWT authentication with Admin, Teacher, and Student roles
- Admin: user management, classrooms, subjects, teacher–class–subject mapping, student enrollments
- Teacher: create/update/delete assignments, draft/publish workflow, review submissions, marks & feedback
- Student: view published assignments for enrolled classes, submit/update before deadline, view status/marks/feedback
- FluentValidation, structured error responses, Serilog request logging, Swagger/OpenAPI
- EF Core migrations + seed data for easy local setup
- Unit tests for auth, authorization rules, and submission workflows

## Technology stack

| Layer | Technology |
|-------|------------|
| Frontend | Next.js 15, React, TypeScript, Tailwind CSS |
| Backend | ASP.NET Core 9 Web API, C#, FluentValidation, Serilog, Swagger |
| Database | PostgreSQL + EF Core |
| Auth | JWT + role-based authorization |
| Tests | xUnit, Moq, FluentAssertions |

## Project structure

```
AssignmentManagementSystem/
├── backend/
│   ├── AssignmentManagement.Api/             # Controllers, middleware, DI extensions
│   ├── AssignmentManagement.Application/     # Feature modules (DTOs/Services/Validators)
│   ├── AssignmentManagement.Domain/          # Entities & enums
│   ├── AssignmentManagement.Infrastructure/  # EF Core, JWT, seed, repositories
│   ├── AssignmentManagement.Tests/           # Unit tests
│   └── AssignmentManagementSystem.sln
├── frontend/
│   └── src/
│       ├── app/                              # Next.js routes (thin pages)
│       ├── features/                         # Feature UI modules (admin managers, etc.)
│       ├── shared/                           # Reusable hooks, table/modal helpers
│       ├── components/                       # Layout + base UI primitives
│       ├── context/                          # Auth provider
│       └── lib/                              # API client, types, formatters
├── docker-compose.yml
├── .env.example
└── README.md
```

## Prerequisites

- .NET 9 SDK
- Node.js 20+ and npm
- PostgreSQL 16+ (or Docker)

## Database setup

### Option A — Docker

```bash
docker compose up -d
```

This starts PostgreSQL on port `5432` with:

- Database: `assignment_management_db`
- User: `postgres`
- Password: `12345`

### Option B — Local PostgreSQL

Create a database named `assignment_management_db` and update the connection string in:

`backend/AssignmentManagement.Api/appsettings.json`

or copy `appsettings.Development.json.example` to `appsettings.Development.json` (gitignored).

Migrations and seed data run automatically when the API starts. No manual table creation is required.

## Environment configuration

Copy `.env.example` and adjust values as needed. Do **not** commit real production secrets.

Frontend:

```bash
cp frontend/.env.example frontend/.env.local
```

Default:

```
NEXT_PUBLIC_API_URL=http://localhost:5249/api
```

## Run the backend

```bash
cd backend/AssignmentManagement.Api
dotnet restore
dotnet run --launch-profile http
```

- API: http://localhost:5249
- Swagger: http://localhost:5249/swagger

## Run the frontend

```bash
cd frontend
npm install
npm run dev
```

App: http://localhost:3000

## Run the tests

```bash
cd backend
dotnet test
```

## Demo credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@school.com | Admin@123 |
| Teacher | teacher@school.com | Teacher@123 |
| Student | student@school.com | Student@123 |

Seeded sample data also includes:

- Class `Grade 10 / A`
- Subjects `Mathematics (MATH-101)`, `English (ENG-101)`
- Teacher assigned to both subjects for that class
- Students enrolled in the class
- One published assignment (`Algebra Basics`) and one draft assignment

## Assumptions

1. A **classroom** is identified by `Name` + `Section` (e.g. Grade 10 / A).
2. Teachers are linked to teaching work via **Teacher–Class–Subject** rows; assignments are created against those rows.
3. Students see only **Published** assignments for classes they are enrolled in.
4. New assignments start as **Draft**; teachers (or admin) publish them.
5. Students may **update** a submission until the deadline; after the deadline, updates are blocked.
6. A first submission after the deadline is stored with status **Late**.
7. Marks must be between `0` and the assignment’s `MaximumMarks`.
8. Only the owning teacher (or Admin) can grade submissions for an assignment.
9. Text answers are stored as plain text (no file uploads in this version).
10. Demo JWT key and DB password in `appsettings.json` are for **local evaluation only**.

## Known limitations

- No email/push notifications
- No file attachments for assignments or submissions
- No pagination/advanced filtering beyond role-scoped lists
- Soft-delete is not implemented (hard delete)
- Single active enrollment model is simple (no academic-year history)

## Final checklist mapping

- [x] Frontend and backend included
- [x] Migrations + seed for database setup
- [x] Demo accounts for Admin, Teacher, Student
- [x] README with run/test instructions
- [x] Role-based access enforced by API
- [x] Business rules covered by unit tests
- [x] No real production secrets required in the repo (demo values only; see `.env.example`)
