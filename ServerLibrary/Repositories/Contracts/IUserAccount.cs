using Shared.Dtos;
using Shared.Responses;

namespace ServerLibrary.Repositories.Contracts;

public interface IUserAccount
{
    Task<GeneralResponse> CreateAsync(RegisterDto user);
    Task<LoginResponse> SignInAsync(LoginDto user);
    Task<LoginResponse> RefreshJwtTokenAsync(RefreshTokenDto token);
}