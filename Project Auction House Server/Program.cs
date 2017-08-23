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
        static List<Item> items = new List<Item>();

        int Bid = 0;

        static void Main(string[] args) {
            Program Run = new Program();
            Run.Run();
        }

        private void Run() {

            items.Add(new Item("Ferrari 250 GTO", 130000000, "car"));
            items.Add(new Item("Fiat Multipla", 20000, "world ugliest car"));
            items.Add(new Item("Arne Jacobsen Svanen", 15000, "Armchair"));
            items.Add(new Item("Rolex Submariner", 120000, "18 karat gold"));
            items.Add(new Item("Poul Henningsen: Koglen",35000 , "Pendel"));
            items.Add(new Item("Gaffatape", 100 , "100m"));
            items.Add(new Item("Shure Sm7b", 3000 , "Microphone"));
            items.Add(new Item("Gold", 260000 , "1kg"));
            items.Add(new Item("Uran", 9500 , "4g"));
            items.Add(new Item("Garden fertilizer", 90 , "7.5kg"));
            
            TcpListener Server = new TcpListener(IPAddress.Any, 12345);
            TcpClient Client;
            Server.Start();

            while (true) {
                Client = Server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ClientConnection, Client);
            }
        }
        private void ClientConnection(object obj) {
            var client = (TcpClient)obj;

            StreamReader read = new StreamReader(client.GetStream());
            StreamWriter write = new StreamWriter(client.GetStream());
            IPEndPoint IPEP = (IPEndPoint)client.Client.RemoteEndPoint;
            ClientIPS.Add(client);

            Console.WriteLine("New Connection: " + IPEP);

            //SendFTInfo(client);
            showBidders();
        }

        private void showBidders() {
            string message = "";
            foreach (var clientIp in ClientIPS) {
                IPEndPoint ipep = (IPEndPoint)clientIp.Client.RemoteEndPoint;
                message += ipep.Address.ToString() + "\n";
            }

            foreach (var client in ClientIPS) {
                StreamWriter write = new StreamWriter(client.GetStream());
                write.AutoFlush = true;
                write.WriteLine(message);
            }
        }

        private void Broadcast() {
            Console.WriteLine("Broadcasting...");
            foreach (var client in ClientIPS) {
                StreamWriter write = new StreamWriter(client.GetStream());
                write.AutoFlush = true;
                write.WriteLine("test1");
            }
        }
        private void SendFTInfo(TcpClient client) {
            StreamWriter write = new StreamWriter(client.GetStream());
            write.AutoFlush = true;
            Console.WriteLine("Sending Current Auction to New Client");
            //hardcoded item "0" intil vi har flere auctions
            write.WriteLine(items[0].name.ToString() + "\n" + items[0].startPrice.ToString() + "\n" + items[0].description.ToString());
        }
    }
}
