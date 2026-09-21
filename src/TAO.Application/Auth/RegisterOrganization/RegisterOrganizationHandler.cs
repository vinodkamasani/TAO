using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.RegisterOrganization;

public sealed class RegisterOrganizationHandler(
    IApplicationDbContext dbContext,
    IIdentityService identityService,
    ITransactionManager transactionManager)
    : IRequestHandler<
        RegisterOrganizationCommand,
        Result<RegisterOrganizationResponse>>
{
    public async Task<Result<RegisterOrganizationResponse>> Handle(
        RegisterOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var organizationName = request.OrganizationName.Trim();

        var organizationCode = request.OrganizationCode
            .Trim()
            .ToUpperInvariant();

        var firstName = request.FirstName.Trim();

        var lastName = request.LastName.Trim();

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        // Check organization code.
        var organizationExists = await dbContext
            .Set<Organization>()
            .AnyAsync(
                x => x.Code == organizationCode,
                cancellationToken);

        if (organizationExists)
        {
            return Result<RegisterOrganizationResponse>.Failure(
                new Error(
                    "Organization.CodeAlreadyExists",
                    "The organization code is already in use."));
        }

        // Check email.
        var emailExists = await dbContext
            .Set<User>()
            .AnyAsync(
                x => x.Email == email,
                cancellationToken);

        if (emailExists)
        {
            return Result<RegisterOrganizationResponse>.Failure(
                new Error(
                    "User.EmailAlreadyExists",
                    "A user with this email already exists."));
        }

        return await transactionManager.ExecuteAsync(
            async ct =>
            {
                // 1. Create organization.
                var organization = new Organization(
                    organizationName,
                    organizationCode);

                dbContext.Set<Organization>().Add(organization);

                await dbContext.SaveChangesAsync(ct);

                // 2. Generate one ID shared by Identity User
                //    and TAO User.
                var userId = Guid.CreateVersion7();

                // 3. Create ASP.NET Identity user.
                var identityResult =
                    await identityService.CreateUserAsync(
                        userId,
                        email,
                        request.Password,
                        ct);

                if (!identityResult.IsSuccess)
                {
                    return Result<RegisterOrganizationResponse>.Failure(
                        identityResult.Error);
                }

                // 4. Create TAO domain user.
                var user = new User(
                    organization.Id,
                    firstName,
                    lastName,
                    email,
                    UserRole.Administrator)
                {
                    Id = userId
                };

                dbContext.Set<User>().Add(user);

                await dbContext.SaveChangesAsync(ct);

                // 5. Transaction manager commits here.
                return Result<RegisterOrganizationResponse>.Success(
                    new RegisterOrganizationResponse(
                        organization.Id,
                        user.Id,
                        organization.Name,
                        organization.Code,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.Role,
                        user.Status));
            },
            cancellationToken);
    }
}