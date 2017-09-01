using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_Auction_House_Server { // Split different Methods out to different Classes, to improve Design and readability.

     
    public class Server {

        /* ¤ is used only to places meant to be split!
         *
         * ITEM - Infront of text that is refering to a item
         * BID - Infront of text that is refering to a bid change.
         * MESSAGE - Infront of text that is refering to a simple message from the server to the client(s).
         * IP - Infront of text that is refering to a IP.
         * ERROR - Infront of text that is refering to a error.
         */

        static void Main(string[] args) {
            Server Run = new Server();
            Run.Run();
        }

        private void Run() {
            Console.WriteLine("Online");

            Repo.Items.Add(new Item("Ferrari 250 GTO", 130000000, "car"));
            Repo.Items.Add(new Item("Fiat Multipla", 20000, "world ugliest car"));
            Repo.Items.Add(new Item("Arne Jacobsen Svanen", 15000, "Armchair"));
            Repo.Items.Add(new Item("Rolex Submariner", 120000, "18 karat gold"));
            Repo.Items.Add(new Item("Poul Henningsen: Koglen",35000 , "Pendel"));
            Repo.Items.Add(new Item("Gaffatape", 100 , "100m"));
            Repo.Items.Add(new Item("Shure Sm7b", 3000 , "Microphone"));
            Repo.Items.Add(new Item("Gold", 260000 , "1kg"));
            Repo.Items.Add(new Item("Uran", 9500 , "4g"));
            Repo.Items.Add(new Item("Garden fertilizer", 90 , "7.5kg"));

            Repo.Lock.WaitOne();
            Repo.Bid = Repo.Items[Repo.AuctionItem].startPrice;
            Repo.Lock.ReleaseMutex();

            TcpListener Server = new TcpListener(IPAddress.Any, 12345);
            TcpClient Client;
            Server.Start();

            while (true) {
                Client = Server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ClientConnection, Client);
            }
            Console.WriteLine("Offline");
            Console.ReadKey();
        }
        private void ClientConnection(object obj) { // Refactor into a ClientHandler Class
            var client = (TcpClient)obj;

            ClientHandler handler = new ClientHandler(client);

            Repo.Clients.Add(handler);

            Console.WriteLine("New Connection: " + handler.IP());
            Console.WriteLine(Repo.Clients.Count + " Client(s) Connected");

            handler.Handle();
        }

        private void TestPrint()
        {
            foreach (var log in Repo.BidLog)
            {
                Console.WriteLine(log);
            }
        }
    }
}
