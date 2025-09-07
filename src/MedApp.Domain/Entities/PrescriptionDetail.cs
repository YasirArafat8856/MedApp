using MedApp.Domain.Common;
namespace MedApp.Domain.Entities;
public class PrescriptionDetail : BaseEntity
{
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public int MedicineId { get; set; }
    public Medicine? Medicine { get; set; }

    public string Dosage { get; set; } = default!; // e.g., "500mg 2x/day"
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Notes { get; set; }
}
