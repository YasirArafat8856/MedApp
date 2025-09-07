using MedApp.Application.Common;
using MedApp.Application.DTOs;
using MedApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _svc;
    public AppointmentsController(IAppointmentService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<PagedResult<AppointmentListItemDto>>> List(
        [FromQuery] string? search,
        [FromQuery] int? doctorId,
        [FromQuery] int? visitType,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var data = await _svc.ListAsync(search, doctorId, visitType, dateFrom, dateTo, page, pageSize, ct);
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDto>> Get(int id, CancellationToken ct)
    {
        var data = await _svc.GetAsync(id, ct);
        return data is null ? NotFound() : Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] AppointmentCreateDto dto, CancellationToken ct)
    {
        var id = await _svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AppointmentUpdateDto dto, CancellationToken ct)
    {
        if (dto.Id != 0 && dto.Id != id) return BadRequest("Mismatched id");
        await _svc.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{id:int}/pdf")]
    public async Task<IActionResult> DownloadPdf(int id, CancellationToken ct)
    {
        var bytes = await _svc.BuildPdfAsync(id, ct);
        return File(bytes, "application/pdf", $"Prescription_{id}.pdf");
    }

    [HttpPost("{id:int}/send-email")]
    public async Task<IActionResult> SendEmail(int id, [FromQuery] string to, CancellationToken ct)
    {
        await _svc.SendEmailWithPdfAsync(id, to, ct);
        return Ok(new { sent = true });
    }
}
