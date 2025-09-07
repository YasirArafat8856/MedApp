using MedApp.Domain.Enums;
namespace MedApp.Application.DTOs;

public record LookupDto(int Id, string Name);

public record PrescriptionDetailDto(
    int? Id,
    int MedicineId,
    string MedicineName,
    string Dosage,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Notes
);

public record AppointmentListItemDto(
    int Id,
    string Patient,
    string Doctor,
    DateOnly AppointmentDate,
    VisitType VisitType,
    string? Diagnosis
);

public record AppointmentDto(
    int Id,
    int PatientId,
    string Patient,
    int DoctorId,
    string Doctor,
    DateOnly AppointmentDate,
    VisitType VisitType,
    string? Notes,
    string? Diagnosis,
    List<PrescriptionDetailDto> Details
);

public class AppointmentCreateDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public VisitType VisitType { get; set; }
    public string? Notes { get; set; }
    public string? Diagnosis { get; set; }
    public List<AppointmentCreateDetailDto> Details { get; set; } = new();
}
public class AppointmentCreateDetailDto
{
    public int MedicineId { get; set; }
    public string Dosage { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Notes { get; set; }
}

public class AppointmentUpdateDto : AppointmentCreateDto
{
    public int Id { get; set; }
}
