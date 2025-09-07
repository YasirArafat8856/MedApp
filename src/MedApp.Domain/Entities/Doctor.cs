using MedApp.Domain.Common;
namespace MedApp.Domain.Entities;
public class Doctor : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string? Specialty { get; set; }
    public bool IsActive { get; set; } = true;
}
