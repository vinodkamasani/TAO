using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAO.Domain.Entities;
using TAO.Infrastructure.Persistence.Extensions;

namespace TAO.Infrastructure.Persistence.Configurations;

public sealed class ResumeImportFailureConfiguration : IEntityTypeConfiguration<ResumeImportFailure>
{
    public void Configure(EntityTypeBuilder<ResumeImportFailure> builder)
    {
        builder.ToTable("ResumeImportFailures");

        builder.ConfigurePrimaryKey();

        builder.ConfigureAuditColumns();

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.ResumeImportId)
            .IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FailureReason)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ResumeImport>()
            .WithMany()
            .HasForeignKey(x => x.ResumeImportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ResumeImportId);
    }
}