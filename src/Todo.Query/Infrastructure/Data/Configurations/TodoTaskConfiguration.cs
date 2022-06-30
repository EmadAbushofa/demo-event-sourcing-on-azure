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
            builder.Property(e => e.UserId).HasMaxLength(128);
            builder.Property(e => e.DueDate).HasColumnType("Date");
            builder.Property(e => e.Note).HasMaxLength(1000);

            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.DueDate);
            builder.HasIndex(e => e.IsCompleted);
            builder.HasIndex(e => new { e.UserId, e.Title, e.IsCompleted });
        }
    }
}
