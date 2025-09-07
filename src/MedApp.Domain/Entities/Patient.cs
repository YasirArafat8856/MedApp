using MedApp.Domain.Common;
namespace MedApp.Domain.Entities;
public class Patient : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
}
