using Rely = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Elite.IIoT.Core.Architects.Repositories;
public interface ISQLiteOperation
{
    Task AddAsync<T>(T entity) where T : SQLiteBase;
    Task PutAsync<T>(int id, T entity) where T : SQLiteBase;
    Task DeleteAsync<T>(int id) where T : SQLiteBase;
    Task<T> GetAsync<T>(int id) where T : SQLiteBase;
    Task<IEnumerable<T>> GetAsync<T>() where T : SQLiteBase;
}

[Rely(ServiceLifetime.Singleton)]
file sealed class SQLiteOperation : SQLiteHelper, ISQLiteOperation
{
    public async Task AddAsync<T>(T entity) where T : SQLiteBase
    {
        await new StandardTrim(new BasicOperation()).CreateAsync(entity);
    }
    public async Task PutAsync<T>(int id, T entity) where T : SQLiteBase
    {
        await new StandardTrim(new BasicOperation()).UpdateAsync(id, entity);
    }
    public async Task DeleteAsync<T>(int id) where T : SQLiteBase
    {
        await new StandardTrim(new BasicOperation()).DeleteAsync<T>(id);
    }
    public async Task<T> GetAsync<T>(int id) where T : SQLiteBase
    {
        return await new StandardTrim(new BasicOperation()).SelectAsync<T>(id);
    }
    public async Task<IEnumerable<T>> GetAsync<T>() where T : SQLiteBase
    {
        return await new StandardTrim(new BasicOperation()).SelectAsync<T>();
    }
}