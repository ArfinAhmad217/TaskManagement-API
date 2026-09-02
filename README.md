# Team Task Management System

A role-based, full-stack Task Management System built with **ASP.NET Core Web API (.NET 8)** and **React**. The application allows organizations to manage teams, assign tasks, track progress, collaborate via comments, and receive notifications on key task events.

---

## Live Demo

| Component | URL |
|---|---|
| **Frontend (React / Vercel)** | https://task-management-frontend-liard-phi.vercel.app |
| **Backend API (Render)** | https://taskmanagement-api-i234.onrender.com |
| **Swagger API Docs (Render)** | https://taskmanagement-api-i234.onrender.com/swagger/index.html |

> **Note:** The backend is hosted on Render's free tier. If the API has been idle for a while, the first request may take **30–50 seconds** to respond while the server "wakes up." This is expected behavior, not a bug.

---

## Sample Credentials

| Role | Email | Password |
|---|---|---|
| **Admin** (seeded automatically) | `admin@taskmanagement.com` | `Admin@123` |

Additional Manager/User accounts can be created via the `POST /api/User` endpoint (Admin only) or through registration (`POST /api/Auth/register`, which always creates a `User` role account).

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API (.NET 8) |
| ORM | Entity Framework Core |
| Database (Local Dev) | SQL Server |
| Database (Production) | PostgreSQL (Render) |
| Authentication | JWT (JSON Web Tokens) |
| Password Hashing | BCrypt |
| API Documentation | Swagger / OpenAPI |
| Frontend | React (Vite) |
| HTTP Client | Axios |
| Routing | React Router DOM |
| Frontend Hosting | Vercel |
| Backend Hosting | Render (Docker-based deployment) |

---

## User Roles & Capabilities

| Role | Capabilities |
|---|---|
| **Admin** | Manage teams, create Manager/User accounts, assign tasks to any team, full visibility over all tasks, delete tasks |
| **Manager** | Create and assign tasks to members of their team, manage team membership |
| **User** | View and update the status of tasks assigned to them, add comments |

---

## Core Features

- User registration and JWT-based login with token expiration handling
- Role-based access control (RBAC) enforced on every protected endpoint
- Team creation and management, including assigning users to teams
- Task creation, assignment, editing, deletion, and status tracking (`To Do`, `In Progress`, `Done`)
- Task filtering by status, priority, and deadline
- Comment section on each task for collaboration
- Mock in-app notifications triggered on:
  - Task assignment
  - Task status update
