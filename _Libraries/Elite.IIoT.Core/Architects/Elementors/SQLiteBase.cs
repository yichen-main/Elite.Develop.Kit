namespace Elite.IIoT.Core.Architects.Elementors;
public abstract class SQLiteBase
{
    public long Id { get; private set; }
    public string CreateTime { get; private set; } = DateTime.UtcNow.ToString("o");
}