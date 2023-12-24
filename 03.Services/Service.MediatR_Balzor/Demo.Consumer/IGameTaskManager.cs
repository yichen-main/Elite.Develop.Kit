namespace Demo.Consumer;
public interface IGameTaskManager
{
    void AddGameTask(GameTask task);
    List<GameTask> GetAllGameTask();
    GameTask? GetGameTask(int taskId);
    GameTask? GetTaskByUser(int userId);
}