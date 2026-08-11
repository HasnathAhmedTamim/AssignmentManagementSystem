using AssignmentManagement.Application.Features.Enrollments.DTOs;
using AssignmentManagement.Application.Features.Enrollments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] Guid? studentId)
    {
        if (studentId.HasValue)
            return Ok(await _enrollmentService.GetByStudentIdAsync(studentId.Value));

        return Ok(await _enrollmentService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _enrollmentService.GetByIdAsync(id);
        if (item is null)
            return NotFound(new { message = "Enrollment not found." });

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateEnrollmentRequest request)
    {
        var created = await _enrollmentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _enrollmentService.DeleteAsync(id);
        return NoContent();
    }
}
