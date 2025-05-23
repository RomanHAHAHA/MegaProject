namespace ReviewsService.Domain.Dtos;

public record ReviewCreateDto(
    Guid UserId,
    Guid ProductId,
    string Text,
    int Rate);