using DependencyAttribute = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Elite.Core.Databases.Relations.Repositories;
public interface IRelationTool
{
    void SetConnectionString(in string? value);
    string? ConnectRDB { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class RelationTool : IRelationTool
{
    public void SetConnectionString(in string? value) => ConnectRDB = value;
    public string? ConnectRDB { get; private set; }
}