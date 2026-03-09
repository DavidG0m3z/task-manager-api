using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces
{
    public interface IUserReposiory : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistAsync(string email);
    }
}
