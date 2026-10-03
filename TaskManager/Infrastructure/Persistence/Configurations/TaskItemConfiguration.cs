using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Infrastructure.Persistence.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItemEntity>
{
    public void Configure(EntityTypeBuilder<TaskItemEntity> builder)
    {
        builder.ToTable("tasks");
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(task => task.IsCompleted)
            .IsRequired();

        builder.Property(task => task.CreatedAt)
            .IsRequired();

        builder.Property(task => task.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(task => task.CreatedAt);
    }
}
