using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
