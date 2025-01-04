using System.Net.Http.Headers;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Implementations;
using Shared.Dtos;
using Shared.Responses;

namespace ClientLibrary.Factories;

public class HttpClientFactory(IHttpClientFactory httpClientFactory, LocalStorageService localStorageService)
{
    private const string HeaderKey = "Authorization";

    public async Task<HttpClient> TryCreateAuthenticatedClient()
    {
        var client = httpClientFactory.CreateClient("SystemApiClient");
        var token = await localStorageService.GetTokenAsync();
        
        if (string.IsNullOrWhiteSpace(token)) return client;
        
        var deserializedToken = Serializer.Deserialize<UserSessionDto>(token);
        
        if (deserializedToken is null) return client;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deserializedToken.Token);

        return client;
    }

    public HttpClient CreateAnonymousClient()
    {
        var client = httpClientFactory.CreateClient("SystemApiClient");
        client.DefaultRequestHeaders.Remove(HeaderKey);
        return client;
    }
}