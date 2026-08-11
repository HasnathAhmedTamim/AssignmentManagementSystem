using AssignmentManagement.Application.Features.Submissions.DTOs;
using AssignmentManagement.Application.Features.Submissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpGet("assignment/{assignmentId:guid}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> GetByAssignment(Guid assignmentId)
    {
        return Ok(await _submissionService.GetByAssignmentIdAsync(assignmentId));
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMine()
    {
        return Ok(await _submissionService.GetMySubmissionsAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _submissionService.GetByIdAsync(id);
        if (item is null)
            return NotFound(new { message = "Submission not found." });

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Submit(CreateSubmissionRequest request)
    {
        var created = await _submissionService.SubmitAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Update(Guid id, UpdateSubmissionRequest request)
    {
        await _submissionService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPut("{id:guid}/grade")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Grade(Guid id, GradeSubmissionRequest request)
    {
        await _submissionService.GradeAsync(id, request);
        return NoContent();
    }
}
