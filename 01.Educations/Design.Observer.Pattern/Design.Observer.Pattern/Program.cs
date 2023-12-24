try
{
    OrderProvider provider = new("provider");
    OrderMonitor observer1 = new("observer1");
    OrderMonitor observer2 = new("observer2");

    observer1.Subscribe(provider);
    observer2.Subscribe(provider);

    provider.OrderStatus(1);
    provider.OrderStatus(2);

    observer1.OnCompleted();
    observer2.OnCompleted();

    observer2.Unsubscribe();

    observer1.OnCompleted();
    observer2.OnCompleted();

    provider.OrderStatus(2);
    observer1.OnCompleted();
    observer2.OnCompleted();

    Console.ReadLine();
}
catch(Exception e)
{
    Console.WriteLine(e.Message);
}

//https://medium.com/ricos-note/%E7%B0%A1%E5%96%AE%E5%AF%A6%E7%8F%BE%E8%A7%80%E5%AF%9F%E8%80%85%E6%A8%A1%E5%BC%8F-23890758ba71