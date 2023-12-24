namespace Demo.Consumer.Models;
public class GameTask
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 獲得經驗值
    /// </summary>
    public int Experience { get; set; }
}