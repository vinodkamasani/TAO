namespace TAO.Domain.Common;

public abstract class Entity
{
    protected Entity()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();

    public Guid? CreatedBy { get; private set; }

    public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;

    public Guid? ModifiedBy { get; private set; }

    public DateTime? ModifiedOn { get; private set; }

    public void MarkAsModified()
    {
        ModifiedOn = DateTime.UtcNow;
    }
}