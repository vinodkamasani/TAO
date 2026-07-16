namespace TAO.Domain.Campaigns;

using TAO.Domain.Abstractions;

public sealed class Campaign : AuditableEntity
{
    private Campaign(
        Guid id,
        string name,
        string description
        ): base(id)
    {
        Name = name;
        Description = description;
        Status= CampaignStatus.Draft;
    }
    public string Name { get; private set; }

    public string Description { get; private set; }

    public CampaignStatus Status { get; private set; }

    public static Campaign Create(
        Guid id,
        string name,
        string description
        )
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Campaign name cannot be empty or whitespace.", nameof(name));
        }
        return new Campaign(id, name.Trim(), description.Trim() );
    }
}
