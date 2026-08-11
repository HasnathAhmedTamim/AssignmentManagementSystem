using AssignmentManagement.Application.Features.ClassRooms.DTOs;
using AssignmentManagement.Application.Features.ClassRooms.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassRoomsController : ControllerBase
{
    private readonly IClassRoomService _classRoomService;

    public ClassRoomsController(IClassRoomService classRoomService)
    {
        _classRoomService = classRoomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _classRoomService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _classRoomService.GetByIdAsync(id);
        if (item is null)
            return NotFound(new { message = "Class room not found." });

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateClassRoomRequest request)
    {
        var created = await _classRoomService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateClassRoomRequest request)
    {
        await _classRoomService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _classRoomService.DeleteAsync(id);
        return NoContent();
    }
}
