using SQLite.Models;
using DependencyAttribute = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace SQLite.Repositories;
public interface ISQLiteHelper
{
    ValueTask CreateTable();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class SQLiteHelper : ISQLiteHelper
{
    public async ValueTask CreateTable()
    {
        if (Directory.CreateDirectory(FilePath).Exists)
        {
            //連接sqlite資料庫
            using SqliteConnection connection = new($"Data Source={DBPath}");

            //當找不到sqlite檔案時，建立新表，新表創建後就會產生sqlite檔案
            if (!File.Exists(DBPath)) await connection.ExecuteAsync($"""
            CREATE TABLE {nameof(Student)}(
                Id INT PRIMARY KEY NOT NULL,
                Name VARCHAR(32) NOT NULL,
                AGE INT NOT NULL,
                BIRTHDAY VARCHAR(32) NOT NULL
            );
            """);
        }
    }
    static string DBPath => $"{FilePath}\\machines.db";
    static string FilePath => $"{Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!}\\LiteDB";
}