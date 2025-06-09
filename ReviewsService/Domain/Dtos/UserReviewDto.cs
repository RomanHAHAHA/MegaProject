namespace ReviewsService.Domain.Dtos;

public class UserReviewDto
{
    public required Guid UserId { get; set; }
    
    public required string NickName { get; set; }

    public required string AvatarPath { get; set; }
}