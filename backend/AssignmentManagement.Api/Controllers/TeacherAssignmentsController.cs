using System.Security.Claims;
using AssignmentManagement.Application.Features.TeacherAssignments.DTOs;
using AssignmentManagement.Application.Features.TeacherAssignments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagement.Api.Controllers;

[ApiController]
[Route("api/teacher-assignments")]
[Authorize]
public class TeacherAssignmentsController : ControllerBase
{
    private readonly ITeacherAssignmentService _service;

    public TeacherAssignmentsController(ITeacherAssignmentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> GetAll([FromQuery] Guid? teacherId)
    {
        if (User.IsInRole("Teacher") && !User.IsInRole("Admin"))
        {
            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentId, out var teacherGuid))
                return Unauthorized();

            return Ok(await _service.GetByTeacherIdAsync(teacherGuid));
        }

        if (teacherId.HasValue)
            return Ok(await _service.GetByTeacherIdAsync(teacherId.Value));

        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null)
            return NotFound(new { message = "Teacher assignment not found." });

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateTeacherAssignmentRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
