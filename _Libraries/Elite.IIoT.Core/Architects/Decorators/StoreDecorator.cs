namespace Elite.IIoT.Core.Architects.Decorators;
public abstract class StoreDecorator
{
    protected interface IOperation
    {
        ValueTask DeleteAsync<T>(long id);
        ValueTask CreateAsync<T>(T entity);
        ValueTask UpdateAsync<T>(long id, T entity);
        ValueTask<T> SelectAsync<T>(long id);
        ValueTask<IEnumerable<T>> SelectAsync<T>();
    }
    protected abstract class OperationDecoration(IOperation operation) : IOperation
    {
        public virtual async ValueTask DeleteAsync<T>(long id) => await operation.DeleteAsync<T>(id);
        public virtual async ValueTask CreateAsync<T>(T entity) => await operation.CreateAsync(entity);
        public virtual async ValueTask UpdateAsync<T>(long id, T entity) => await operation.UpdateAsync(id, entity);
        public virtual async ValueTask<T> SelectAsync<T>(long id) => await operation.SelectAsync<T>(id);
        public virtual async ValueTask<IEnumerable<T>> SelectAsync<T>() => await operation.SelectAsync<T>();
    }
}