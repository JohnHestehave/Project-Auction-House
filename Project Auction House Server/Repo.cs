using System;
using System.Collections.Generic;
using System.Threading;

namespace Project_Auction_House_Server {
    public static class Repo
    {
        public static List<string> BidLog = new List<string>();
        public static List<ClientHandler> Clients = new List<ClientHandler>();
        public static List<Item> Items = new List<Item>();

        public static int AuctionItem = 0;
        private static int _bid = 0;
        public static ClientHandler WinningClient;
        public static Mutex Lock = new Mutex();

        public static int Bid {
            get { return _bid; }
            set
            {
                if (_bid != value)
                {
                    _bid = value;
                    Broadcast();
                }
            }
        }
        private static void Broadcast()
        {
            Console.WriteLine("Broadcasting...");
            foreach (var client in Clients)
            {
                if (WinningClient != null)
                {
                    client.Writer().WriteLine("BID¤" + Bid + "¤" + WinningClient.IP());
                }
                else
                {
                    client.Writer().WriteLine("BID¤" + Bid + "¤" + "No one is currently Winning");
                }
            }
        }
    }
}
