namespace Demo.Consumer.Handlers;
public class GetTaskByUserHandler(IMediator mediator) : IRequestHandler<GetMyTaskQuery, List<GameTask>>
{
    readonly IMediator _mediator = mediator;
    public async Task<List<GameTask>> Handle(GetMyTaskQuery request, CancellationToken cancellationToken)
    {
        var allTask = await _mediator.Send(new GetAllTaskQuery(), cancellationToken);
        return allTask.Where(item => item.UserId == request.UserId).ToList();
    }
}