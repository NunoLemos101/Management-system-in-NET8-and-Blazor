using ServerLibrary.Enums;
using Shared.Entities;

namespace Shared.Repositories;

public interface IDatabaseRepository
{ 
    Task<T> AddToDatabase<T>(T model);
    Task<ApplicationUser?> FindUserByEmail(string email);
    Task<SystemRole?> FindSystemRole(SystemRoleName systemRoleName);
}