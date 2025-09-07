using MedApp.Application.DTOs;
using MedApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupsController : ControllerBase
{
    private readonly ILookupService _svc;
    public LookupsController(ILookupService svc) => _svc = svc;

    [HttpGet("patients")]
    public async Task<ActionResult<IReadOnlyList<LookupDto>>> Patients(CancellationToken ct)
        => Ok(await _svc.PatientsAsync(ct));

    [HttpGet("doctors")]
    public async Task<ActionResult<IReadOnlyList<LookupDto>>> Doctors(CancellationToken ct)
        => Ok(await _svc.DoctorsAsync(ct));

    [HttpGet("medicines")]
    public async Task<ActionResult<IReadOnlyList<LookupDto>>> Medicines(CancellationToken ct)
        => Ok(await _svc.MedicinesAsync(ct));
}
