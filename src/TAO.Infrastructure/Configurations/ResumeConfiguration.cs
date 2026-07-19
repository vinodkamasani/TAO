using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.ToTable("Resumes");

        builder.ConfigurePrimaryKey();
        builder.ConfigureAuditColumns();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.ApplicationId)
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.FileContent)
            .HasColumnType("varbinary(max)")
            .IsRequired();

        builder.HasOne<CandidateApplication>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OrganizationId);

        builder.HasIndex(x => x.ApplicationId);

        builder.HasIndex(x => new
        {
            x.OrganizationId,
            x.ApplicationId
        });
    }
}