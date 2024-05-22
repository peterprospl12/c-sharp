using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace PTLab12___ClientServer;

public class Client
{
    private TcpClient _client;
    
    public Client(string ip, int port)
    {
        _client = new TcpClient(ip, port);
    }

    
    public void Send()
    {
     
        NetworkStream stream = _client.GetStream();
        Random random = new Random();
        int randomRam = random.Next(1, 32);
        int randomHDD = random.Next(1, 4);
        var computer = new Computer("Intel i7", randomRam, randomHDD);
        string jsonString = JsonSerializer.Serialize(computer);
        byte[] data = Encoding.UTF8.GetBytes(jsonString);
        Console.WriteLine("Sending: " + computer.ToString());
        stream.Write(data, 0, data.Length);

        byte[] buffer = new byte[_client.ReceiveBufferSize];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Computer modifiedComputer = JsonSerializer.Deserialize<Computer>(response);
        Console.WriteLine("Received: " + modifiedComputer.ToString());

    }
    
    public void Close()
    {
        _client.Close();
    }
    
    
    
    
}