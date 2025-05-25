using System.Net.Http.Headers;
using System.Text.Json;
using Common.Domain.Dtos;
using Common.Domain.Interfaces;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Services;

public class CartServiceClient(
    HttpClient httpClient,
    IHttpUserContext httpContext) : ICartServiceClient
{
    public async Task<IReadOnlyList<CartItemDto>> GetCartItemsAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        var accessToken = httpContext.AccessToken; 

        if (string.IsNullOrEmpty(accessToken))
        {
            return [];
        }

        httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await httpClient.GetAsync(
            $"https://localhost:7059/api/carts/{userId}", 
            cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var wrapper = JsonSerializer.Deserialize<DataWrapper<IReadOnlyList<CartItemDto>>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return wrapper?.Data ?? [];
    }
}

public class DataWrapper<T>
{
    public T Data { get; set; } = default!;
}