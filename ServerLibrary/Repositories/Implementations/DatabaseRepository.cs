using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Enums;
using Shared.Entities;

namespace Shared.Repositories;

public class DatabaseRepository(AppDbContext dbContext) : IDatabaseRepository
{
    public async Task<T> AddToDatabase<T>(T model)
    {
        var result = dbContext.Add(model!);
        await dbContext.SaveChangesAsync();
        return (T)result.Entity;
    }

    public Task<ApplicationUser?> FindUserByEmail(string email)
    {
        return dbContext.ApplicationUsers.FirstOrDefaultAsync(user => user!.Email!.ToLower().Equals(email.ToLower()));
    }

    public async Task<SystemRole?> FindSystemRole(SystemRoleName systemRoleName)
    {
        return await dbContext.SystemRoles.FirstOrDefaultAsync(systemRole => systemRole!.Name!.Equals(systemRoleName.ToString()));
    }
}