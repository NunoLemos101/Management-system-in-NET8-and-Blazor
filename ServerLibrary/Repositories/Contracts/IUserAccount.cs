using Shared.Dtos;
using Shared.Responses;

namespace ServerLibrary.Repositories.Contracts;

public interface IUserAccount
{
    Task<GeneralResponse> CreateAsync(Register user);
    Task<LoginResponse> SignInAsync(Login user);
}