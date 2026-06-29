namespace MusicForU.Domain.Entities;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int PlanId { get; set; }
    public Plan Plan { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive => DateTime.UtcNow <= EndDate;
}
