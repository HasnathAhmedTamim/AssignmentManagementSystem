# CampusDesk Frontend

Next.js (App Router) + TypeScript + Tailwind client for the Assignment & Submission Management System.

## Setup

```bash
cd frontend
npm install
cp .env.example .env.local
npm run dev
```

App runs at [http://localhost:3000](http://localhost:3000).

API default: `http://localhost:5249/api` (from backend `launchSettings.json`). Override with `NEXT_PUBLIC_API_URL`.

## Scripts

- `npm run dev` — development server
- `npm run build` — production build
- `npm start` — serve production build

## Demo logins

- admin@school.com / Admin@123
- teacher@school.com / Teacher@123
- student@school.com / Student@123
