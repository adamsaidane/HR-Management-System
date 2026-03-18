# 🏢 HR Management System

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0+-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0+-239120?style=flat-square&logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework%20Core-8.0+-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=flat-square&logo=bootstrap&logoColor=white)
![License](https://img.shields.io/badge/License-Private-red?style=flat-square)

A comprehensive, enterprise-grade Human Resource Management System built with **ASP.NET Core**, **C#**, and **SQL Server**. Designed for large organisations to streamline employee management, recruitment, payroll, and all day-to-day HR operations from a single platform.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [System Architecture](#-system-architecture)
- [Installation](#-installation)
- [Database Setup](#-database-setup)
- [Configuration](#-configuration)
- [Usage Guide](#-usage-guide)
- [API Documentation](#-api-documentation)
- [Security](#-security)
- [Deployment](#-deployment)
- [Troubleshooting](#-troubleshooting)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 Overview

The **HR Management System** (HRMS) is a modern full-stack web application for managing the complete employee lifecycle. It covers everything from onboarding and payroll to recruitment pipelines and analytics — all secured behind a role-based access control layer.

---

## ✨ Features

### 👥 Employee Management
- Complete employee profiles with personal and contact information
- Employment history, contract tracking, and status management (active / inactive / on leave)
- Department and position assignment
- Emergency contact information and skill / certification tracking
- Document upload and management per employee

### 💰 Payroll Management
- Salary calculation, processing, and history
- Monthly payroll run with bonus allocation
- Benefits management (health insurance, retirement, etc.)
- Tax calculations and department-level salary analysis
- Exportable salary reports

### 🎯 Recruitment
- Job offer creation and lifecycle management
- Multi-stage candidate pipeline with status tracking
- Interview scheduling, evaluation forms, and feedback recording
- Hiring analytics and trends by month and department

### 📊 Analytics Dashboard (40+ views)
- Live counts: total employees, open job offers, monthly salary totals
- Charts: employees by department, status, gender, age range, contract type
- Salary evolution trends and recent promotions list
- Recruitment: candidates by stage, job offers by department, hiring trends
- Equipment status overview and bonuses by department
- Interview success rates and employee growth projections

### 📋 Document Management
- Document upload, categorisation, and versioning
- Access audit trails per document

### ⚙️ Equipment Management
- Full inventory tracking and assignment to employees
- Maintenance schedules and status monitoring
- Reports by equipment type and status

### 🔐 Security & Access Control
- Role-based access control (Admin, Manager, Employee)
- Department-level data filtering
- Audit logging across all sensitive operations
- Secure password policy enforcement

---

## 💻 Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core 8.0+ |
| **Language** | C# 12.0+ |
| **Database** | SQL Server 2019+ |
| **ORM** | Entity Framework Core 8 |
| **Frontend** | HTML5, CSS3, Bootstrap 5, JavaScript |
| **Authentication** | Cookie-based Authentication |
| **Authorization** | Role-Based Access Control (RBAC) |
| **Logging** | Serilog |
| **Charting** | Chart.js, Plotly.js |
| **Hosting** | IIS Express (dev) / Azure App Service (prod) |

---

## 🏗️ System Architecture

```
HR-Management-System/
│
├── Controllers/                  # MVC Controllers
│   ├── HomeController            # Dashboard & reports
│   ├── EmployeesController       # Employee CRUD
│   ├── DepartmentsController     # Department management
│   ├── SalariesController        # Payroll operations
│   ├── RecruitmentController     # Hiring process
│   ├── InterviewController       # Interview management
│   ├── PromotionsController      # Career progression
│   ├── EquipmentController       # Asset management
│   ├── AccountController         # Authentication
│   └── DocumentsController       # File management
│
├── Services/                     # Business Logic
│   ├── DashboardService
│   ├── EmployeeService
│   ├── DepartmentService
│   ├── SalaryService
│   ├── RecruitmentService
│   ├── PromotionService
│   ├── EquipmentService
│   ├── AccountService
│   └── DocumentService
│
├── Repositories/                 # Data Access Layer
│   ├── EmployeeRepository
│   ├── DepartmentRepository
│   ├── SalaryRepository
│   ├── CandidateRepository
│   ├── EquipmentRepository
│   └── DocumentRepository
│
├── Models/                       # EF Core Entities
│   ├── Employee, Department, Position
│   ├── Salary, Bonus, Benefit
│   ├── Candidate, JobOffer, Interview
│   ├── Equipment, EquipmentAssignment
│   └── Promotion, User, Document
│
├── ViewModels/                   # 30+ View Models
│   ├── DashboardViewModel
│   ├── EmployeeFormViewModel
│   ├── SalaryReportViewModel
│   ├── RecruitmentViewModel
│   └── ...
│
├── Enums/
│   ├── EmployeeStatus, ContractType
│   ├── EquipmentStatus, CandidateStatus
│   ├── JobOfferStatus, InterviewResult
│   └── UserRole
│
├── Views/
│   ├── Home/Index.cshtml         # Main dashboard
│   ├── Employees/                # List, Create, Edit, Details
│   ├── Salaries/                 # Salary management & reports
│   ├── Recruitment/              # Job offers & candidates
│   ├── Shared/                   # Layout, NavBar, Footer
│   └── Account/                  # Login, Register, ChangePassword
│
└── Data/
    └── AppDbContext              # EF Core context + migrations
```

---

## 🚀 Installation

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code
- Git

### Step 1 — Clone the repository

```bash
git clone https://github.com/adamsaidane/HR-Management-System.git
cd HR-Management-System
```

### Step 2 — Restore dependencies

```bash
dotnet restore
```

### Step 3 — Configure the database connection

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HRMS;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### Step 4 — Apply migrations

```bash
dotnet ef database update
```

### Step 5 — Run the application

```bash
dotnet run
```

The app will be available at **https://localhost:5001**

---

## 🗄️ Database Setup

### Create the database

```sql
CREATE DATABASE HRMS;
```

### EF Core migration commands

```bash
# Create a new migration
dotnet ef migrations add InitialCreate

# Apply all pending migrations
dotnet ef database update

# List migration history
dotnet ef migrations list
```

### Seed initial data

In `Program.cs`, seed the database on startup:

```csharp
var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
DbInitializer.Initialize(context);
```

### Key database tables

| Table | Description |
|---|---|
| `Users` | System users and login credentials |
| `Employees` | Core employee records |
| `Departments` | Organisation structure |
| `Positions` | Job positions and grades |
| `Salaries` | Salary records and history |
| `Bonuses` | Bonus allocations |
| `Benefits` | Employee benefits |
| `Candidates` | Job applicants |
| `JobOffers` | Active job postings |
| `Interviews` | Interview records and results |
| `Equipment` | Company assets |
| `EquipmentAssignments` | Asset-to-employee allocation |
| `Promotions` | Career advancement records |
| `Documents` | HR documents and files |

---

## ⚙️ Configuration

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=HRMS;..."
  },
  "Authentication": {
    "CookieName": "HRMS.Auth",
    "ExpireTimeSpan": 480
  },
  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "FileUpload": {
    "MaxFileSize": 5242880,
    "AllowedExtensions": [".pdf", ".docx", ".doc", ".xlsx"]
  }
}
```

### Service registration (`Program.cs`)

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ISalaryService, SalaryService>();
builder.Services.AddScoped<IRecruitmentService, RecruitmentService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();
```

---

## 📖 Usage Guide

### Administrator workflow

1. Log in with admin credentials at `/Account/Login`
2. View the main dashboard for live statistics and charts
3. Manage employees under **Employees → Add Employee**
4. Process payroll under **Salaries**
5. Track open roles and candidates under **Recruitment**

### Adding an employee

1. Navigate to **Employees**
2. Click **Add Employee**
3. Complete the employee form (personal info, department, position)
4. Save — then assign salary and benefits under **Salaries**

### Running payroll

1. Go to **Salaries**
2. Review and update individual salary records as needed
3. Add bonuses and assign benefits
4. Generate and export the payroll report

### Managing the recruitment pipeline

1. Create a job offer under **Recruitment → Job Offers**
2. Review incoming candidate applications
3. Schedule interviews and record feedback
4. Update candidate status through each stage
5. On hire, convert the candidate record into a new employee

---

## 📌 API Documentation

This is an MVC application. Below are the key controller actions available via HTTP.

### `EmployeesController`

```csharp
// GET  /Employees
public async Task<IActionResult> Index(string searchString, int? departmentId)

// GET  /Employees/Details/{id}
public async Task<IActionResult> Details(int id)

// GET  /Employees/Create          [AdminRH only]
// POST /Employees/Create
public async Task<IActionResult> Create(EmployeeFormViewModel model)
```

### `SalariesController`

```csharp
// GET  /Salaries
public async Task<IActionResult> Index()

// GET  /Salaries/EmployeeSalary/{employeeId}
public async Task<IActionResult> EmployeeSalary(int employeeId)

// POST /Salaries/UpdateSalary
public async Task<IActionResult> UpdateSalary(int employeeId, decimal newSalary)

// POST /Salaries/AddBonus
public async Task<IActionResult> AddBonus(Bonus bonus)
```

### `RecruitmentController`

```csharp
// GET  /Recruitment/JobOffers
public async Task<IActionResult> JobOffers()

// POST /Recruitment/CreateJobOffer
public async Task<IActionResult> CreateJobOffer(JobOfferFormViewModel model)

// GET  /Recruitment/Candidates
public async Task<IActionResult> Candidates()

// POST /Interview/Create
public async Task<IActionResult> ScheduleInterview(Interview interview)
```

---

## 🔐 Security

### Authentication

- Cookie-based authentication with configurable session timeout
- Password hashing with bcrypt
- Automatic redirect to login on session expiry

### Authorization

- Three built-in roles: `AdminRH`, `Manager`, `Employee`
- Department-level data filtering enforced at the service layer
- Sensitive actions protected with `[Authorize(Roles = "AdminRH")]`

### Password policy

```csharp
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
});
```

### Best practices applied

- HTTPS enforced on all connections
- CSRF token validation on all POST forms
- Input validation and sanitisation throughout
- Full audit logging for sensitive operations

---

## 🚀 Deployment

### Azure App Service

```bash
# Publish a release build
dotnet publish -c Release -o ./publish

# Create an App Service plan
az appservice plan create \
  --name hrms-plan \
  --resource-group myResourceGroup \
  --sku B1 --is-linux

# Deploy the zip
az webapp deployment source config-zip \
  --resource-group myResourceGroup \
  --name hrms-app \
  --src publish.zip
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=builder /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "HRMS.dll"]
```

```bash
docker build -t hrms:latest .
docker run -p 80:80 hrms:latest
```

---

## 🐛 Troubleshooting

### Migration errors

```bash
# Remove the last migration
dotnet ef migrations remove

# Reset and re-apply the database
dotnet ef database drop --force
dotnet ef database update
```

### Connection string issues

```bash
# Test SQL Server connectivity
sqlcmd -S localhost -U sa -P YourPassword
```

### Build errors

```bash
# Clean and rebuild
dotnet clean
dotnet build
```

---

## 🤝 Contributing

Contributions are welcome!

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m "feat: describe your change"`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Open a Pull Request against `master`

Please include tests for any new functionality and follow the existing code style.

---

## 📚 Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [SQL Server Documentation](https://docs.microsoft.com/sql/sql-server)
