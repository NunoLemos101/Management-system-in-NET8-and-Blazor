using Blazored.LocalStorage;

namespace ClientLibrary.Services.Implementations;

public class LocalStorageService(ILocalStorageService localStorageService)
{
    private const string StorageKey = "authentication-token";
    public async Task<string> GetTokenAsync() => await localStorageService.GetItemAsStringAsync(StorageKey);
    public async Task SetTokenAsync(string token) => await localStorageService.SetItemAsync(StorageKey, token);
    public async Task RemoveTokenAsync() => await localStorageService.RemoveItemAsync(StorageKey);
}