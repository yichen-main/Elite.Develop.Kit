using System.IO;
using System.Net.Sockets;
using System.Net;

try
{
    // 当前服务器使用的ip和端口
    TcpListener tcpListener = new(new IPEndPoint(IPAddress.Parse("192.168.1.99"), 8234));
    tcpListener.Start();
    Console.WriteLine("服务端已启用......"); // 阻塞线程的执行，直到一个客户端连接
    TcpClient tcpClient = tcpListener.AcceptTcpClient();
    Console.WriteLine("已连接.");
    var stream = tcpClient.GetStream();          // 创建用于发送和接受数据的NetworkStream
}
catch (Exception e)
{

}