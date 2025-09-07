using MedApp.Application.DTOs;
using MedApp.Application.Interfaces;
using MedApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedApp.Infrastructure.Services;

public class LookupService : ILookupService
{
    private readonly MedAppDbContext _db;
    public LookupService(MedAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<LookupDto>> DoctorsAsync(CancellationToken ct)
        => await _db.Doctors.AsNoTracking().OrderBy(x => x.FullName).Select(x => new LookupDto(x.Id, x.FullName)).ToListAsync(ct);
    public async Task<IReadOnlyList<LookupDto>> PatientsAsync(CancellationToken ct)
        => await _db.Patients.AsNoTracking().OrderBy(x => x.FullName).Select(x => new LookupDto(x.Id, x.FullName)).ToListAsync(ct);
    public async Task<IReadOnlyList<LookupDto>> MedicinesAsync(CancellationToken ct)
        => await _db.Medicines.AsNoTracking().OrderBy(x => x.Name).Select(x => new LookupDto(x.Id, x.Name)).ToListAsync(ct);
}
