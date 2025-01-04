using System.Net.Http.Json;
using ClientLibrary.Factories;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using Shared.Dtos;
using Shared.Responses;

namespace ClientLibrary.Services.Implementations;

public class UserAccountService(HttpClientFactory httpClientFactory) : IUserAccountService
{
    public const string AuthUrl = "api/authentication";
    
    public async Task<GeneralResponse> CreateAsync(RegisterDto user)
    {
        var client = httpClientFactory.CreateAnonymousClient();

        var result = await client.PostAsJsonAsync($"{AuthUrl}/register", user);

        if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "An error occurred");

        return await result.Content.ReadFromJsonAsync<GeneralResponse>();
    }

    public async Task<LoginResponse> SignInAsync(LoginDto user)
    {
        var client = httpClientFactory.CreateAnonymousClient();
        var result = await client.PostAsJsonAsync($"{AuthUrl}/login", user);
        
        if (!result.IsSuccessStatusCode) return new LoginResponse(false, "An error occurred");
        
        return await result.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public Task<LoginResponse> RefreshJwtTokenAsync(RefreshTokenDto token)
    {
        throw new NotImplementedException();
    }

    public Task<WeatherForecast[]> GetWeatherForecastAsync()
    {
        throw new NotImplementedException();
    }
}