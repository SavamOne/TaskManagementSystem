using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Client.Proxies;

public abstract class BaseProxy
{
    private const string ApplicationJson = "application/json";

    private readonly HttpClient httpClient;

    protected BaseProxy(HttpClient httpClient, ILocalStorageService storageService)
    {
        StorageService = storageService.AssertNotNull();
        this.httpClient = httpClient.AssertNotNull();
    }

    protected ILocalStorageService StorageService { get; }

    protected abstract Task RefreshTokens();

    protected async Task<TResponse> SendRequestAsync<TResponse>(string url, HttpMethod method)
    {
        bool isFirstTry = true;

        while (true)
        {
            HttpResponseMessage response = await SendRequestCoreAsync(url, method, true);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                // TODO: Конкретный тип исключения.
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            if (!isFirstTry)
            {
                throw new UnauthorizedAccessException($"You don't have access to this page {url}");
            }

            await RefreshTokens();
            isFirstTry = false;
        }
    }

    protected async Task<TResponse> SendRequestAsync<TRequest, TResponse>(
        string url,
        HttpMethod method,
        TRequest request)
    {
        bool isFirstTry = true;

        while (true)
        {
            HttpResponseMessage response = await SendRequestCoreAsync(url, method, request, true);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            if (!isFirstTry)
            {
                throw new UnauthorizedAccessException($"You don't have access to this page {url}");
            }

            await RefreshTokens();
            isFirstTry = false;
        }
    }

    protected async Task<TResponse> SendAnonymousRequestAsync<TResponse>(string url, HttpMethod method)
    {
        using HttpResponseMessage response = await SendRequestCoreAsync(url, method, false);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    protected async Task<TResponse> SendAnonymousRequestAsync<TRequest, TResponse>(
        string url,
        HttpMethod method,
        TRequest request)
    {
        using HttpResponseMessage response = await SendRequestCoreAsync(url, method, request, false);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    private async Task<HttpResponseMessage> SendRequestCoreAsync(
        string url,
        HttpMethod httpMethod,
        bool addAuthorization)
    {
        using HttpRequestMessage requestMessage = new(httpMethod, url);

        if (addAuthorization)
        {
            string token = await StorageService.GetAccessTokenAsync();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

        return response;
    }

    private async Task<HttpResponseMessage> SendRequestCoreAsync<TRequest>(
        string url,
        HttpMethod httpMethod,
        TRequest content,
        bool addAuthorization)
    {
        using HttpRequestMessage requestMessage = new(httpMethod, url)
        {
            Content = JsonContent.Create(content, MediaTypeHeaderValue.Parse(ApplicationJson))
        };

        if (addAuthorization)
        {
            string token = await StorageService.GetAccessTokenAsync();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

        return response;
    }
}