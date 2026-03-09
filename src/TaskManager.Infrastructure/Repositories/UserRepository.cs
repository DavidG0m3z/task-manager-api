using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserReposiory
{
    public UserRepository(ApplicationDbContext context) : base(context)
    { 
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower()  == email.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>u.Id == id);
    }
}

