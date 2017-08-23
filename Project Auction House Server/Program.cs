using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_Auction_House_Server {
    class Program {

        List<string> BidLog = new List<string>();
        static List<TcpClient> ClientIPS = new List<TcpClient>();

        int Bid = 0;

        static void Main(string[] args)
        {
            Program Run = new Program();
            Run.Run();
        }

        private void Run()
        {
            TcpListener Server = new TcpListener(IPAddress.Any, 12345);
            TcpClient Client;

            Server.Start();

            while (true)
            {
                Client = Server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ClientConnection, Client);
            }
        }
        private void ClientConnection(object obj)
        {
            var client = (TcpClient)obj;

            StreamReader read = new StreamReader(client.GetStream());
            StreamWriter write = new StreamWriter(client.GetStream());
            IPEndPoint IPEP = (IPEndPoint)client.Client.RemoteEndPoint;
            ClientIPS.Add(client);

            Console.WriteLine("New Connection: " + IPEP);

            SendFTInfo(client);


        }
        private void Broadcast()
        {
            Console.WriteLine("Broadcasting...");
            foreach (var client in ClientIPS)
            {
                StreamWriter write = new StreamWriter(client.GetStream());
                write.AutoFlush = true;
                write.WriteLine("test1");
            }
        }
        private void SendFTInfo(TcpClient client)
        {
            StreamWriter write = new StreamWriter(client.GetStream());
            write.AutoFlush = true;
            Console.WriteLine("Sending Current Auction to New Client");
            write.WriteLine("test2");
        }
    }
}
