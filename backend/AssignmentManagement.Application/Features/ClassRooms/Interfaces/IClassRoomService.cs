using AssignmentManagement.Application.Features.ClassRooms.DTOs;

namespace AssignmentManagement.Application.Features.ClassRooms.Interfaces;

public interface IClassRoomService
{
    Task<List<ClassRoomResponse>> GetAllAsync();

    Task<ClassRoomResponse?> GetByIdAsync(Guid id);

    Task<ClassRoomResponse> CreateAsync(CreateClassRoomRequest request);

    Task UpdateAsync(Guid id, UpdateClassRoomRequest request);

    Task DeleteAsync(Guid id);
}
