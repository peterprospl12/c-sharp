using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace PTLab12___ClientServer;

public class Server
{
    private TcpListener _server;
    private bool _isRunning;
    
    public Server(int port)
    {
        _server = new TcpListener(IPAddress.Any, port);
        _server.Start();
        _isRunning = true;
        Console.WriteLine("Server started on port " + port);
    }
    
    public void Run()
    {
        while (_isRunning)
        {
            // One thread to accept the client and another to proccess the client serialized data and send it back
            TcpClient client = _server.AcceptTcpClient();
            new Thread(() =>
            {
                ProcessClient(client);
            }).Start();
        }
    }

    public void ProcessClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        Computer computer = JsonSerializer.Deserialize<Computer>(data);
        Console.WriteLine("Received: " + computer.ToString());
        computer.Ram *= 2;
        string dataToSend = JsonSerializer.Serialize(computer);
        byte[] dataToSendBytes = Encoding.UTF8.GetBytes(dataToSend);
        stream.Write(dataToSendBytes, 0, dataToSendBytes.Length);

        client.Close();
    }
    
    public void Stop()
    {
        _isRunning = false;
        _server.Stop();
    }
    
    
    
    
}