using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;
using AssignmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Infrastructure.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var admin = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FullName = "System Admin",
            Email = "admin@school.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = Role.Admin,
            IsActive = true,
            CreatedAt = now
        };

        var teacher = new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FullName = "John Teacher",
            Email = "teacher@school.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
            Role = Role.Teacher,
            IsActive = true,
            CreatedAt = now
        };

        var student = new User
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            FullName = "Alex Student",
            Email = "student@school.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
            Role = Role.Student,
            IsActive = true,
            CreatedAt = now
        };

        var student2 = new User
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            FullName = "Sam Student",
            Email = "student2@school.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
            Role = Role.Student,
            IsActive = true,
            CreatedAt = now
        };

        await context.Users.AddRangeAsync(admin, teacher, student, student2);

        var classRoom = new ClassRoom
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Name = "Grade 10",
            Section = "A",
            CreatedAt = now
        };

        await context.ClassRooms.AddAsync(classRoom);

        var math = new Subject
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Name = "Mathematics",
            Code = "MATH-101",
            CreatedAt = now
        };

        var english = new Subject
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Name = "English",
            Code = "ENG-101",
            CreatedAt = now
        };

        await context.Subjects.AddRangeAsync(math, english);

        var teacherAssignment = new TeacherClassSubject
        {
            Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            TeacherId = teacher.Id,
            ClassRoomId = classRoom.Id,
            SubjectId = math.Id,
            CreatedAt = now
        };

        var teacherAssignmentEnglish = new TeacherClassSubject
        {
            Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            TeacherId = teacher.Id,
            ClassRoomId = classRoom.Id,
            SubjectId = english.Id,
            CreatedAt = now
        };

        await context.TeacherClassSubjects.AddRangeAsync(teacherAssignment, teacherAssignmentEnglish);

        await context.StudentEnrollments.AddRangeAsync(
            new StudentEnrollment
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff1"),
                StudentId = student.Id,
                ClassRoomId = classRoom.Id,
                CreatedAt = now
            },
            new StudentEnrollment
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff2"),
                StudentId = student2.Id,
                ClassRoomId = classRoom.Id,
                CreatedAt = now
            });

        var publishedAssignment = new Assignment
        {
            Id = Guid.Parse("12121212-1212-1212-1212-121212121212"),
            TeacherClassSubjectId = teacherAssignment.Id,
            Title = "Algebra Basics",
            Description = "Solve the attached algebra problems and explain your steps clearly.",
            Deadline = now.AddDays(14),
            MaximumMarks = 100,
            Status = AssignmentStatus.Published,
            CreatedAt = now,
            CreatedBy = teacher.Id
        };

        var draftAssignment = new Assignment
        {
            Id = Guid.Parse("13131313-1313-1313-1313-131313131313"),
            TeacherClassSubjectId = teacherAssignmentEnglish.Id,
            Title = "Essay Draft (Unpublished)",
            Description = "Write a short essay on your favorite book. This remains a draft until published.",
            Deadline = now.AddDays(21),
            MaximumMarks = 50,
            Status = AssignmentStatus.Draft,
            CreatedAt = now,
            CreatedBy = teacher.Id
        };

        await context.Assignments.AddRangeAsync(publishedAssignment, draftAssignment);
        await context.SaveChangesAsync();
    }
}
