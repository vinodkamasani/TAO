using System;
using System.Collections.Generic;
using System.Text;

namespace TAO.Domain.Abstractions
{
    public abstract class AggregateRoot : Entity
    {
        protected AggregateRoot(Guid id)
            : base(id)
        {
        }
    }
}
