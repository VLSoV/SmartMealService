using Sms.Application.Interfaces;
using Sms.Domain.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Sms.Infrastructure.Servises;

public class HttpSmsClient : ISmsClient
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    public HttpSmsClient(string baseUrl, string username, string password, string endpoint = "/api/command")
    {
        _endpoint = endpoint;
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
    }

    public async Task<IEnumerable<MenuItem>> GetMenuAsync(bool withPrice, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Command = "GetMenu",
            CommandParameters = new { WithPrice = withPrice }
        };

        var response = await PostAsync<GetMenuResponse>(request, cancellationToken);
        if (!response.Success)
            throw new Exception($"Server returned error: {response.ErrorMessage}");

        return response.Data?.MenuItems ?? Enumerable.Empty<MenuItem>();
    }

    public async Task<bool> SendOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Command = "SendOrder",
            CommandParameters = new
            {
                OrderId = order.Id,
                MenuItems = order.Items.Select(i => new { i.Article, Quantity = i.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture) })
            }
        };

        var response = await PostAsync<SendOrderResponse>(request, cancellationToken);
        if (!response.Success)
            throw new Exception($"Server returned error: {response.ErrorMessage}");

        return true;
    }

    private async Task<T> PostAsync<T>(object payload, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(_endpoint, content, cancellationToken);
        var responseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        var result = JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result;
    }

    // Вспомогательные классы для десериализации
    private class GetMenuResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public GetMenuData Data { get; set; }
    }

    private class GetMenuData
    {
        public List<MenuItem> MenuItems { get; set; }
    }

    private class SendOrderResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}