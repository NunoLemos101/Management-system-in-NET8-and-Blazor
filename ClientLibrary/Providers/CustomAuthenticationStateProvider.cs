using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Implementations;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Dtos;

namespace ClientLibrary.Providers;

public class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var stringToken = await localStorageService.GetTokenAsync();
        
        if (string.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(_anonymous));
        
        var token = Serializer.Deserialize<UserSessionDto>(stringToken);
        
        if (token is null) return await Task.FromResult(new AuthenticationState(_anonymous));

        var userClaims = DecryptToken(token.Token!);
        
        var claimsPrincipal = SetClaimsPrincipal(userClaims);
        
        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    public async Task UpdateAuthenticationStateAsync(UserSessionDto userSession)
    {
        var claims = new ClaimsPrincipal();

        if (userSession.Token != null || userSession.RefreshToken != null)
        {
            var serializedToken = Serializer.Serialize(userSession);
            await localStorageService.SetTokenAsync(serializedToken);
            var userClaims = DecryptToken(userSession.Token!);
            claims = SetClaimsPrincipal(userClaims);
        }
        else
        {
            await localStorageService.RemoveTokenAsync();
        }
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claims)));
    }
    public static ClaimsPrincipal SetClaimsPrincipal(CustomUserClaimsDto claims)
    {
        if (claims.Email is null) return new ClaimsPrincipal();

        return new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, claims.Id!),
                new Claim(ClaimTypes.Name, claims.Email),
                new Claim(ClaimTypes.Email, claims.Name!),
                new Claim(ClaimTypes.Role, claims.Role!)
            }, 
            "jwtAuth"
        ));
    }

    private static CustomUserClaimsDto DecryptToken(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        
        var userClaims = new CustomUserClaimsDto()
        {
            Id = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value,
            Email = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)!.Value,
            Name = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value,
            Role = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)!.Value
        };
        
        return userClaims;
    }
}