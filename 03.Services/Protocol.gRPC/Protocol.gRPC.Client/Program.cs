using GrpcServer.HumanResource;

try
{
    await Task.Delay(3 * 1000);  //等待 Server 啟動
    using var channel = GrpcChannel.ForAddress("http://localhost:5432", new GrpcChannelOptions
    {
        //在閒置期間每隔 60 秒將保持運作 Ping 傳送至伺服器。 Ping 可確保伺服器和使用中的任何 Proxy 都不會因為閒置而關閉連線。
        HttpHandler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan
        }
    });
    var client = new Employee.EmployeeClient(channel);

    {//Single data

        //Read
        var read = await client.GetEmployeeAsync(new EmployeeRequest
        {
            Id = 3
        });
        var result = JsonSerializer.Serialize(read, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        //Write
        var write = await client.AddEmployeeAsync(new()
        {
            Id = 2,
            Name = "Johnny",
            EmployeeType = EmployeeType.FirstLevel,
            PhoneNumbers =
            {
                new EmployeeModel.Types.PhoneNumber
                {
                    Value = "0912345678"
                }
            },
            ModifiedTime = new DateTime(2019, 10, 03, 17, 45, 00).Ticks
        });
    }
    {//Multiple data

        //Read
        var reads = client.GetEmployees(new EmployeeRequest
        {
            Id = 5
        }, default);
        var results = JsonSerializer.Serialize(reads, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        //Write
        List<EmployeeModel> datas = new()
        {
            new()
            {
                Id = 1,
                Name = "Johnny",
                EmployeeType = EmployeeType.FirstLevel,
                PhoneNumbers =
                {
                    new EmployeeModel.Types.PhoneNumber
                    {
                        Value = "0912345678"
                    }
                },
                ModifiedTime = new DateTime(2019, 10, 3, 17, 45, 00).Ticks
            },
            new()
            {
                Id = 2,
                Name = "Mary",
                EmployeeType = EmployeeType.SecondLevel,
                PhoneNumbers =
                {
                    new EmployeeModel.Types.PhoneNumber
                    {
                        Value = "0923456789"
                    }
                },
                ModifiedTime = new DateTime(2019, 10, 4, 9, 21, 00).Ticks
            }
        };
        using var creator = client.AddEmployees();
        {
            //將資料逐一輸出至 Server
            foreach (var data in datas)
            {
                await creator.RequestStream.WriteAsync(data);
            }
            await creator.RequestStream.CompleteAsync();
            var result = await creator.ResponseAsync;
        }
    }
    Console.ReadKey();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}