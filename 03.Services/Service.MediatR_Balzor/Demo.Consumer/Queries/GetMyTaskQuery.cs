namespace Demo.Consumer.Queries;

/// <summary>
/// 獲取用戶任務
/// </summary>
/// <param name="UserId"></param>
public record GetMyTaskQuery(int UserId) : IRequest<List<GameTask>>;