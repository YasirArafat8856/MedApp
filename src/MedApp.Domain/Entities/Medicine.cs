using MedApp.Domain.Common;
namespace MedApp.Domain.Entities;
public class Medicine : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Unit { get; set; } // e.g., "mg", "tablet"
    public bool IsActive { get; set; } = true;
}
