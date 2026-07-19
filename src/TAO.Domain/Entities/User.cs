using TAO.Domain.Common;
using TAO.Domain.Enums;

namespace TAO.Domain.Entities;

public sealed class User : Entity
{
    private User()
    {
    }

    public User(
        Guid organizationId,
        string firstName,
        string lastName,
        string email,
        UserRole role)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        FirstName = Guard.AgainstNullOrWhiteSpace(
            firstName,
            nameof(FirstName));

        LastName = Guard.AgainstNullOrWhiteSpace(
            lastName,
            nameof(LastName));

        Email = Guard.AgainstNullOrWhiteSpace(
            email,
            nameof(Email)).ToLowerInvariant();

        Role = role;
        Status = UserStatus.Active;
    }

    public Guid OrganizationId { get; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public UserStatus Status { get; private set; }

    public void Rename(string firstName, string lastName)
    {
        FirstName = Guard.AgainstNullOrWhiteSpace(
            firstName,
            nameof(FirstName));

        LastName = Guard.AgainstNullOrWhiteSpace(
            lastName,
            nameof(LastName));
    }

    public void ChangeEmail(string email)
    {
        Email = Guard.AgainstNullOrWhiteSpace(
            email,
            nameof(Email)).ToLowerInvariant();
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
        {
            return;
        }

        Status = UserStatus.Active;
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
        {
            return;
        }

        Status = UserStatus.Inactive;
    }
}