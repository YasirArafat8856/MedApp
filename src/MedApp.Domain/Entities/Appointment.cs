using MedApp.Domain.Common;
using MedApp.Domain.Enums;
namespace MedApp.Domain.Entities;
public class Appointment : BaseEntity
{
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public DateOnly AppointmentDate { get; set; }
    public VisitType VisitType { get; set; }

    public string? Notes { get; set; }
    public string? Diagnosis { get; set; }

    public ICollection<PrescriptionDetail> Details { get; set; } = new List<PrescriptionDetail>();
}
