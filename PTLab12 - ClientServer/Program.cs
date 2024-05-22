
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PTLab12___ClientServer;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: PTLab12-ClientServer.exe <server/client> <port> <message>");
            return;
        }
        
        if (args[0] == "server")
        {
            Server server = new Server(int.Parse(args[1]));
            server.Run();
        }
        else if (args[0] == "client")
        {
            for (int i = 0; i < 10; i++)
            {
                Client client = new Client("localhost", int.Parse(args[1]));
                client.Send();
            }
        }


    }
}