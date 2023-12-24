namespace Demo.Consumer.Handlers;
public class GetAllTaskQueryHandler(IGameTaskManager gameTaskManager) : IRequestHandler<GetAllTaskQuery, List<GameTask>>
{
    readonly IGameTaskManager _gameTaskManager = gameTaskManager;
    public Task<List<GameTask>> Handle(GetAllTaskQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_gameTaskManager.GetAllGameTask());
    }
}