namespace Elite.Core.Databases.Relations.Foundations;
public abstract class TableModel
{
    public Guid Id { get; init; }
    public string Updater { get; init; } = string.Empty;
    public DateTime UpdateTime { get; init; }
    public string Creator { get; init; } = string.Empty;
    public DateTime CreateTime { get; init; }
    protected internal static string TableName<T>() => typeof(T).GetCustomAttribute<TableAttribute>()!.Name;
}