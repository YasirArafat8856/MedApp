# Medical Appointment System (Clean Architecture, .NET 8 + EF Core + SQL Server)

This repo contains a **working .NET 8 Web API** that satisfies the interview requirements:

- Master entity **Appointment** with detail grid **PrescriptionDetail**.
- CRUD endpoints for appointments (create with details, update with details, delete).
- Server-side list endpoint with **search** (patient or doctor), **filters** (doctor, visit type), **pagination**.
- Lookup endpoints for **Patients**, **Doctors**, **Medicines** (for dropdowns).
- **PDF** download for an appointment (QuestPDF).
- **Bonus**: Send email with PDF attachment (MailKit).
- **Clean Architecture** layout: Domain, Application, Infrastructure, API.
- **EF Core** (SQL Server). Optional **stored procedures** included in `/src/MedApp.Infrastructure/Database/StoredProcedures.sql`.

> NOTE: This solution seeds a few Patients/Doctors/Medicines for quick testing.

## Solution structure

```
MedApp.sln
src/
  MedApp.Domain/           -- Entities + Enums
  MedApp.Application/      -- DTOs + Interfaces + PagedResult
  MedApp.Infrastructure/   -- DbContext + Repos + Services + PDF + Email
  MedApp.API/              -- Controllers + Program
```

## Run locally (Visual Studio)

1. Open **MedApp.sln** in Visual Studio 2022 (v17+).
2. Ensure SQL Server is running and adjust `ConnectionStrings:DefaultConnection` in `src/MedApp.API/appsettings.json` if needed.
3. **Create database** via EF Core:
   - Open **Package Manager Console** and set `Default project` = `MedApp.Infrastructure`.
   - Run:
     ```powershell
     Add-Migration InitialCreate
     Update-Database
     ```
4. (Optional) Execute the stored procedures script:
   - Open the DB in SSMS and run: `/src/MedApp.Infrastructure/Database/StoredProcedures.sql`.
5. Set **MedApp.API** as startup project and **Run** (F5). Swagger will open.

## Important endpoints

- `GET /api/appointments` — list with **search/filter/pagination**
  - query: `search`, `doctorId`, `visitType` (0=First, 1=FollowUp), `dateFrom`, `dateTo`, `page`, `pageSize`
- `GET /api/appointments/{id}` — get one with details
- `POST /api/appointments` — create with details
- `PUT /api/appointments/{id}` — update with details (replace style)
- `DELETE /api/appointments/{id}` — delete
- `GET /api/appointments/{id}/pdf` — **download** Prescription PDF
- `POST /api/appointments/{id}/send-email?to=someone@example.com` — **bonus** email with PDF

- Dropdowns for the master/detail UI:
  - `GET /api/lookups/patients`
  - `GET /api/lookups/doctors`
  - `GET /api/lookups/medicines`

## Sample JSON (POST /api/appointments)

```json
{
  "patientId": 1,
  "doctorId": 1,
  "appointmentDate": "2025-08-20",
  "visitType": 0,
  "notes": "Take care",
  "diagnosis": "Fever",
  "details": [
    { "medicineId": 1, "dosage": "500mg 2x/day", "startDate": "2025-08-20", "endDate": "2025-08-25", "notes": "After meals" },
    { "medicineId": 2, "dosage": "850mg 1x/day", "startDate": "2025-08-21", "endDate": "2025-09-21", "notes": "Morning" }
  ]
}
```

## Notes & decisions

- `DateOnly` is used for dates; EF Core conversions are configured.
- `Update` replaces existing details for simplicity and reliability.
- PDF is produced with **QuestPDF** (no license needed for this use).
- Email uses **localhost:25**. Configure SMTP for your environment if you want to actually send.
- The list endpoint is EF LINQ for simplicity. If your reviewer insists on stored procedures, use the provided SQL (and you can easily call it with `FromSqlRaw` / `ExecuteSqlRaw` in the Infrastructure layer).

## Angular (coming next)

The API is ready for an Angular client with:
- Master form (Appointment create/edit).
- Inline editable **detail grid** for prescriptions.
- Master **grid** with search, filter, pagination, and actions (Edit, Delete, Download PDF).

Good luck on the interview!
