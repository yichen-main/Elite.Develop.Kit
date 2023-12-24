namespace Design.Observer.Pattern.Repositories;
internal class OrderProvider(string name) : IObservable<OrderInfo>
{
    readonly HashSet<IObserver<OrderInfo>> _observers = new();
    readonly HashSet<OrderInfo> _orders = new();
    readonly string _name = name;
    public string SubscribeName { get; set; } = string.Empty;
    public IDisposable Subscribe(IObserver<OrderInfo> observer)
    {
        if (_observers.Add(observer))
        {
            //提供所有訂單資訊給觀察者
            foreach (OrderInfo item in _orders)
            {
                observer.OnNext(item);
            }
        }
        return new Unsubscriber<OrderInfo>(SubscribeName, _observers, observer);
    }
    protected void Notification(OrderInfo info)
    {
        //提供訂單資訊給所有觀察者
        foreach (var observer in _observers)
        {
            observer.OnNext(info);
        }
    }
    public void OrderStatus(int number) => OrderStatus(number, $"Customer_{number}");
    private void OrderStatus(int number, string customer)
    {
        var info = new OrderInfo(number, customer);

        //add new info object to list.
        if (number > 0 && _orders.Add(info))
        {
            foreach (IObserver<OrderInfo> observer in _observers)
            {
                observer.OnNext(info);
            }
        }
        else
        {
            // order is done.
            if (_orders.RemoveWhere(order => order.Number == info.Number) > 0)
            {
                foreach (IObserver<OrderInfo> observer in _observers)
                {
                    observer.OnNext(info);
                }
            }
        }
    }
    class Unsubscriber<OrderInfo> : IDisposable
    {
        readonly ISet<IObserver<OrderInfo>> _observers;
        readonly IObserver<OrderInfo> _observer;
        readonly string _name;
        public Unsubscriber(string name, ISet<IObserver<OrderInfo>> observers,
            IObserver<OrderInfo> observer) => (_name, _observers, _observer) = (name, observers, observer);
        public void Dispose()
        {
            if (_observer is not null && _observers.Contains(_observer))
            {
                _observers.Remove(_observer);
                Console.WriteLine($"{_name} UnSubscribed");
            }
        }
    }
}