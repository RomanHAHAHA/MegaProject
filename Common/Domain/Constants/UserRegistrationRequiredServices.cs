namespace Common.Domain.Constants;

public static class UserRegistrationRequiredServices
{
    public static string ReviewsService { get; set; } = nameof(ReviewsService);
    
    public static string OrdersService { get; set; } = nameof(OrdersService);
}