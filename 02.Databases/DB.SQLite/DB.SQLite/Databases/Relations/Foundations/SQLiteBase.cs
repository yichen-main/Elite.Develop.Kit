using Base = Elite.Core.Databases.Relations.Foundations.TableModel;

namespace Elite.Core.Databases.Relations.Foundations;
public abstract class SQLiteBase<T> where T : Base
{
    protected async Task<IDbConnection> ConnectionAsync()
    {
        List<string> columns = [];
        var properties = typeof(T).GetProperties();
        for (int i = default; i < properties.Length; i++)
        {
            switch (properties[i].Name)
            {
                case nameof(Base.Id):
                    columns.Add($"{properties[i].Name} INTEGER PRIMARY KEY AUTOINCREMENT");
                    break;

                case nameof(Base.CreateTime):
                    columns.Add($"{properties[i].Name} TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL");
                    break;

                default:
                    columns.Add($"{properties[i].Name} {ColumnInfo(properties[i].PropertyType)} NOT NULL");
                    break;
            }
        }
        var connection = new SqliteConnection($"Data Source={DatabasePath()}");
        if (!File.Exists(DatabasePath()) && Directory.CreateDirectory(FolderPath()).Exists)
        {
            await connection.ExecuteAsync($"CREATE TABLE {Base.TableName<T>()}({string.Join(',', columns)})");
        }
        return connection;
        static string ColumnInfo(in Type type) => type switch
        {
            var item when item.IsEnum => "INTEGER",
            var item when item.Equals(typeof(byte[])) => "BLOB",
            var item when item.Equals(typeof(float)) => "REAL",
            var item when item.Equals(typeof(double)) => "REAL",
            var item when item.Equals(typeof(bool)) => "INTEGER",
            var item when item.Equals(typeof(byte)) => "INTEGER",
            var item when item.Equals(typeof(short)) => "INTEGER",
            var item when item.Equals(typeof(int)) => "INTEGER",
            var item when item.Equals(typeof(long)) => "INTEGER",
            var item when item.Equals(typeof(sbyte)) => "INTEGER",
            var item when item.Equals(typeof(ushort)) => "INTEGER",
            var item when item.Equals(typeof(uint)) => "INTEGER",
            var item when item.Equals(typeof(ulong)) => "INTEGER",
            _ => "TEXT"
        };
        static string FolderPath() => Path.Combine(Path.GetDirectoryName(typeof(T).Assembly.Location)!, "LiteDB");
        static string DatabasePath() => Path.Combine(FolderPath(), $"{typeof(T).Assembly.FullName!.Split(',')[default]}.db");
    }
    protected async ValueTask<IEnumerable<T>> QueryAsync(IDbConnection connection)
    {
        List<string> columns = [];
        var properties = typeof(T).GetProperties();
        for (int i = default; i < properties.Length; i++) columns.Add(properties[i].Name);
        return await connection.QueryAsync<T>($"SELECT {string.Join(',', columns)} FROM {Base.TableName<T>()}");
    }
    protected async ValueTask<int> InsertAsync(IDbConnection connection, T entity)
    {
        List<string> uppers = [];
        List<string> lowers = [];
        DynamicParameters parameters = new();
        var properties = entity!.GetType().GetProperties();
        for (int i = default; i < properties.Length; i++)
        {
            if (properties[i].Name is not nameof(Base.Id) && properties[i].Name is not nameof(Base.CreateTime))
            {
                uppers.Add(properties[i].Name);
                lowers.Add($"@{properties[i].Name}");
                parameters.Add(properties[i].Name, properties[i].GetValue(entity));
            }
        }
        return await connection.ExecuteAsync($"""
        INSERT INTO {Base.TableName<T>()}({string.Join(',', uppers)})VALUES({string.Join(',', lowers)})
        """, parameters);
    }
    protected async ValueTask<int> InsertAsync(IDbConnection connection, IEnumerable<T> entities)
    {
        List<string> uppers = [];
        List<string> lowers = [];
        var properties = typeof(T).GetProperties();
        for (int i = default; i < properties.Length; i++)
        {
            if (properties[i].Name is not nameof(Base.Id) && properties[i].Name is not nameof(Base.CreateTime))
            {
                uppers.Add(properties[i].Name);
                lowers.Add($"@{properties[i].Name}");
            }
        }
        return await connection.ExecuteAsync($"""
        INSERT INTO {Base.TableName<T>()}({string.Join(',', uppers)})VALUES({string.Join(',', lowers)})
        """, entities);
    }
    protected async ValueTask<int> UpdateAsync(IDbConnection connection, long id, IEnumerable<(string name, object value)> properties)
    {
        List<string> columns = [];
        DynamicParameters parameters = new();
        foreach (var (name, value) in properties)
        {
            if (name is not nameof(Base.Id) && name is not nameof(Base.CreateTime))
            {
                columns.Add($"{name}=@{name}");
                parameters.Add(name, value);
            }
        }
        return await connection.ExecuteAsync($"""
        UPDATE {Base.TableName<T>()} SET {string.Join(',', columns)} WHERE {nameof(Base.Id)}='{id}'
        """, parameters);
    }
    protected async ValueTask<int> DeleteAsync(IDbConnection connection, long id)
    {
        return await connection.ExecuteAsync($"DELETE FROM {Base.TableName<T>()} WHERE {nameof(Base.Id)}='{id}'");
    }
}