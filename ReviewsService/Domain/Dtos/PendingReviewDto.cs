namespace ReviewsService.Domain.Dtos;

public class PendingReviewDto
{
    public Guid ProductId { get; set; }
    
    public Guid UserId { get; set; }
    
    public string NickName { get; set; } = string.Empty;

    public string AvatarPath { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public double Rate { get; set; } 

    public string CreatedAt { get; set; } = string.Empty;
}