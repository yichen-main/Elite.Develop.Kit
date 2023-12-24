var channel = Channel.CreateUnbounded<Message>(new UnboundedChannelOptions
{
    SingleWriter = false,
    SingleReader = false,
    AllowSynchronousContinuations = true
});

var sender1 = SendMessageThreadAsync(channel.Writer, 1);
var sender2 = SendMessageThreadAsync(channel.Writer, 2);

var receiver = ReceiveMessageThreadAsync(channel.Reader, 1);

//發送者
await Task.WhenAll(sender1, sender2);

// 確保收到所有訊息(沒有生產者關閉)
channel.Writer.Complete();

//接收者
await receiver;



Console.WriteLine("Press any key to exit...");
Console.ReadKey();

//https://github.com/BYJRK/CSharpChannelDemo
//https://www.cnblogs.com/ireadme/p/14502286.html
//https://www.cnblogs.com/tiger-wang/p/14068973.html
//https://learn.microsoft.com/zh-tw/dotnet/core/extensions/channels

//https://mp.weixin.qq.com/s/ZMcCcUlHWA8Fids-OxyiYg

async Task SendMessageThreadAsync(ChannelWriter<Message> writer, int id)
{
    for (int i = 1; i <= 20; i++)
    {
        await writer.WriteAsync(new Message(id, i.ToString()));
        Console.WriteLine($"Thread {id} sent {i}");
        await Task.Delay(100);
    }
    //我們不在這裡完成作者，因為可能有多個寄件者
}

async Task ReceiveMessageThreadAsync<T>(ChannelReader<T> reader, int id)
{
    await foreach (var item in reader.ReadAllAsync())
    {
        switch (item)
        {
            case Message message:
                Console.WriteLine($"Thread {id} received {message.Content}");
                break;
        }
    }
}

record Message(int FromId, string Content);