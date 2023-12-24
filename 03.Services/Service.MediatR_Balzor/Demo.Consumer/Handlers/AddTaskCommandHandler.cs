namespace Demo.Consumer.Handlers;
internal class AddTaskCommandHandler(IMediator mediator, IGameTaskManager gameTaskManager) : IRequestHandler<AddTaskCommand, GameTask>
{
    readonly IMediator _mediator = mediator;
    readonly IGameTaskManager _gameTaskManager = gameTaskManager;
    public async Task<GameTask> Handle(AddTaskCommand request, CancellationToken cancellationToken)
    {
        var allTask = await _mediator.Send(new GetAllTaskQuery(), cancellationToken);
        GameTask task = new()
        {
            UserId = request.UserId,
            TaskId = allTask.Max(item => item.TaskId) + 1,
            TaskName = request.TaskName,
            Experience = Random.Shared.Next(0, 1000)
        };
        _gameTaskManager.AddGameTask(task);
        return task;
    }
}