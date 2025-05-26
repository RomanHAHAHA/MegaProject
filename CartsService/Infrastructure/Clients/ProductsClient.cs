using System.Text.Json;
using CartsService.Domain.Interfaces;
using Common.Domain.Models.Results;

namespace CartsService.Infrastructure.Clients;

public class ProductsClient(HttpClient httpClient) : IProductsClient
{
    private const string BaseProductsUrl = "https://localhost:7278/api/products";
    
    public async Task<BaseResponse<int>> GetProductQuantityAsync(
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient
            .GetAsync($"{BaseProductsUrl}/{productId}/quantity", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return BaseResponse<int>.NotFound("Product");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseDto = JsonSerializer.Deserialize<ResponseDto<int>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var quantity = responseDto?.Data;

        return quantity is null ? 
            BaseResponse<int>.InternalServerError() : 
            BaseResponse<int>.Ok(quantity.Value);
    }
}

public class ResponseDto<T>
{
    public T? Data { get; set; }
}