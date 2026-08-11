# Assignment & Submission Management System

Role-based full-stack web application for schools and colleges. Admins manage users, classes, and subjects; teachers create and grade assignments; students submit work and view marks and feedback.

**Repository:** https://github.com/HasnathAhmedTamim/AssignmentManagementSystem

---

## Quick start

```bash
# 1) Database
docker compose up -d

# 2) Backend (http://localhost:5249)
cd backend/AssignmentManagement.Api
dotnet run --launch-profile http

# 3) Frontend (new terminal)
cd frontend
cp .env.example .env.local
npm install
npm run dev -- -p 3001
```

Open **http://localhost:3001** and sign in with a demo account below.

> If port `3000` is free, you can use `npm run dev` instead (default Next.js port).

---

## Main features

- JWT authentication with **Admin**, **Teacher**, and **Student** roles
- **Admin:** users, classrooms, subjects, teacher–class–subject mapping, student enrollments
- **Teacher:** create / update / delete assignments, draft → publish, review submissions, marks & feedback
- **Student:** view published assignments for enrolled classes, submit / update before deadline, view status / marks / feedback
- FluentValidation, structured API errors, Serilog logging, Swagger/OpenAPI
- EF Core migrations + seed data (no manual table creation)
- Unit tests for auth, authorization, and submission business rules

---

## Technology stack

| Layer | Technology |
|-------|------------|
| Frontend | Next.js 15, React, TypeScript, Tailwind CSS |
| Backend | ASP.NET Core 9 Web API, C#, FluentValidation, Serilog, Swagger |
| Database | PostgreSQL + EF Core |
| Auth | JWT + role-based authorization |
| Tests | xUnit, Moq, FluentAssertions |

---

## Architecture

**Backend** uses Clean Architecture with feature modules:

| Project | Responsibility |
|---------|----------------|
| `Domain` | Entities and enums |
| `Application` | Feature services, DTOs, validators, business rules |
| `Infrastructure` | EF Core, repositories, JWT, seed/migrations |
| `Api` | Controllers, middleware, DI/Swagger/CORS extensions |

**Frontend** uses thin App Router pages plus feature modules and shared reusable code:

- `app/` — routes only
- `features/` — screen logic (e.g. admin managers)
- `shared/` — reusable hooks (`useAsyncList`, `useCrudModal`), `DataTable`, `FormModal`, validation helpers
- `components/` — layout + UI primitives
- `lib/` — API client, types, formatters

---

## Project structure

```
AssignmentManagementSystem/
├── backend/
│   ├── AssignmentManagement.Api/
│   ├── AssignmentManagement.Application/
│   ├── AssignmentManagement.Domain/
│   ├── AssignmentManagement.Infrastructure/
│   ├── AssignmentManagement.Tests/
│   └── AssignmentManagementSystem.sln
├── frontend/
│   └── src/
│       ├── app/
│       ├── features/
│       ├── shared/
│       ├── components/
│       ├── context/
│       └── lib/
├── docker-compose.yml
├── .env.example
└── README.md
```

---

## Prerequisites

- .NET 9 SDK
- Node.js 20+ and npm
- PostgreSQL 16+ **or** Docker

---

## Database setup

### Option A — Docker

```bash
docker compose up -d
```

Starts Postgres on `localhost:5432` with database `assignment_management_db`.  
Demo DB username/password live in `docker-compose.yml` only (local evaluation defaults — **not** personal secrets). Use the matching connection string in `appsettings.json` when running the API against that container.

### Option B — Local PostgreSQL

1. Create database `assignment_management_db`
2. Update the connection string in `backend/AssignmentManagement.Api/appsettings.json`  
   or copy `appsettings.Development.json.example` → `appsettings.Development.json` (gitignored)

On API startup, migrations apply and demo data is seeded automatically.

---

## Environment configuration

Do **not** commit real production secrets.

Root `.env.example` shows **required variable names with placeholders only** (`YOUR_DB_PASSWORD`, etc.).

For **local Docker**, use the demo Postgres settings from `docker-compose.yml` with the matching connection string in `appsettings.json` so `dotnet run` works out of the box. Treat those as **local demo only**, not personal/production credentials. Root `.env.example` keeps placeholders only.

**Frontend**

```bash
# Linux / macOS
cp frontend/.env.example frontend/.env.local

# Windows PowerShell
Copy-Item frontend/.env.example frontend/.env.local
```

```
NEXT_PUBLIC_API_URL=http://localhost:5249/api
```

---

## Run the backend

```bash
cd backend/AssignmentManagement.Api
dotnet restore
dotnet run --launch-profile http
```

| Resource | URL |
|----------|-----|
| API | http://localhost:5249 |
| Swagger | http://localhost:5249/swagger |

---

## Run the frontend

```bash
cd frontend
npm install
npm run dev -- -p 3001
```

| Resource | URL |
|----------|-----|
| App | http://localhost:3001 |

CORS allows `http://localhost:3000` and `http://localhost:3001`.

---

## Run the tests

```bash
cd backend
dotnet test
```

---

## Demo credentials (application login)

Required for evaluators to test each role. These are **seeded demo users**, not personal accounts:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@school.com | Admin@123 |
| Teacher | teacher@school.com | Teacher@123 |
| Student | student@school.com | Student@123 |

**Seeded sample data**

- Class: `Grade 10 / A`
- Subjects: `Mathematics (MATH-101)`, `English (ENG-101)`
- Teacher assigned to both subjects for that class
- Students enrolled in the class
- Published assignment: `Algebra Basics`
- Draft assignment: `Essay Draft (Unpublished)`

---

## Assumptions

1. A classroom is identified by **Name + Section** (e.g. Grade 10 / A).
2. Teachers are linked via **Teacher–Class–Subject** rows; assignments are created against those rows.
3. Students see only **Published** assignments for classes they are enrolled in.
4. New assignments start as **Draft**; teachers (or admin) publish them.
5. Students may **update** a submission until the deadline; after the deadline, updates are blocked.
6. A first submission after the deadline is stored with status **Late**.
7. Marks must be between `0` and the assignment’s `MaximumMarks`.
8. Only the owning teacher (or Admin) can grade submissions for an assignment.
9. Answers are stored as plain text (no file uploads in this version).
10. Demo JWT key and DB password in `appsettings.json` are for **local evaluation only**.

---

## Known limitations

- No email / push notifications
- No file attachments for assignments or submissions
- No pagination / advanced filtering beyond role-scoped lists
- Soft-delete is not implemented (hard delete)
- Simple enrollment model (no academic-year history)

---

## Submission checklist

- [x] Frontend and backend included
- [x] Migrations + seed for database setup
- [x] Demo accounts for Admin, Teacher, and Student
- [x] README with run / test instructions
- [x] Role-based access enforced by the API
- [x] Business rules covered by unit tests
- [x] No real production secrets required (demo values only; see `.env.example`)
