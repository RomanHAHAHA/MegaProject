namespace ReviewsService.Domain.Dtos;

public record ReviewCreateDto(
    Guid ProductId,
    string Text,
    int Rate);