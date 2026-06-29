namespace MusicForU.Application.DTOs;

public record CreateSubscriptionDto(int UserId, int PlanId);
public record SubscriptionResultDto(int Id, string PlanName, DateTime StartDate, DateTime EndDate, bool IsActive);
