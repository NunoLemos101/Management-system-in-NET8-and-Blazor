using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
    public async Task<GeneralResponse> CreateAsync(RegisterDto user)
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
        // Only first user will be admin
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

    public async Task<LoginResponse> SignInAsync(LoginDto user)
    {
        var applicationUser = await databaseRepository.FindUserByEmail(user.Email!);
        if (applicationUser is null) return new LoginResponse(false, "User not found.");
        
        if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password!))
        {
            return new LoginResponse(false, "Invalid password.");
        }

        var userRole = await databaseRepository.FindUserRole(applicationUser.Id);
        
        if (userRole is null) return new LoginResponse(false, "User role not found.");

        var systemRole = await databaseRepository.FindSystemRole(userRole.Id);
        
        if (systemRole is null) return new LoginResponse(false, "System role not found.");
        
        var jwtToken = GenerateJwtToken(applicationUser, systemRole.Name!);
        var refreshTokenString = GenerateRefreshToken();
        
        var refreshToken = await databaseRepository.FindRefreshToken(applicationUser);

        if (refreshToken is not null)
        {
            refreshToken.Token = refreshTokenString;
            await databaseRepository.SaveChanges();
        }
        else
        {
            await databaseRepository.AddToDatabase(new RefreshToken() { Token = refreshTokenString, UserId = applicationUser.Id });
        }
        
        return new LoginResponse(true, "Login successful.", jwtToken, refreshTokenString);
    }

    public async Task<LoginResponse> RefreshJwtTokenAsync(RefreshTokenDto tokenDto)
    {
        var token = await databaseRepository.FindRefreshToken(tokenDto);

        if (token is null) return new LoginResponse(false, "Token not found.");
        
        var user = await databaseRepository.FindUserById(token.UserId);
        
        if (user is null) return new LoginResponse(false, "User not found.");

        var userRole = await databaseRepository.FindUserRole(user.Id);
        var systemRole = await databaseRepository.FindSystemRole(userRole!.Id);
        var jwtToken = GenerateJwtToken(user, systemRole!.Name!);
        var refreshToken = GenerateRefreshToken();

        await databaseRepository.UpdateRefreshToken(token, refreshToken);

        return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
    }

    private string GenerateJwtToken(ApplicationUser user, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, role)
        };
        var token = new JwtSecurityToken(issuer: config.Value.Issuer, audience: config.Value.Audience, claims: userClaims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}