- Dashboard with task-status summary, priority breakdown, overdue count, and upcoming deadlines (role-aware — Admins see everything, Managers see their team's tasks, Users see only their assigned tasks)

---

## Project Structure

```
TaskManagement-API/              # Backend (.NET 8 Web API)
├── Controllers/                 # API endpoints
├── Services/                    # Business logic
├── Models/                      # EF Core entities
├── DTOs/                        # Request/response contracts
├── Data/                        # DbContext + DbSeeder
└── Program.cs                   # App configuration, DI, middleware

TaskManagement-Frontend/         # Frontend (React + Vite)
├── src/
│   ├── api/                     # Axios instance
│   ├── components/               # Sidebar, Layout, ProtectedRoute
│   ├── context/                  # AuthContext
│   ├── pages/                    # Login, Register, Dashboard, Tasks, TaskDetail, Teams, Notifications, Users
│   └── App.jsx                   # Route definitions
└── vercel.json                   # SPA rewrite rule for client-side routing
```

---

## Local Setup Instructions

### Prerequisites

- .NET 8 SDK
- Node.js (v18+) and npm
- SQL Server (local instance, e.g. SQL Express)
- Visual Studio 2022 (or any C# IDE) — optional but recommended

### Backend Setup

1. Clone the repository and open `TaskManagement-API` in Visual Studio.
2. Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=TaskManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
3. Apply database migrations (Package Manager Console):
   ```powershell
   Update-Database
   ```
   This creates the database schema and automatically seeds a default Admin account (see credentials above).
4. Run the project (`Ctrl+F5`). The API will start on a local port (e.g. `https://localhost:44301`).
5. Open Swagger at `https://localhost:44301/swagger/index.html` to explore and test endpoints.

### Frontend Setup

1. Navigate to the `TaskManagement-Frontend` folder.
2. Install dependencies:
   ```bash
   npm install
   ```
3. Create a `.env` file in the project root:
   ```
   VITE_API_URL=https://localhost:44301/api
   ```
4. Start the dev server:
   ```bash
   npm run dev
   ```
5. Open the app at `http://localhost:5173`.

---

## How to Test the API (Swagger)

The API is fully documented and testable via **Swagger UI**, available at `/swagger/index.html` on both the local and live (Render) URLs.

### Step-by-step testing flow

1. **Register or log in as Admin**
   ```
   POST /api/Auth/login
   {
     "email": "admin@taskmanagement.com",
     "password": "Admin@123"
   }
   ```
   Copy the `accessToken` from the response.

2. **Authorize in Swagger**
   Click the **Authorize** 🔒 button at the top of the Swagger page and enter:
   ```
   Bearer <your_access_token>
   ```

3. **Create supporting users** (optional — Admin only)
   ```
   POST /api/User
   {
     "fullName": "Manager Name",
     "email": "manager@test.com",
     "password": "test123",
     "role": "Manager"
   }
   ```

4. **Create a team**
   ```
   POST /api/Team
   {
     "name": "Development Team",
     "managerId": <manager_user_id>
   }
   ```

5. **Add a member to the team**
   ```
   POST /api/Team/{teamId}/members
   {
     "userId": <user_id>
   }
   ```

6. **Create a task** (Admin or Manager only)
   ```
   POST /api/Tasks
   {
     "title": "Fix Login Bug",
     "description": "Login validation issue",
     "priority": "High",
     "deadline": "2026-12-01T18:00:00.000Z",
     "teamId": <team_id>,
     "assignedToUserId": <user_id>
   }
   ```

7. **Update task status**
   ```
   PATCH /api/Tasks/{id}/status
   { "status": "InProgress" }
   ```

8. **Add a comment**
   ```
   POST /api/comments/task/{taskId}
   { "content": "Working on this now" }
   ```

9. **Check notifications** — log in as the assigned user and call:
   ```
   GET /api/Notifications
   ```

10. **View dashboard summary**
    ```
    GET /api/Dashboard/summary
    ```

### Verifying Role-Based Access Control (RBAC)

To confirm RBAC is enforced correctly, log in with a **User**-role account and attempt to:
- `POST /api/Team` → expect **403 Forbidden** (Team creation is Admin-only)
- `POST /api/Tasks` → expect **403 Forbidden** (Task creation is Admin/Manager-only)
- `GET /api/Tasks` → expect only tasks **assigned to that user**, not the full list

---

## API Endpoints Summary

| Module | Method | Endpoint | Access |
|---|---|---|---|
| Auth | POST | `/api/Auth/register` | Public |
| Auth | POST | `/api/Auth/login` | Public |
| Auth | GET | `/api/Auth/me` | Authenticated |
| Team | POST | `/api/Team` | Admin |
| Team | GET | `/api/Team` | Admin, Manager, User |
| Team | GET | `/api/Team/{id}` | Admin, Manager, User |
| Team | POST | `/api/Team/{teamId}/members` | Admin, Manager |
| Team | DELETE | `/api/Team/{teamId}/members/{userId}` | Admin, Manager |
| User | POST | `/api/User` | Admin |
| User | GET | `/api/User` | Admin, Manager |
| User | GET | `/api/User/{id}` | Admin, Manager, User |
| Tasks | POST | `/api/Tasks` | Admin, Manager |
| Tasks | GET | `/api/Tasks` | Authenticated (role-filtered) |
| Tasks | GET | `/api/Tasks/{id}` | Authenticated |
| Tasks | PUT | `/api/Tasks/{id}` | Admin, Manager |
| Tasks | PATCH | `/api/Tasks/{id}/status` | Authenticated |
| Tasks | DELETE | `/api/Tasks/{id}` | Admin |
| Comments | POST | `/api/comments/task/{taskId}` | Authenticated |
| Comments | GET | `/api/comments/task/{taskId}` | Authenticated |
| Notifications | GET | `/api/Notifications` | Authenticated |
| Notifications | PATCH | `/api/Notifications/{id}/read` | Authenticated |
| Dashboard | GET | `/api/Dashboard/summary` | Authenticated (role-aware) |

---

## Deployment Notes

- **Backend** is containerized with Docker and deployed on **Render**, using **PostgreSQL** as the production database (EF Core's provider is switched based on `IsProduction()` in `Program.cs`).
- **Frontend** is deployed on **Vercel**. A `vercel.json` rewrite rule is included so that client-side routes (e.g. `/tasks/5`) resolve correctly on direct navigation or page refresh.
- CORS is configured on the backend to explicitly allow the deployed frontend origin.

---

## Known Limitations

- Notifications are in-app (mocked) rather than sent via email, per the assignment's allowance for mock notifications.
- The Render free-tier backend spins down after inactivity, causing a cold-start delay on the first request.

---

## Author

**Arfin Ahmad**
