-- Optional stored procedures (preferred). Run manually after creating DB.

-- Table type for bulk details
IF TYPE_ID(N'dbo.PrescriptionDetailType') IS NULL
    CREATE TYPE dbo.PrescriptionDetailType AS TABLE
    (
        MedicineId INT NOT NULL,
        Dosage NVARCHAR(100) NOT NULL,
        StartDate DATE NOT NULL,
        EndDate DATE NULL,
        Notes NVARCHAR(200) NULL
    );
GO

-- Create
IF OBJECT_ID('dbo.sp_Appointment_Create') IS NOT NULL DROP PROCEDURE dbo.sp_Appointment_Create;
GO
CREATE PROCEDURE dbo.sp_Appointment_Create
    @PatientId INT,
    @DoctorId INT,
    @AppointmentDate DATE,
    @VisitType INT,
    @Notes NVARCHAR(400) = NULL,
    @Diagnosis NVARCHAR(400) = NULL,
    @Details dbo.PrescriptionDetailType READONLY,
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRAN;
    INSERT INTO Appointments(PatientId, DoctorId, AppointmentDate, VisitType, Notes, Diagnosis)
    VALUES(@PatientId, @DoctorId, @AppointmentDate, @VisitType, @Notes, @Diagnosis);

    SET @NewId = SCOPE_IDENTITY();

    INSERT INTO PrescriptionDetails(AppointmentId, MedicineId, Dosage, StartDate, EndDate, Notes)
    SELECT @NewId, MedicineId, Dosage, StartDate, EndDate, Notes FROM @Details;

    COMMIT;
END
GO

-- Update (replace details)
IF OBJECT_ID('dbo.sp_Appointment_Update') IS NOT NULL DROP PROCEDURE dbo.sp_Appointment_Update;
GO
CREATE PROCEDURE dbo.sp_Appointment_Update
    @Id INT,
    @PatientId INT,
    @DoctorId INT,
    @AppointmentDate DATE,
    @VisitType INT,
    @Notes NVARCHAR(400) = NULL,
    @Diagnosis NVARCHAR(400) = NULL,
    @Details dbo.PrescriptionDetailType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRAN;
    UPDATE Appointments
        SET PatientId = @PatientId,
            DoctorId = @DoctorId,
            AppointmentDate = @AppointmentDate,
            VisitType = @VisitType,
            Notes = @Notes,
            Diagnosis = @Diagnosis
    WHERE Id = @Id;

    DELETE FROM PrescriptionDetails WHERE AppointmentId = @Id;
    INSERT INTO PrescriptionDetails(AppointmentId, MedicineId, Dosage, StartDate, EndDate, Notes)
    SELECT @Id, MedicineId, Dosage, StartDate, EndDate, Notes FROM @Details;
    COMMIT;
END
GO

-- List with pagination/search/filter
IF OBJECT_ID('dbo.sp_Appointment_List') IS NOT NULL DROP PROCEDURE dbo.sp_Appointment_List;
GO
CREATE PROCEDURE dbo.sp_Appointment_List
    @Search NVARCHAR(100) = NULL,
    @DoctorId INT = NULL,
    @VisitType INT = NULL,
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL,
    @Page INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@Page-1) * @PageSize;

    ;WITH A AS (
        SELECT a.Id, p.FullName AS Patient, d.FullName AS Doctor, a.AppointmentDate, a.VisitType, a.Diagnosis
        FROM Appointments a
        JOIN Patients p ON a.PatientId = p.Id
        JOIN Doctors d ON a.DoctorId = d.Id
        WHERE (@DoctorId IS NULL OR a.DoctorId = @DoctorId)
          AND (@VisitType IS NULL OR a.VisitType = @VisitType)
          AND (@DateFrom IS NULL OR a.AppointmentDate >= @DateFrom)
          AND (@DateTo IS NULL OR a.AppointmentDate <= @DateTo)
          AND (
                @Search IS NULL OR
                p.FullName LIKE '%' + @Search + '%' OR
                d.FullName LIKE '%' + @Search + '%'
          )
    )
    SELECT * FROM A
    ORDER BY AppointmentDate DESC, Id
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS Total FROM A;
END
GO
