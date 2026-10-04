using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Desktop.Data.Entities;

namespace TaskManager.Desktop.Data.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    private const int ConstMaxTitleLength = 100;
    
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("tasks");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(ConstMaxTitleLength);

        builder.Property(x => x.IsCompleted)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => x.CreatedAt);
    }
}
