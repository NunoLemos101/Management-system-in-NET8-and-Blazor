using Shared.Dtos;
using Shared.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IUserAccountService
{
    Task<GeneralResponse> CreateAsync(RegisterDto user);
    Task<LoginResponse> SignInAsync(LoginDto user);
    Task<LoginResponse> RefreshJwtTokenAsync(RefreshTokenDto token);
    Task<WeatherForecast[]> GetWeatherForecastAsync();
}