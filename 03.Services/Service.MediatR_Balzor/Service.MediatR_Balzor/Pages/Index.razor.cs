namespace Service.MediatR_Balzor.Pages;
public partial class Index
{
    List<GameTask> GameTasks = new();
    protected override async Task OnInitializedAsync()
    {
        GameTasks = await Mediator.Send(new GetAllTaskQuery());
    }
    async Task GetMyTaskAsync()
    {
        GameTasks = await Mediator.Send(new GetMyTaskQuery(666));
    }
    async Task AddTaskAsync()
    {
        var task = await Mediator.Send(new AddTaskCommand(666, "嘿嘿嘿"));
        GameTasks.Add(task);
    }
}