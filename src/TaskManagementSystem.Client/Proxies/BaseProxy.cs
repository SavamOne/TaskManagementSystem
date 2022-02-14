using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagementSystem.Client.Resources;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Options;

namespace TaskManagementSystem.Client.Proxies;

public abstract class BaseProxy
{
    private const string ApplicationJson = "application/json";

    private readonly HttpClient httpClient;
    private readonly ILocalizationService localizationService;

    protected BaseProxy(HttpClient httpClient, ILocalTokensService storageService, IToastService toastService, ILocalizationService localizationService)
    {
        this.localizationService = localizationService;
        ToastService = toastService;
        StorageService = storageService.AssertNotNull();
        this.httpClient = httpClient.AssertNotNull();
    }

    protected IToastService ToastService { get; }

    protected ILocalTokensService StorageService { get; }

    protected abstract Task RefreshTokens();

    protected async Task<Result<TResponse>> SendRequestAsync<TResponse>(string url, HttpMethod method)
    {
        bool isFirstTry = true;

        while (true)
        {
            HttpResponseMessage response = await SendRequestCoreAsync(url, method, true);

            if (response.IsSuccessStatusCode)
            {
                TResponse? value = await response.Content.ReadFromJsonAsync<TResponse>();
                return Result<TResponse>.Success(value!);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ErrorObject? error = await response.Content.ReadFromJsonAsync<ErrorObject>();
                return Result<TResponse>.Error(error!.Error);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return Result<TResponse>.Error(LocalizedResources.BaseProxy_InternalServerError);
            }
            if (!isFirstTry && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<TResponse>.Error(string.Format(LocalizedResources.BaseProxy_SendRequestAsync_YouDontHaveAccessToThisUrl, url.ToLower()));
            }
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return Result<TResponse>.Error(string.Format(LocalizedResources.BaseProxy_UnhandledStatusCode, response.StatusCode));
            }

            await RefreshTokens();
            isFirstTry = false;
        }
    }

    protected async Task<Result<TResponse>> SendRequestAsync<TRequest, TResponse>(
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
                TResponse? value = await response.Content.ReadFromJsonAsync<TResponse>();
                return Result<TResponse>.Success(value!);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ErrorObject? error = await response.Content.ReadFromJsonAsync<ErrorObject>();
                return Result<TResponse>.Error(error!.Error);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return Result<TResponse>.Error(LocalizedResources.BaseProxy_InternalServerError);
            }
            if (!isFirstTry && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<TResponse>.Error(string.Format(LocalizedResources.BaseProxy_SendRequestAsync_YouDontHaveAccessToThisUrl, url.ToLower()));
            }
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return Result<TResponse>.Error(string.Format(LocalizedResources.BaseProxy_UnhandledStatusCode, response.StatusCode));
            }

            await RefreshTokens();
            isFirstTry = false;
        }
    }

    protected async Task<Result<TResponse>> SendAnonymousRequestAsync<TResponse>(string url, HttpMethod method)
    {
        using HttpResponseMessage response = await SendRequestCoreAsync(url, method, false);

        if (response.IsSuccessStatusCode)
        {
            TResponse? value = await response.Content.ReadFromJsonAsync<TResponse>();
            return Result<TResponse>.Success(value!);
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ErrorObject? error = await response.Content.ReadFromJsonAsync<ErrorObject>();
            return Result<TResponse>.Error(error!.Error);
        }
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return Result<TResponse>.Error(LocalizedResources.BaseProxy_InternalServerError);
        }

        return Result<TResponse>.Error(string.Format(LocalizedResources.BaseProxy_UnhandledStatusCode, response.StatusCode));
    }

    protected async Task<Result<TResponse>> SendAnonymousRequestAsync<TRequest, TResponse>(
        string url,
        HttpMethod method,
        TRequest request)
    {
        HttpResponseMessage response = await SendRequestCoreAsync(url, method, request, false);

        if (response!.IsSuccessStatusCode)
        {
            TResponse? value = await response.Content.ReadFromJsonAsync<TResponse>(ApplicationJsonOptions.Options);
            return Result<TResponse>.Success(value!);
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ErrorObject? error = await response.Content.ReadFromJsonAsync<ErrorObject>(ApplicationJsonOptions.Options);
            return Result<TResponse>.Error(error!.Error);
        }
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return Result<TResponse>.Error(LocalizedResources.BaseProxy_InternalServerError);
        }

        return Result<TResponse>.Error(string.Format(LocalizedResources.BaseProxy_UnhandledStatusCode, response.StatusCode));
    }

    private async Task<HttpResponseMessage> SendRequestCoreAsync(
        string url,
        HttpMethod httpMethod,
        bool addAuthorization)
    {
        using HttpRequestMessage requestMessage = new(httpMethod, url);

        CultureInfo cultureInfo = await localizationService.GetApplicationCultureAsync();
        requestMessage.Headers.AcceptLanguage.Clear();
        requestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo.TwoLetterISOLanguageName));

        if (addAuthorization)
        {
            string? token = await StorageService.GetAccessTokenAsync();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine(requestMessage.Headers.AcceptLanguage);
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

        CultureInfo cultureInfo = await localizationService.GetApplicationCultureAsync();
        requestMessage.Headers.AcceptLanguage.Clear();
        requestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo.TwoLetterISOLanguageName));

        if (addAuthorization)
        {
            string? token = await StorageService.GetAccessTokenAsync();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

        return response;
    }
}