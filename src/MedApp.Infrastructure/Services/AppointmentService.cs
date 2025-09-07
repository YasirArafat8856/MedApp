using MedApp.Application.Common;
using MedApp.Application.DTOs;
using MedApp.Application.Interfaces;
using MedApp.Domain.Entities;
using MedApp.Domain.Enums;
using MedApp.Infrastructure.Email;
using MedApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedApp.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly MedAppDbContext _db;
    private readonly IEmailSender _emailSender;
    public AppointmentService(MedAppDbContext db, IEmailSender emailSender) { _db = db; _emailSender = emailSender; }

    public async Task<PagedResult<AppointmentListItemDto>> ListAsync(
        string? search, int? doctorId, int? visitType, DateOnly? dateFrom, DateOnly? dateTo, int page, int pageSize, CancellationToken ct)
    {
        var q = _db.Appointments.AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var like = $"%{search.ToLower()}%";
            q = q.Where(a => a.Patient!.FullName.ToLower().Contains(like) || a.Doctor!.FullName.ToLower().Contains(like));
        }
        if (doctorId.HasValue) q = q.Where(a => a.DoctorId == doctorId);
        if (visitType.HasValue) q = q.Where(a => (int)a.VisitType == visitType);
        if (dateFrom.HasValue) q = q.Where(a => a.AppointmentDate >= dateFrom);
        if (dateTo.HasValue) q = q.Where(a => a.AppointmentDate <= dateTo);

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AppointmentListItemDto(
                a.Id, a.Patient!.FullName, a.Doctor!.FullName, a.AppointmentDate, a.VisitType, a.Diagnosis))
            .ToListAsync(ct);

        return new PagedResult<AppointmentListItemDto>(items, total, page, pageSize);
    }

    public async Task<AppointmentDto?> GetAsync(int id, CancellationToken ct)
    {
        var a = await _db.Appointments.AsNoTracking()
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .Include(x => x.Details).ThenInclude(d => d.Medicine)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a == null) return null;
        return new AppointmentDto(
            a.Id,
            a.PatientId,
            a.Patient!.FullName,
            a.DoctorId,
            a.Doctor!.FullName,
            a.AppointmentDate,
            a.VisitType,
            a.Notes,
            a.Diagnosis,
            a.Details.Select(d => new PrescriptionDetailDto(d.Id, d.MedicineId, d.Medicine!.Name, d.Dosage, d.StartDate, d.EndDate, d.Notes)).ToList()
        );
    }

    public async Task<int> CreateAsync(AppointmentCreateDto dto, CancellationToken ct)
    {
        // Basic validations
        if (!await _db.Patients.AnyAsync(p => p.Id == dto.PatientId, ct)) throw new ArgumentException("Invalid patient");
        if (!await _db.Doctors.AnyAsync(d => d.Id == dto.DoctorId, ct)) throw new ArgumentException("Invalid doctor");
        var medIds = dto.Details.Select(d => d.MedicineId).Distinct().ToList();
        var medCount = await _db.Medicines.CountAsync(m => medIds.Contains(m.Id), ct);
        if (medCount != medIds.Count) throw new ArgumentException("One or more medicines are invalid");

        var entity = new Appointment
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            AppointmentDate = dto.AppointmentDate,
            VisitType = dto.VisitType,
            Notes = dto.Notes,
            Diagnosis = dto.Diagnosis,
            Details = dto.Details.Select(d => new PrescriptionDetail
            {
                MedicineId = d.MedicineId,
                Dosage = d.Dosage,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Notes = d.Notes
            }).ToList()
        };
        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(int id, AppointmentUpdateDto dto, CancellationToken ct)
    {
        var a = await _db.Appointments
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a == null) throw new KeyNotFoundException("Appointment not found");

        a.PatientId = dto.PatientId;
        a.DoctorId = dto.DoctorId;
        a.AppointmentDate = dto.AppointmentDate;
        a.VisitType = dto.VisitType;
        a.Notes = dto.Notes;
        a.Diagnosis = dto.Diagnosis;

        // Sync details (simple replace for clarity)
        _db.PrescriptionDetails.RemoveRange(a.Details);
        a.Details.Clear();
        foreach (var d in dto.Details)
        {
            a.Details.Add(new PrescriptionDetail
            {
                MedicineId = d.MedicineId,
                Dosage = d.Dosage,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Notes = d.Notes
            });
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var a = await _db.Appointments.FindAsync([id], ct);
        if (a == null) return;
        _db.Appointments.Remove(a);
        await _db.SaveChangesAsync(ct);
    }

    // --- PDF & Email ---
    public async Task<byte[]> BuildPdfAsync(int id, CancellationToken ct)
    {
        try
        {
            var dto = await GetAsync(id, ct) ?? throw new KeyNotFoundException("Appointment not found");
            return Pdf.PdfGenerator.Build(dto);
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }

    public async Task SendEmailWithPdfAsync(int id, string toEmail, CancellationToken ct)
    {
        try
        {
            var bytes = await BuildPdfAsync(id, ct);
            await _emailSender.SendAsync(toEmail, "Prescription Report", "Please find the attached prescription report.", bytes, $"Prescription_{id}.pdf", ct);
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }
}
