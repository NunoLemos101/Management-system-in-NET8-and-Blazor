using ServerLibrary.Enums;
using Shared.Dtos;
using Shared.Entities;

namespace Shared.Repositories;

public interface IDatabaseRepository
{ 
    Task<T> AddToDatabase<T>(T model);
    Task<ApplicationUser?> FindUserById(int id);
    Task<ApplicationUser?> FindUserByEmail(string email);
    Task<UserRole?> FindUserRole(int userId);
    Task<SystemRole?> FindSystemRole(SystemRoleName systemRoleName);
    Task<SystemRole?> FindSystemRole(int roleId);
    Task<RefreshToken?> FindRefreshToken(RefreshTokenDto token);
    Task<RefreshToken?> FindRefreshToken(ApplicationUser user);
    Task<RefreshToken> UpdateRefreshToken(RefreshToken refreshToken, string newToken);
    Task SaveChanges();
}