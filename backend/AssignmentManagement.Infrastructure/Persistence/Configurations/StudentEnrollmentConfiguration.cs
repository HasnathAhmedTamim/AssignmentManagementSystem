using AssignmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssignmentManagement.Infrastructure.Persistence.Configurations;

public class StudentEnrollmentConfiguration : IEntityTypeConfiguration<StudentEnrollment>
{
    public void Configure(EntityTypeBuilder<StudentEnrollment> builder)
    {
        builder.ToTable("StudentEnrollments");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.StudentId, x.ClassRoomId })
            .IsUnique();

        builder.HasOne(x => x.Student)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ClassRoom)
            .WithMany(x => x.StudentEnrollments)
            .HasForeignKey(x => x.ClassRoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
