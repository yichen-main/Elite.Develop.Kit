namespace Demo.Consumer.Commands;
public record AddTaskCommand(int UserId, string TaskName) : IRequest<GameTask>;