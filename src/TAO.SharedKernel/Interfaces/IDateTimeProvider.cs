using System;
using System.Collections.Generic;
using System.Text;

namespace TAO.SharedKernel.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }
}
