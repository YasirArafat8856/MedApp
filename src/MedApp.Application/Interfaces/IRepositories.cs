using MedApp.Domain.Entities;

namespace MedApp.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> Query();
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IAppointmentService
{
    Task<MedApp.Application.Common.PagedResult<MedApp.Application.DTOs.AppointmentListItemDto>> ListAsync(
        string? search, int? doctorId, int? visitType, DateOnly? dateFrom, DateOnly? dateTo, int page, int pageSize, CancellationToken ct);

    Task<MedApp.Application.DTOs.AppointmentDto?> GetAsync(int id, CancellationToken ct);
    Task<int> CreateAsync(MedApp.Application.DTOs.AppointmentCreateDto dto, CancellationToken ct);
    Task UpdateAsync(int id, MedApp.Application.DTOs.AppointmentUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);

    Task<byte[]> BuildPdfAsync(int id, CancellationToken ct);
    Task SendEmailWithPdfAsync(int id, string toEmail, CancellationToken ct);
}

public interface ILookupService
{
    Task<IReadOnlyList<MedApp.Application.DTOs.LookupDto>> PatientsAsync(CancellationToken ct);
    Task<IReadOnlyList<MedApp.Application.DTOs.LookupDto>> DoctorsAsync(CancellationToken ct);
    Task<IReadOnlyList<MedApp.Application.DTOs.LookupDto>> MedicinesAsync(CancellationToken ct);
}
