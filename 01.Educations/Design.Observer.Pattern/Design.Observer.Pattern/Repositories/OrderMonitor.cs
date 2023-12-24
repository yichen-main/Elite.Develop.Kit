namespace Design.Observer.Pattern.Repositories;
internal class OrderMonitor(string name) : IObserver<OrderInfo>
{
    private readonly string _name = name;
    private readonly List<int> _orders = new();
    private IDisposable? _cancellation;
    public virtual void Subscribe(OrderProvider provider)
    {
        provider.SubscribeName = _name;
        _cancellation = provider.Subscribe(this);
    }
    public virtual void Unsubscribe()
    {
        _cancellation?.Dispose();
        _orders.Clear();
    }
    public void OnCompleted()
    {
        Console.WriteLine($"{_name}: Order Count:{_orders.Count}.");
    }
    public void OnError(Exception error)
    {
        Console.WriteLine($"Error:{error.Message}");
    }
    public void OnNext(OrderInfo value)
    {
        var updated = false;
        if (value.Number > 0 && string.IsNullOrEmpty(value.Customer) is false)
        {
            if (_orders.Any(c => c.Equals(value.Number)) is false)
            {
                _orders.Add(value.Number);
                updated = true;
            }
        }
        if (updated)
        {
            Console.WriteLine($"{_name}: Order Created, Number {value.Number}.");
            //foreach (var info in _orders)
            //{
            //    Console.WriteLine($"{_name}: Order Number:{info}.");
            //}               
        }
        else
        {
            var item = _orders.Where(c => c.Equals(value.Number)).FirstOrDefault();
            _orders.Remove(item);

            Console.WriteLine($"{_name}: Order Removed, Number {value.Number}.");
        }
        Console.WriteLine();
    }
}