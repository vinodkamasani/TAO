using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.SharedKernel.Results;
using TAO.Domain.Entities;

namespace TAO.Application.Organizations.Create;

public sealed class CreateOrganizationHandler(
    IApplicationDbContext dbContext)
    : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await dbContext
            .Set<Organization>()
            .AnyAsync(
                x => x.Code == request.Code,
                cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(
                new Error(
                    "Organization.DuplicateCode",
                    "An organization with the same code already exists."));
        }

        var organization = new Organization(
     request.Name,
     request.Code);

        await dbContext
            .Set<Organization>()
            .AddAsync(
                organization,
                cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(organization.Id);
    }
}