using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Query.Entities;

namespace Todo.Query.Infrastructure.Data.Configurations
{
    public class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
    {
        public void Configure(EntityTypeBuilder<TodoTask> builder)
        {
            builder.HasKey(e => e.Id).IsClustered(false);

            builder.Property<int>("ClusterId").ValueGeneratedOnAdd();
            builder.HasIndex("ClusterId").IsClustered();

            builder.Property(e => e.Title).HasMaxLength(128);
            builder.Property(e => e.NormalizedTitle).HasMaxLength(148);
            builder.Property(e => e.UserId).HasMaxLength(128);
            builder.Property(e => e.Note).HasMaxLength(1000);

            builder.Property(e => e.DueDate)
                   .HasConversion(
                        dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                        dateTime => DateOnly.FromDateTime(dateTime)
                    ).HasColumnType("date");

            builder.HasIndex(e => e.DueDate);

            builder.HasIndex(e => new { e.UserId, e.NormalizedTitle, e.IsCompleted })
                .HasFilter("[IsCompleted] = 0")
                .IsUnique();
        }
    }
}
