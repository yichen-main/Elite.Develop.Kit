namespace Demo.Consumer.Queries;

/// <summary>
/// 獲取所有任務
/// </summary>
public record GetAllTaskQuery() : IRequest<List<GameTask>>;