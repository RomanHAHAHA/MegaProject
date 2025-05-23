using ReviewsService.Domain.Enums;

namespace ReviewsService.Domain.Entities;

public class Review 
{
    public Guid UserId { get; set; }

    public UserSnapshot? User { get; set; }

    public Guid ProductId { get; set; }

    public ProductSnapshot? Product { get; set; }

    public string Text { get; set; } = string.Empty;

    public int Rate { get; set; }
    
    public ReviewStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public List<ReviewVote> Votes { get; set; } = [];
}