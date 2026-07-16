using System;
using System.Collections.Generic;
using System.Text;

namespace TAO.Domain.Abstractions;

public abstract class AuditableEntity : AggregateRoot
{
    protected AuditableEntity(Guid id)
        : base(id)
    {
    }

    public DateTime CreatedOnUtc { get; protected set; }

    public Guid? CreatedBy { get; protected set; }

    public DateTime? ModifiedOnUtc { get; protected set; }

    public Guid? ModifiedBy { get; protected set; }
}
