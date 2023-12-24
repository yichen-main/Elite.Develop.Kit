namespace Demo.Consumer;
public class GameTaskManager : IGameTaskManager
{
    readonly List<GameTask> _gameTasks = [];
    public GameTaskManager()
    {
        _gameTasks.Add(new() { TaskId = 1, TaskName = "任務一", Experience = 100 });
        _gameTasks.Add(new() { TaskId = 2, TaskName = "任務二", Experience = 200 });
        _gameTasks.Add(new() { TaskId = 3, TaskName = "任務三", Experience = 300 });
        _gameTasks.Add(new() { UserId = 666, TaskId = 4, TaskName = "任務四", Experience = 400 });
    }
    public List<GameTask> GetAllGameTask() => _gameTasks;
    public GameTask? GetGameTask(int taskId) => _gameTasks.FirstOrDefault(item => item.TaskId == taskId);
    public GameTask? GetTaskByUser(int userId) => _gameTasks.FirstOrDefault(item => item.UserId == userId);
    public void AddGameTask(GameTask task) => _gameTasks.Add(task);
}