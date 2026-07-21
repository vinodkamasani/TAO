using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.Exceptions;

namespace TAO.Domain.Entities;

public sealed class Organization : Entity
{
    private Organization()
    {
    }

    public Organization(string name, string code)
    {
        ValidateName(name);
        ValidateCode(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Status = OrganizationStatus.Active;
    }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; }

    public OrganizationStatus Status { get; private set; }

    public void Rename(string name)
    {
        ValidateName(name);

        var trimmedName = name.Trim();

        if (Name.Equals(trimmedName, StringComparison.Ordinal))
        {
            return;
        }

        Name = trimmedName;
    }

    public void Activate()
    {
        if (Status == OrganizationStatus.Active)
        {
            return;
        }

        Status = OrganizationStatus.Active;
    }

    public void Deactivate()
    {
        if (Status == OrganizationStatus.Inactive)
        {
            return;
        }

        Status = OrganizationStatus.Inactive;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Organization name is required.");
        }
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Organization code is required.");
        }

        code = code.Trim();

        if (code.Any(c => !char.IsLetterOrDigit(c) && c != '-'))
        {
            throw new DomainException(
                "Organization code can only contain letters, numbers and hyphens.");
        }
    }
}