using AssignmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssignmentManagement.Infrastructure.Persistence.Configurations;

public class TeacherClassSubjectConfiguration : IEntityTypeConfiguration<TeacherClassSubject>
{
    public void Configure(EntityTypeBuilder<TeacherClassSubject> builder)
    {
        builder.ToTable("TeacherClassSubjects");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.TeacherId, x.ClassRoomId, x.SubjectId })
            .IsUnique();

        builder.HasOne(x => x.Teacher)
            .WithMany(x => x.TeachingAssignments)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClassRoom)
            .WithMany(x => x.TeacherClassSubjects)
            .HasForeignKey(x => x.ClassRoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Subject)
            .WithMany(x => x.TeacherClassSubjects)
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
