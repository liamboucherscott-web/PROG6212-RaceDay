# RaceDay - PROG6212 Portfolio of Evidence

## Live Deployment Links
- **Frontend (MVC Application):** [https://raceday-api-liam-gnffh4gzanhebkh9.centralindia-01.azurewebsites.net](https://raceday-api-liam-gnffh4gzanhebkh9.centralindia-01.azurewebsites.net)
- **Backend (RESTful API):** [https://raceday-api-liam104-amcsgffjasfjfgf8.centralindia-01.azurewebsites.net](https://raceday-api-liam104-amcsgffjasfjfgf8.centralindia-01.azurewebsites.net)

*(Note: The application is hosted on Azure's Free Tier. Please open the Backend API URL first and give it 30-60 seconds to warm up from a cold start, then open the Frontend URL.)*

---

## Video Walkthrough
[Watch the Video Walkthrough on YouTube](https://youtu.be/bs2I2O4bS6k)

---

## System Overview
**RaceDay** is a full-stack event management platform for the South African road running, walking, and cycling community. It allows Event Organisers to create and manage events, and Participants to browse and enrol in them. 

The system follows a decoupled architecture:
- **Part 2 (Backend):** ASP.NET Core Web API (C#) with Entity Framework Core and Swagger. Handles all business logic, authentication, and Azure Blob Storage.
- **Part 3 (Frontend):** ASP.NET Core MVC application. It consumes the RESTful API using HttpClient and handles session-based authentication for the user interface. 

### Roles and Features
1. **Organiser:**
   - Dashboard with event summaries and enrolment counts.
   - Create, edit, and delete events (including uploading event banner images to Azure Blob Storage).
   - Manage categories (age/distance) per event.
   - View enrolments and capture final results (finish times and positions).
2. **Participant:**
   - Browse upcoming events and filter by type.
   - Enrol in specific event categories.
   - View personal enrolment history and race results.
   - Update personal profiles and upload profile pictures.

*Role-based access is strictly enforced at the API level and reflected in the MVC interface.*

---

## Database Note (Important for Grader)
For the live Azure deployment, the API is configured to use **Entity Framework Core In-Memory Database**. This was chosen intentionally to ensure the application remains fully functional, fast, and stable on the Azure Free Tier without incurring SQL Server hosting costs or timing out during the grading process.

For **Part 1 compliance**, the complete SQL Server script (`docs/RaceDayDB.sql`) is still provided in the repository and can be run locally in SSMS. The Entity Framework models and the SQL script match the ERD exactly.

---

## Part 1: System Planning and Database
The `/docs` folder contains all planning artifacts as required:
- **ERD:** `docs/ERD.png` (Shows 7 entities: Role, EventType, User, Event, Category, Enrolment, Result).
- **API Endpoint Plan:** `docs/api-endpoint-plan.md` (28 endpoints across 6 resource groups).
- **SQL Script:** `docs/RaceDayDB.sql` (Creates and seeds the full relational database).

**How to run the SQL Script locally:**
1. Open `docs/RaceDayDB.sql` in SQL Server Management Studio (SSMS).
2. Connect to a local SQL Server instance.
3. Press F5 to execute. It will force-drop and recreate the RaceDayDB database with seed data.

---

## Part 2 & 3: How to Run Locally (Docker)

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.
- .NET 10 SDK (if running without Docker).

### Running with Docker (Recommended)
The application is fully containerised. To build and run the API container, use the following commands in the root directory:

docker build -t raceday-api -f RaceDay.Api/Dockerfile .
docker run -d -p 8080:8080 -p 8081:8081 --name raceday-container raceday-api

The API will be available at http://localhost:8080 and Swagger at http://localhost:8080/swagger.

### Running Locally (Visual Studio / CLI)
1. Open the terminal in the root folder.
2. Run the API: `dotnet run --project RaceDay.Api`
3. Run the MVC App: `dotnet run --project RaceDay.Mvc`
4. Navigate to the localhost URL provided in the terminal for the MVC app.

---

## Azure Blob Storage Integration
As per Part 3 requirements, Azure Blob Storage is integrated directly into the Part 2 API, not the MVC frontend. 
- The API handles the file upload via IFormFile.
- The MVC app sends a multipart/form-data request to the API.
- Organisers can upload event banners, and Participants can upload profile pictures. The API returns the secure Blob URL, which the MVC app stores and displays.

---

## CI/CD Status

Here is the screenshot of the GitHub Actions build passing:

<img width="1436" height="636" alt="Screenshot 2026-10-19 131038" src="https://github.com/user-attachments/assets/41389ef3-1096-4e9d-88b8-e89c44abcd4d" />

### Note on CI/CD:
GitHub Actions is disabled at the organization level on EMGPPT. To prove the workflow runs successfully, the repository was mirrored to a personal account. The workflow at `.github/workflows/validate-docs.yml` builds the project, runs unit tests, and validates the `/docs` folder.
- **Mirrored Repo:** [https://github.com/liamboucherscott-web/PROG6212-RaceDay](https://github.com/liamboucherscott-web/PROG6212-RaceDay)

---

## Tools Used
- **Languages:** C#, HTML/CSS, SQL
- **Frameworks:** ASP.NET Core Web API, ASP.NET Core MVC, Entity Framework Core
- **Cloud:** Microsoft Azure (App Services, Blob Storage)
- **Containerisation:** Docker
- **Database:** Entity Framework Core (In-Memory for Azure) / SQL Server (Local)
- **DevOps:** GitHub Actions
- **Documentation:** draw.io (ERD), Markdown

---
*AI Usage Disclosure: AI tools were used for debugging Azure deployment version mismatches and generating this README structure. All code logic, system design, and database architecture were implemented by the student.*