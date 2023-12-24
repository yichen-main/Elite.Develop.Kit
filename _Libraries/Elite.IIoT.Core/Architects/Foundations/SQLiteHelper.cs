namespace Elite.IIoT.Core.Architects.Foundations;
internal abstract class SQLiteHelper : StoreDecorator
{
    protected sealed class StandardTrim(IOperation operation) : OperationDecoration(operation)
    {
        public override async ValueTask DeleteAsync<T>(long id) => await base.DeleteAsync<T>(id);
        public override async ValueTask CreateAsync<T>(T entity) => await base.CreateAsync(entity);
        public override async ValueTask UpdateAsync<T>(long id, T entity) => await base.UpdateAsync(id, entity);
        public override async ValueTask<T> SelectAsync<T>(long id) => await base.SelectAsync<T>(id);
        public override async ValueTask<IEnumerable<T>> SelectAsync<T>() => await base.SelectAsync<T>();
    }
    protected sealed class BasicOperation : IOperation
    {
        public async ValueTask DeleteAsync<T>(long id)
        {
            DynamicParameters parameters = new();
            parameters.Add(AtId, id);
            var type = typeof(T);
            using var connection = await InitialAsync(type);
            await connection.ExecuteAsync($"""
                DELETE FROM {type.Name}
                WHERE {nameof(SQLiteBase.Id)}={AtId}
                """, parameters);
        }
        public async ValueTask CreateAsync<T>(T entity)
        {
            List<string> uppers = [];
            List<string> lowers = [];
            DynamicParameters parameters = new();
            var infos = entity!.GetType().GetProperties();
            for (int i = default; i < infos.Length; i++)
            {
                if (infos[i].Name is not nameof(SQLiteBase.Id) && infos[i].Name is not nameof(SQLiteBase.CreateTime))
                {
                    uppers.Add(infos[i].Name);
                    lowers.Add($"{MarkSymbol.At}{infos[i].Name}");
                    parameters.Add(infos[i].Name, infos[i].GetValue(entity));
                }
            }
            var type = typeof(T);
            using var connection = await InitialAsync(type);
            await connection.ExecuteAsync($"""
                INSERT INTO {type.Name}({string.Join(MarkSymbol.Comma, uppers)})
                VALUES({string.Join(MarkSymbol.Comma, lowers)})
                """, parameters);
        }
        public async ValueTask UpdateAsync<T>(long id, T entity)
        {
            List<string> columns = [];
            DynamicParameters parameters = new();
            parameters.Add(AtId, id);
            var infos = entity!.GetType().GetProperties();
            for (int i = default; i < infos.Length; i++)
            {
                if (infos[i].Name is not nameof(SQLiteBase.Id) && infos[i].Name is not nameof(SQLiteBase.CreateTime))
                {
                    var value = infos[i].GetValue(entity);
                    if (value is not null)
                    {
                        columns.Add($"{infos[i].Name}={MarkSymbol.At}{infos[i].Name}");
                        parameters.Add(infos[i].Name, value);
                    }
                }
            }
            var type = typeof(T);
            using var connection = await InitialAsync(type);
            await connection.ExecuteAsync($"""
                UPDATE {type.Name} SET {string.Join(MarkSymbol.Comma, columns)}
                WHERE {nameof(SQLiteBase.Id)}={AtId}
                """, parameters);
        }
        public async ValueTask<T> SelectAsync<T>(long id)
        {
            var type = typeof(T);
            DynamicParameters parameters = new();
            parameters.Add(AtId, id);
            using var connection = await InitialAsync(type);
            return await connection.QuerySingleAsync<T>($"""
                SELECT {string.Join(MarkSymbol.Comma, GetQueryColumn(type))} FROM {type.Name}
                WHERE {nameof(SQLiteBase.Id)}={AtId}
                """, parameters);
        }
        public async ValueTask<IEnumerable<T>> SelectAsync<T>()
        {
            var type = typeof(T);
            using var connection = await InitialAsync(type);
            return await connection.QueryAsync<T>($"""
                SELECT {string.Join(MarkSymbol.Comma, GetQueryColumn(type))} FROM {type.Name}
                """);
        }
        List<string> GetQueryColumn(in Type type)
        {
            List<string> columns = [];
            var infos = type.GetProperties();
            for (int i = default; i < infos.Length; i++) columns.Add(infos[i].Name);
            return columns;
        }
        static string AtId => $"{MarkSymbol.At}{nameof(SQLiteBase.Id)}";
    }
    static async ValueTask<SqliteConnection> InitialAsync(Type type)
    {
        var infos = type.GetProperties();
        List<string> columns = [];
        for (int i = default; i < infos.Length; i++)
        {
            switch (infos[i].Name)
            {
                case nameof(SQLiteBase.Id):
                    columns.Add($"{infos[i].Name} INTEGER PRIMARY KEY AUTOINCREMENT");
                    break;

                case nameof(SQLiteBase.CreateTime):
                    columns.Add($"{infos[i].Name} TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL");
                    break;

                default:
                    columns.Add($"{infos[i].Name} {infos[i].PropertyType switch
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
                    }} NOT NULL");
                    break;
            }
        }
        if (!File.Exists(GetDatabasePath())) Directory.CreateDirectory(GetFolderPath());
        SqliteConnection connection = new($"Data Source={GetDatabasePath()}");
        if (!(await connection.QueryAsync($"SELECT name FROM sqlite_master WHERE type='table' ORDER BY '{type.Name}'")).Any())
        {
            await connection.ExecuteAsync($"CREATE TABLE {type.Name}({string.Join(MarkSymbol.Comma, columns)})");
        }
        return connection;
        string GetFolderPath() => Path.Combine(Path.GetDirectoryName(type.Assembly.Location)!, "LiteDB");
        string GetDatabasePath() => Path.Combine(GetFolderPath(), $"{type.Assembly.FullName!.Split(MarkSymbol.Comma)[default]}.db");
    }
}