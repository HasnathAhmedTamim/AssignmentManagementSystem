using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Features.ClassRooms.DTOs;
using AssignmentManagement.Application.Features.ClassRooms.Interfaces;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Features.ClassRooms.Services;

public class ClassRoomService : IClassRoomService
{
    private readonly IClassRoomRepository _classRoomRepository;

    public ClassRoomService(IClassRoomRepository classRoomRepository)
    {
        _classRoomRepository = classRoomRepository;
    }

    public async Task<List<ClassRoomResponse>> GetAllAsync()
    {
        var classRooms = await _classRoomRepository.GetAllAsync();

        return classRooms.Select(MapToResponse).ToList();
    }

    public async Task<ClassRoomResponse?> GetByIdAsync(Guid id)
    {
        var classRoom = await _classRoomRepository.GetByIdAsync(id);

        if (classRoom == null)
            return null;

        return MapToResponse(classRoom);
    }

    public async Task<ClassRoomResponse> CreateAsync(CreateClassRoomRequest request)
    {
        if (await _classRoomRepository.ExistsByNameAndSectionAsync(request.Name, request.Section))
            throw new ConflictException("A classroom with this name and section already exists.");

        var classRoom = new ClassRoom
        {
            Name = request.Name,
            Section = request.Section,
            CreatedAt = DateTime.UtcNow
        };

        await _classRoomRepository.AddAsync(classRoom);
        await _classRoomRepository.SaveChangesAsync();

        return MapToResponse(classRoom);
    }

    public async Task UpdateAsync(Guid id, UpdateClassRoomRequest request)
    {
        var classRoom = await _classRoomRepository.GetByIdAsync(id);

        if (classRoom == null)
            throw new NotFoundException("Classroom not found.");

        if (await _classRoomRepository.ExistsByNameAndSectionAsync(request.Name, request.Section, id))
            throw new ConflictException("A classroom with this name and section already exists.");

        classRoom.Name = request.Name;
        classRoom.Section = request.Section;
        classRoom.UpdatedAt = DateTime.UtcNow;

        _classRoomRepository.Update(classRoom);
        await _classRoomRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var classRoom = await _classRoomRepository.GetByIdAsync(id);

        if (classRoom == null)
            throw new NotFoundException("Classroom not found.");

        _classRoomRepository.Delete(classRoom);
        await _classRoomRepository.SaveChangesAsync();
    }

    private static ClassRoomResponse MapToResponse(ClassRoom classRoom)
    {
        return new ClassRoomResponse
        {
            Id = classRoom.Id,
            Name = classRoom.Name,
            Section = classRoom.Section,
            CreatedAt = classRoom.CreatedAt
        };
    }
}
