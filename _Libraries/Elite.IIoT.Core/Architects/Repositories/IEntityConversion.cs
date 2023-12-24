using Rely = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Elite.IIoT.Core.Architects.Repositories;
public interface IEntityConversion
{
    string GetDescription(in Enum item);
    IDictionary<string, object> JsonToDictionary(in string text);
    IDictionary<int, (string name, string description)> EnumToDictionary<T>() where T : Enum;
    void MonitorFile(in Action<object, FileSystemEventArgs> action, in string path, in string name);
    IEnumerable<TSource> Concat<TSource>(IEnumerable<TSource> originals, IEnumerable<TSource> merges);
    IEnumerable<T> FindRepeat<T>(in T[] entities);
    T[] Concat<T>(in T[] array1, in T[] array2);
    bool CheckParity<T>(in IEnumerable<T> fronts, in IEnumerable<T> backs) where T : notnull;
    void Clear(EventHandler? @event);
}

[Rely(ServiceLifetime.Singleton)]
file sealed class EntityConversion : IEntityConversion
{
    public string GetDescription(in Enum item) =>
        item.GetType().GetRuntimeField(item.ToString())!.GetCustomAttribute<DescriptionAttribute>()!.Description;
    public IDictionary<string, object> JsonToDictionary(in string text)
    {
        Dictionary<string, object> results = new(StringComparer.Ordinal);
        foreach (var item in JsonSerializer.Deserialize<JsonObject>(text ?? string.Empty)!)
        {
            object @object;
            switch (item.Value)
            {
                case null:
                    @object = null!;
                    break;

                case JsonArray:
                    @object = item.Value.AsArray();
                    break;

                case JsonObject:
                    @object = item.Value.AsObject();
                    break;

                default:
                    var jsonValue = item.Value.AsValue();
                    if (item.Value.ToJsonString().StartsWith('"'))
                    {
                        if (jsonValue.TryGetValue<DateTime>(out var dateTime)) @object = dateTime;
                        else if (jsonValue.TryGetValue<Guid>(out var guid)) @object = guid;
                        else @object = jsonValue.GetValue<string>();
                    }
                    else @object = jsonValue.GetValue<decimal>();
                    break;
            }
            results.Add(item.Key, @object);
        }
        return results;
    }
    public IDictionary<int, (string name, string description)> EnumToDictionary<T>() where T : Enum
    {
        Dictionary<int, (string name, string description)> results = [];
        foreach (Enum item in Enum.GetValues(typeof(T))) results.Add(item.GetHashCode(), (item.ToString(), GetDescription(item)));
        return results.ToFrozenDictionary();
    }   
    public void MonitorFile(in Action<object, FileSystemEventArgs> action, in string path, in string name) => new FileSystemWatcher
    {
        Path = path,
        Filter = name,
        EnableRaisingEvents = true,
        IncludeSubdirectories = false,
        NotifyFilter = NotifyFilters.LastWrite,
    }.Changed += new FileSystemEventHandler(action);
    public IEnumerable<TSource> Concat<TSource>(IEnumerable<TSource> originals, IEnumerable<TSource> merges)
    {
        foreach (var item in originals) yield return item;
        foreach (var item in merges) yield return item;
    }
    public IEnumerable<T> FindRepeat<T>(in T[] entities)
    {
        HashSet<T> hashset = [];
        return entities.Where(item => !hashset.Add(item));
    }
    public T[] Concat<T>(in T[] array1, in T[] array2)
    {
        ArgumentNullException.ThrowIfNull(array1);
        ArgumentNullException.ThrowIfNull(array2);
        var length1 = array1.Length;
        var length2 = array2.Length;
        var result = new T[length1 + length2];
        Array.Copy(array1, result, length1);
        Array.Copy(array2, default, result, length1, length2);
        return result;
    }
    public bool CheckParity<T>(in IEnumerable<T> fronts, in IEnumerable<T> backs) where T : notnull
    {
        Dictionary<T, int> result = [];
        foreach (var item in fronts)
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrNullRef(result, item);
            if (!Unsafe.IsNullRef(ref value)) value++;
            else result.Add(item, 1);
        }
        foreach (var item in backs)
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrNullRef(result, item);
            if (!Unsafe.IsNullRef(ref value)) value--;
            else return default;
        }
        return result.Values.All(item => item == default);
    }
    public void Clear(EventHandler? @event)
    {
        if (@event is not null) Array.ForEach(@event.GetInvocationList(), item => @event -= (EventHandler)item);
    }
}