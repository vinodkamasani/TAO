using TAO.Domain.Enums;

namespace TAO.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid? UserId { get; }

    bool IsAuthenticated { get; }
    Guid? OrganizationId { get; }

    UserRole? Role { get; }
}