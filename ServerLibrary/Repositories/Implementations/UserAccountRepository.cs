using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerLibrary.Data;
using ServerLibrary.Enums;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using Shared.Dtos;
using Shared.Entities;
using Shared.Repositories;
using Shared.Responses;

namespace ServerLibrary.Repositories.Implementations;

public class UserAccountRepository(IOptions<JwtSection> config, IDatabaseRepository databaseRepository) : IUserAccount
{
    public async Task<GeneralResponse> CreateAsync(Register user)
    {
        var checkUser = await databaseRepository.FindUserByEmail(user.Email!);

        if (checkUser != null) return new GeneralResponse(false, "User already exists.");

        var applicationUser = await databaseRepository.AddToDatabase(new ApplicationUser()
        {
            Name = user.Name,
            Email = user.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
        });

        var adminRole = await databaseRepository.FindSystemRole(SystemRoleName.Admin);

        if (adminRole is null)
        {
            adminRole = await databaseRepository.AddToDatabase(new SystemRole() { Name = SystemRoleName.Admin.ToString() });

            await databaseRepository.AddToDatabase(new UserRole()
            {
                RoleId = adminRole.Id,
                UserId = applicationUser.Id
            });

            return new GeneralResponse(true, "Account created.");
        }

        var userRole = await databaseRepository.FindSystemRole(SystemRoleName.User);

        if (userRole is null)
        {
            var response = await databaseRepository.AddToDatabase(new SystemRole() { Name = SystemRoleName.User.ToString() });
            await databaseRepository.AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
        }
        else
        {
            await databaseRepository.AddToDatabase(new UserRole() { RoleId = userRole.Id, UserId = applicationUser.Id });
        }
        
        return new GeneralResponse(true, "Account created.");
    }

    public Task<LoginResponse> SignInAsync(Login user)
    {
        throw new NotImplementedException();
    }
}