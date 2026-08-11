using AssignmentManagement.Application.Features.Assignments.DTOs;
using AssignmentManagement.Application.Features.Assignments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _assignmentService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _assignmentService.GetByIdAsync(id);
        if (item is null)
            return NotFound(new { message = "Assignment not found." });

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Create(CreateAssignmentRequest request)
    {
        var created = await _assignmentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateAssignmentRequest request)
    {
        await _assignmentService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Publish(Guid id)
    {
        await _assignmentService.PublishAsync(id);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _assignmentService.DeleteAsync(id);
        return NoContent();
    }
}
