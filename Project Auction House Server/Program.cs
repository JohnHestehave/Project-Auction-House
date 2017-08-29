﻿using System;
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

        // ¤ is used only to places meant to be split!

        List<string> BidLog = new List<string>();
        static List<TcpClient> ClientIPS = new List<TcpClient>();
        static List<Item> items = new List<Item>();

        int AuctionItem = 0;

        int Bid = 0;

        static void Main(string[] args) {
            Program Run = new Program();
            Run.Run();
        }

        private void Run() {
            Console.WriteLine("Online");

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

            Bid = items[AuctionItem].startPrice;
            
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
        private void ClientConnection(object obj) {
            var client = (TcpClient)obj;
            

            StreamReader read = new StreamReader(client.GetStream());
            StreamWriter write = new StreamWriter(client.GetStream());
            write.AutoFlush = true;
            
            IPEndPoint IPEP = (IPEndPoint)client.Client.RemoteEndPoint;

            ClientIPS.Add(client);

            Console.WriteLine("New Connection: " + IPEP);
            Console.WriteLine(ClientIPS.Count + " Client(s) Connected");

            SendFTInfo(client);
            ShowBidders();

            while (true)
            {
                string message = read.ReadLine();
                bool conversion;
                int BidAttempt = 0;

                if (message != "" && message != "EXIT")
                {
                    BidLog.Add(message);
                    TestPrint();
                    conversion = int.TryParse(message, out BidAttempt);
                }
                if (BidAttempt > Bid)
                {
                    Bid = BidAttempt;
                    Broadcast();
                }
                if (!client.Connected || message == "EXIT")
                {
                    if (message == "EXIT")
                    {
                        client.Close();
                    }
                    ClientIPS.Remove(client);
                    Console.WriteLine(IPEP + " - Disconnected");
                    Console.WriteLine(ClientIPS.Count + " Client(s) Connected");
                    break;
                }
            }
        }

        private void TestPrint()
        {
            foreach (var log in BidLog)
            {
                Console.WriteLine(log);
            }
        }

        private void ShowBidders() {
            string message = "";
            foreach (var clientIp in ClientIPS) {
                IPEndPoint ipep = (IPEndPoint)clientIp.Client.RemoteEndPoint;
                message += ipep.Address.ToString() + "¤";
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
                write.WriteLine(Bid);
            }
        }
        private void SendFTInfo(TcpClient client) {
            StreamWriter write = new StreamWriter(client.GetStream());
            write.AutoFlush = true;
            Console.WriteLine("Sending Current Auction to New Client");
            //hardcoded item "0" intil vi har flere auctions
            write.WriteLine(items[AuctionItem].startPrice.ToString() + "¤" + items[AuctionItem].name.ToString() + "¤" + items[AuctionItem].description.ToString());
        }
    }
}
