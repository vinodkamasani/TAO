using System;
using System.Collections.Generic;
using System.Text;

namespace TAO.SharedKernel.Interfaces
{
    public interface IUserContext
    {
        Guid? UserId { get; }

        string? Email { get; }

        string? Name { get; }

        bool IsAuthenticated { get; }
    }
}
