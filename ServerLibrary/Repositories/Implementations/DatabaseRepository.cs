using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Enums;
using Shared.Dtos;
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
    
    public async Task<ApplicationUser?> FindUserById(int id)
    {
        return await dbContext.ApplicationUsers.FirstOrDefaultAsync(user => user!.Id == id);
    }

    public async Task<ApplicationUser?> FindUserByEmail(string email)
    {
        return await dbContext.ApplicationUsers.FirstOrDefaultAsync(user => user!.Email!.ToLower().Equals(email.ToLower()));
    }
    
    public async Task<UserRole?> FindUserRole(int userId)
    {
        return await dbContext.UserRoles.FirstOrDefaultAsync(userRole => userRole!.UserId == userId);
    }

    public async Task<SystemRole?> FindSystemRole(SystemRoleName systemRoleName)
    {
        return await dbContext.SystemRoles.FirstOrDefaultAsync(systemRole => systemRole!.Name!.Equals(systemRoleName.ToString()));
    }

    public async Task<SystemRole?> FindSystemRole(int roleId)
    {
        return await dbContext.SystemRoles.FirstOrDefaultAsync(systemRole => systemRole.Id == roleId);
    }

    public async Task<RefreshToken?> FindRefreshToken(RefreshTokenDto token)
    {
        return await dbContext.RefreshTokens.FirstOrDefaultAsync(refreshToken => refreshToken.Token!.Equals(token.Token));
    }
    
    public async Task<RefreshToken?> FindRefreshToken(ApplicationUser user)
    {
        return await dbContext.RefreshTokens.FirstOrDefaultAsync(refreshToken => refreshToken.UserId == user.Id);
    }
    
    public async Task<RefreshToken> UpdateRefreshToken(RefreshToken refreshToken, string newToken)
    {
        refreshToken.Token = newToken;
        await dbContext.SaveChangesAsync();
        return refreshToken;
    }
    
    public async Task SaveChanges()
    {
        await dbContext.SaveChangesAsync();
    }
}